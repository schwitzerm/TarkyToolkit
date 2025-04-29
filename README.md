# TarkyToolkit
A no nonsense SPT API.

.NET 4.7.1, C# 14.0

Set the environment variable `SPTCLIENTPLUGINSPATH` to... well where the SPT Client plugins go. This is usually `<YourSPTInstall>/BepInEx/plugins`.

To do so, run:
```setx SPTCLIENTPLUGINSPATH "<YourPluginsPath>"```

In my case (using Windows path separators...):
```set SPTCLIENTPLUGINSPATH "E:\SPT\BepInEx\plugins"```

Read the README files in each folder in lib to know what to put where. 
If you don't like opening multiple files:
   * Copy all files from `<YourSPTInstall>/BepInEx/core` to `lib/BepInEx`.
   * Copy all files from `<YourSPTInstall>/BepInEx/plugins/spt` to `lib/SPT`.
   * Copy all files from `<YourSPTInstall>/EscapeFromTarkov_Data/Managed` to `lib/TarkovManaged`.

Press build. Enjoy your new Tarky Toolkit.


(This readme is dogshit, it will be replaced.)
