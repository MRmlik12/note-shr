![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![GitHub repo size](https://img.shields.io/github/repo-size/MRmlik12/note-shr?style=for-the-badge)
![GitHub](https://img.shields.io/github/license/MRmlik12/note-shr?style=for-the-badge)
# NoteSHR

A multi-platform application to effectively manage notes on a whiteboard.

## Goals

For more detailed plan of app development please look into [TODO board](https://dolczyk.notion.site/TODO-68c8d6e46fbe4519b3fb762d7469b6e6?pvs=4)

- [ ]  Proof of concept
- [ ]  Save file scheme
- [ ]  Beautify app
- [ ]  Share board with everyone
- [ ]  Collaboration API
- [ ]  Keep responsiveness and stability for web, desktop and mobile platforms

## Build/Run process

To run this project you must install .NET 8 with workloads such as `android` , `iOS` and `wasm-tools`

### Install dependencies

```bash
$ dotnet restore
# Install workloads
$ dotnet workload install android
$ dotnet workload install iOS
$ dotnet workload install wasm-to
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

### Build

```bash
$ dotnet restore
# For desktop platforms
$ dotnet build --project src/NoteSHR.Desktop
# For iOS platform
$ dotnet build --project src/NoteSHR.iOS
# For Android platform
$ dotnet build --project src/NoteSHR.Android
# For Web platform
$ dotnet build --project src/NoteSHR.Browser
```

## Used libraries

- [Avalonia](https://avaloniaui.net/)
- [ReactiveUI](https://www.reactiveui.net/)