# TarkyToolkit Log Server

A simple HTTP server for displaying logs sent from the TarkyToolkit's BatchHttpLogger.

## Features

- Receives log messages over HTTP POST
- Displays logs in a readable, color-coded web interface
- Automatically refreshes to show new log messages
- Supports different log levels (Debug, Info, Warning, Error)

## Usage

1. Start the log server:
   ```
   dotnet run
   ```
   
2. The server will start listening on `http://localhost:22322`

3. Open a web browser and navigate to `http://localhost:22322` to view the logs

4. Logs will appear automatically as they are sent from the BatchHttpLogger

5. Press Ctrl+C in the console to stop the server

## Note

This server is designed to work specifically with the BatchHttpLogger component of TarkyToolkit, which sends formatted log messages as plain text over HTTP POST requests.
