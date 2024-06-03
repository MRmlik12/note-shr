using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Serilog;
using static Nuke.Common.EnvironmentInfo;

[GitHubActions(
    "build",
    GitHubActionsImage.WindowsLatest, 
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.Push],
    AutoGenerate = false)]
    //InvokedTargets = [nameof(PublishBuilds)])]
class Build : NukeBuild
{
    [Parameter("Allow android build")] readonly bool AllowAndroidBuild;

    [Parameter("Allow desktop build")] readonly bool AllowDesktopBuild = true;

    [Parameter("Allow iOS build")] readonly bool AllowIOSBuild;
    
    [Parameter("Allow browser build")] readonly bool AllowBrowserBuild;

    [Parameter("Skip tests")] readonly bool SkipTests;

    [Parameter("Artifacts path")] readonly AbsolutePath ArtifactsPath = RootDirectory / "artifacts";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;

    [Parameter("Output path")] readonly AbsolutePath OutputPath = RootDirectory / "out";

    readonly List<PlatformItem> Platforms = [];

    [Solution] readonly Solution Solution;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            OutputPath.DeleteDirectory();
            ArtifactsPath.DeleteDirectory();
        });
    
    Target Setup => _ => _
        .Executes(() =>
        {
            if (AllowDesktopBuild)
                switch (Platform)
                {
                    case PlatformFamily.Windows:
                        Platforms.Add(new PlatformItem("NoteSHR.Desktop", "win", ["x64"]));
                        break;
                    case PlatformFamily.OSX:
                        Platforms.Add(new PlatformItem("NoteSHR.Desktop", "osx", ["x64", "arm64"]));
                        break;
                    case PlatformFamily.Linux:
                        Platforms.Add(new PlatformItem("NoteSHR.Desktop", "linux", ["x64", "arm64"]));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            if (AllowAndroidBuild) 
                Platforms.Add(new PlatformItem("NoteSHR.Android", "android", null));

            if (AllowIOSBuild && IsOsx) 
                Platforms.Add(new PlatformItem("NoteSHR.iOS", "ios", null));
            
            if (AllowBrowserBuild)
                Platforms.Add(new PlatformItem("NoteSHR.Browser", string.Empty, null));

            Log.Information("Projects to build: {Platforms}", Platforms.Select(x => $"{x.Name} [{x.Architectures?.Join("; ")}]").Join(", "));
        });

    Target Restore => _ => _
        .DependsOn(Setup)
        .Executes(() =>
        {
            if (AllowAndroidBuild)
                DotNetTasks.DotNetWorkloadInstall(s => s
                    .SetWorkloadId("android")
                    .SetSkipManifestUpdate(true));
           
            if (AllowIOSBuild)
                DotNetTasks.DotNetWorkloadInstall(s => s
                    .SetWorkloadId("ios")
                    .SetSkipManifestUpdate(true));
            
            if (AllowBrowserBuild)
                DotNetTasks.DotNetWorkloadInstall(s => s
                    .SetWorkloadId("wasm-tools")
                    .SetSkipManifestUpdate(true));
            
            foreach (var platform in Platforms)
            {
                DotNetTasks.DotNetRestore(_ => _.SetProjectFile(Solution.AllProjects.Single(x => x.Name == platform.Name).Path));
            }
        });
    
    Target CompileBrowser => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        { 
            Log.Information("{Path}",Solution.AllProjects.Single(x => x.Name == "NoteSHR.Browser").Path);
            DotNetTasks.DotNetBuild(_ => _.EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution.AllProjects.Single(x => x.Name == "NoteSHR.Browser").Path));
        });

    Target Test => _ => _
        .DependsOn(Restore)
        .OnlyWhenDynamic(() => !SkipTests)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ =>
                _.SetProjectFile(Solution.AllProjects.Single(x => x.Name == "NoteSHR.File.Tests").Path));
            DotNetTasks.DotNetTest(_ => _.EnableNoRestore());
        });
    
    Target Compile => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            var projects = Solution.AllProjects.Where(x => Platforms.Select(p => p.Name).Contains(x.Name));

            foreach (var (project, projectDetail) in projects.Join(Platforms, p => p.Name, pD => pD.Name,
                         (p, pD) => (p, pD)))
            {
                var outputPath = $"{OutputPath}/{project.Name}";
                if (projectDetail.Architectures == null)
                {
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework($"net8.0{(projectDetail.Platform.IsNullOrEmpty() ? string.Empty : "-")}{projectDetail.Platform}")
                        .SetOutput(outputPath));
                    
                    continue;
                }

                foreach (var architecture in projectDetail.Architectures)
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework("net8.0")
                        .SetRuntime($"{projectDetail.Platform}-{architecture}")
                        .SetOutput($"{outputPath}/{projectDetail.Platform}-{architecture}"));
            }
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (!Directory.Exists(ArtifactsPath.Name)) ArtifactsPath.CreateDirectory();

            foreach (var folder in OutputPath.GetDirectories())
            {
                if (folder.Name == "NoteSHR.Android")
                {
                    foreach (var platformBuildFolder in folder.GetFiles().Where(x => x.Name.Contains("-Signed.apk")))
                        platformBuildFolder.MoveToDirectory(RootDirectory / "artifacts");
                    continue;
                }

                foreach (var platformBuildFolder in folder.GetDirectories())
                    platformBuildFolder.ZipTo(
                        ArtifactsPath / $"{platformBuildFolder.Name}-{GitRepository.Branch}-{GitRepository.Commit}.zip",
                        compressionLevel: CompressionLevel.SmallestSize,
                        fileMode: FileMode.CreateNew);
            }
        });

    // Target PublishBuilds => _ => _
    //     .DependsOn(Pack)
    //     .Produces(ArtifactsPath / "*.zip")
    //     .Executes(() =>
    //     {
    //
    //     });
    
    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Pack);
}