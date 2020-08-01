# MsBuild Task Explorer

[![Build status](https://ci.appveyor.com/api/projects/status/aq6wyq5n3d6fn0p3?svg=true)](https://ci.appveyor.com/project/Serg046/msbuildtaskexplorer) [![](https://vsmarketplacebadge.apphb.com/version-short/saaseev.MsBuildTaskExplorer.svg)](https://marketplace.visualstudio.com/items?itemName=saaseev.MsBuildTaskExplorer) [![](https://vsmarketplacebadge.apphb.com/downloads-short/saaseev.MsBuildTaskExplorer.svg)](https://marketplace.visualstudio.com/items?itemName=saaseev.MsBuildTaskExplorer)

Visual Studio Add-In for developers who would like to execute MsBuild targets inside Visual Studio.
The add-in analyzes the solution folders to find MsBuild project files (.proj) to get targets and then lists it to provide ability to execute them.

![targets](https://i.imgur.com/rX79jVa.png)

## Possibilities
1. Get solution targets.
2. Search for the concrete target by text pattern.
3. Target execution with log in the Output Window.
4. Print all target properties to the Output Window.

![log](http://i.imgur.com/b2J7mo7.png)

## Getting started
1. Install [MsBuildTaskExplorer](https://marketplace.visualstudio.com/items?itemName=saaseev.MsBuildTaskExplorer).
2. Click Visual Studio -> View -> MsBuild Task Explorer.

## Debugging
1. Navigate to solution's properties.
2. Open "Debug" tab.
3. Select "Start external program" and set Visual Studio path:  
`C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe`.
4. Set "Command line arguments" `/rootsuffix Exp`.
