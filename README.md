# MsBuild Task Explorer
Visual Studio Add-In for developers who would like to execute MsBuild targets inside Visual Studio.
The add-in analyzes the solution folders to find MsBuild project files (.proj) to get targets and then lists it to provide ability to execute them.

![targets](http://i.imgur.com/1BCuNKo.png)

## Possibilities
1. Get solution targets.
2. Search for the concrete target by text pattern.
3. Target execution with log in the Output Window.
4. Print all target properties to the Output Window.

![log](http://i.imgur.com/b2J7mo7.png)

## Getting started
1. Install [MsBuildTaskExplorer](https://marketplace.visualstudio.com/items?itemName=saaseev.MsBuildTaskExplorer).
2. Click Visual Studio -> View -> Other Windows -> MsBuild Task Explorer.

## Debugging
1. Navigate to solution's properties.
2. Open "Debug" tab.
3. Select "Start external program" and set Visual Studio path:  
`C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe`.
4. Set "Command line arguments" `/rootsuffix Exp`.
