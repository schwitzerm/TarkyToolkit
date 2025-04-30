using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
// ReSharper disable StringLiteralTypo

namespace TarkyToolkit.LogServer
{
    internal static class Program
    {
        private static readonly List<string> LogMessages = new List<string>();
        private static readonly object LogLock = new object();
        private static bool _isRunning = true;

        private static void Main()
        {
            Console.WriteLine("Starting TarkyToolkit Log Server on http://localhost:22322");
            Console.WriteLine("Press Ctrl+C to exit");

            // Set up the HTTP server
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:22322/");

            try
            {
                listener.Start();

                // Handle Ctrl+C to gracefully shut down
                Console.CancelKeyPress += (sender, e) => {
                    e.Cancel = true;
                    _isRunning = false;
                    Console.WriteLine("Shutting down server...");
                };

                // Start a thread to handle incoming requests
                ThreadPool.QueueUserWorkItem(ListenerCallback, listener);

                // Keep the application running until Ctrl+C
                while (_isRunning)
                {
                    Thread.Sleep(100);
                }

                // Clean up
                listener.Stop();
                Console.WriteLine("Server stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting server: {ex.Message}");
            }
        }

        private static void ListenerCallback(object state)
        {
            var listener = (HttpListener)state;

            while (_isRunning)
            {
                try
                {
                    var context = listener.GetContext();
                    ProcessRequest(context);
                }
                catch (Exception ex) when (_isRunning)
                {
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }
                catch (Exception)
                {
                    // Ignore exceptions during shutdown
                }
            }
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST")
                {
                    // Process log messages from POST
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var logContent = reader.ReadToEnd();
                        var logLines = logContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        lock (LogLock)
                        {
                            foreach (var line in logLines)
                            {
                                LogMessages.Add($"[{DateTime.Now:HH:mm:ss.fff}] {line}");
                                Console.WriteLine($"Received: {line}");
                            }

                            // Keep only the last 1000 messages
                            if (LogMessages.Count > 1000)
                            {
                                LogMessages.RemoveRange(0, LogMessages.Count - 1000);
                            }
                        }
                    }

                    // Send 200 OK response
                    response.StatusCode = 200;
                    response.Close();
                }
                else if (request.HttpMethod == "GET")
                {
                    // Serve the HTML log viewer
                    var html = GenerateHtmlLogViewer();
                    var buffer = Encoding.UTF8.GetBytes(html);

                    response.ContentType = "text/html";
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.Close();
                }
                else
                {
                    // Method isn't supported
                    response.StatusCode = 405;
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in request processing: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private static string GenerateHtmlLogViewer()
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta http-equiv=\"refresh\" content=\"2\">");
            html.AppendLine("    <script>");
            html.AppendLine("        // Store the scroll position before refresh");
            html.AppendLine("        window.addEventListener('beforeunload', function() {");
            html.AppendLine("            const container = document.querySelector('.log-container');");
            html.AppendLine("            if (container) {");
            html.AppendLine("                const scrollIsAtBottom = container.scrollHeight - container.scrollTop - container.clientHeight < 5;");
            html.AppendLine("                localStorage.setItem('scrollAtBottom', scrollIsAtBottom);");
            html.AppendLine("            }");
            html.AppendLine("        });");
            html.AppendLine("    </script>");
            html.AppendLine("    <title>TarkyToolkit Log Viewer</title>");
            html.AppendLine("    <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/hack-font@3/build/web/hack.css\">");
            html.AppendLine("    <style>");
            html.AppendLine("        @font-face {");
            html.AppendLine("            font-family: 'Hack';");
            html.AppendLine("            src: url('https://cdn.jsdelivr.net/npm/hack-font@3/build/web/fonts/hack-regular.woff2') format('woff2');");
            html.AppendLine("            font-display: swap;");
            html.AppendLine("        }");
            html.AppendLine("        body { font-family: 'Hack', 'Consolas', 'DejaVu Sans Mono', monospace; margin: 0; padding: 5px; background: #0c0c0c; color: #d4d4d4; }");
            html.AppendLine("        .fonts-loaded { opacity: 1; }");
            html.AppendLine("        .log-container { background: #000000; border-radius: 2px; padding: 3px; white-space: pre; font-size: 12px; line-height: 1.05; height: calc(100vh - 70px); overflow-y: auto; border: 1px solid #333; }");
            html.AppendLine("        .log-line { margin: 0; padding: 0; font-family: Hack, monospace; letter-spacing: 0; height: 1.05em; overflow: hidden; }");
            html.AppendLine("        .debug { color: #569cd6; }");
            html.AppendLine("        .info { color: #9cdcfe; }");
            html.AppendLine("        .warn { color: #ce9178; }");
            html.AppendLine("        .error { color: #f44747; }");
            html.AppendLine("        .timestamp { color: #6a9955; }");
            html.AppendLine("        h1 { color: #dcdcaa; margin: 0 0 5px 0; font-size: 18px; }");
            html.AppendLine("        p { margin: 0 0 5px 0; font-size: 12px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <h1>TarkyToolkit Log Viewer</h1>");
            html.AppendLine("    <div class=\"log-container\">");

            lock (LogLock)
            {
                // Display messages in chronological order (oldest first, newest last)
                for (int i = 0; i < LogMessages.Count; i++)
                {
                    var message = LogMessages[i];

                    // Apply CSS class based on the log level
                    string cssClass = "info"; // Default
                    if (message.Contains("[Debug")) cssClass = "debug";
                    else if (message.Contains("[Warn")) cssClass = "warn";
                    else if (message.Contains("[Error")) cssClass = "error";

                    // Extract timestamp
                    int bracketIndex = message.IndexOf(']');
                    if (bracketIndex > 0)
                    {
                        string timestamp = message.Substring(0, bracketIndex + 1);
                        string logContent = message.Substring(bracketIndex + 1);

                        html.AppendLine($"<div class=\"log-line\"><span class=\"timestamp\">{timestamp}</span><span class=\"{cssClass}\">{logContent}</span></div>");
                    }
                    else
                    {
                        html.AppendLine($"<div class=\"log-line {cssClass}\">{message}</div>");
                    }
                }
            }

            html.AppendLine("    </div>");
            html.AppendLine("    <p><small>Auto-refreshes every 2s • Using Hack font</small></p>");
            html.AppendLine("    <script>");
            html.AppendLine("        // Auto-scroll to bottom of log container on load");
            html.AppendLine("        window.onload = function() {");
            html.AppendLine("            var container = document.querySelector('.log-container');");
            html.AppendLine("            // Check if we should scroll to bottom (either first load or was at bottom before refresh)");
            html.AppendLine("            const wasAtBottom = localStorage.getItem('scrollAtBottom') !== 'false';");
            html.AppendLine("            if (wasAtBottom) {");
            html.AppendLine("                container.scrollTop = container.scrollHeight;");
            html.AppendLine("            }");
            html.AppendLine("            // Add class after fonts are loaded to prevent layout shift");
            html.AppendLine("            document.body.classList.add('fonts-loaded');");
            html.AppendLine("        }");
            html.AppendLine("    </script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}
