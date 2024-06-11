![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![GitHub repo size](https://img.shields.io/github/repo-size/MRmlik12/note-shr?style=for-the-badge)
![GitHub Repo Start](https://img.shields.io/github/stars/MRmlik12/note-shr?style=for-the-badge)
![GitHub](https://img.shields.io/github/license/MRmlik12/note-shr?style=for-the-badge)
![GitHub Actions Workflow Build Status](https://img.shields.io/github/actions/workflow/status/MRmlik12/note-shr/build.yml?style=for-the-badge)
![GitHub Actions Workflow Test Status](https://img.shields.io/github/actions/workflow/status/MRmlik12/note-shr/test.yml?style=for-the-badge&label=Test)

# NoteSHR

<div style="margin-bottom: 0.5rem;">
    <img src="images/preview.png" alt="drawing" style="width:800px;"/>
</div>

A cross-platform application to create own knowledge database, writing own thoughts, ideas and organize information in a creative way.

## Development plan

### Goals

For more detailed plan of app development please look into [TODO board](https://dolczyk.notion.site/TODO-68c8d6e46fbe4519b3fb762d7469b6e6?pvs=4)

- [x]  Proof of concept
- [x]  Import and export board
- [ ]  Beautify app
- [ ]  Zoom in/out board
- [ ]  Share board with everyone
- [ ]  Collaboration API
- [ ]  Keep responsiveness and stability for web, desktop and mobile platforms

### Implemented features

- [x] Dragging and resizing notes across the board
- [x] Adding nodes to note
- [x] Edit and remove mode for note nodes
- [x] Importing and exporting board
- [x] Components in node: Text, List, Check and Image
- [x] Fully working web and desktop version

## Try it out

* [Web version](https://note-shr.dolczyk.rocks)
* Desktop platforms (Windows, MacOS, Linux). Pick the latest build action from [here](https://github.com/MRmlik12/note-shr/actions/workflows/build.yml) (Requires .NET 8 runtime to run)
* Mobile platforms (Android, iOS) not ready yet

## Build/Run process

To run this project you must install .NET 8 with workloads such as `android` , `iOS` and `wasm-tools`

### Install dependencies

```bash
$ dotnet restore
# Install workloads
$ dotnet workload install android
$ dotnet workload install iOS
$ dotnet workload install wasm-tools
```

### Run project

```bash
# Run a Desktop platform 
$ dotnet run --project src/NoteSHR.Desktop
# For iOS platform
$ dotnet run --project src/NoteSHR.iOS
# For Android platform
$ dotnet run --project src/NoteSHR.Android
# For Web platform
$ dotnet run --project src/NoteSHR.Browser
```

### Run tests

#### Using Bash
```bash
$ ./build.sh Test
```

#### Using PowerShell
```powershell
> .\build.ps1 Test
```

### Build

#### Using Bash

```bash
# For desktop platforms
$ ./build.sh --configuration Release
# For iOS platform
$ ./build.sh --configuration Release --allow-ios-build
# For Android platform
$ ./build.sh --configuration Release --allow-android-build
```

#### Using PowerShell

```powershell
# For desktop platforms 
> .\build.ps1 
# For iOS platform
> .\build.ps1 --allow-ios-build
# For Android platform
> .\build.ps1 --allow-android-build
```

## Used libraries

- [Avalonia](https://avaloniaui.net/)
- [ReactiveUI](https://www.reactiveui.net/)
- [Avalonia.Xaml.Behaviors](https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors)
- [PanAndZoom](https://github.com/wieslawsoltes/PanAndZoom)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia)
- [NUKE](https://nuke.build/)
