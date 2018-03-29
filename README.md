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

### Optional. Set your custom extension for search targets.
1. Click Visual Studio -> Tool -> MsBuild Task Explorer setting
2. Set your extension by mask *.*extension

### Default extension
*.*proj and *.targets
