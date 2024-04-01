using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Pack);

    readonly List<PlatformItem> Platforms = [];
    
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter("Output path")]
    readonly AbsolutePath OutputPath = RootDirectory / "out";
    
    [Parameter("Artifacts path")]
    readonly AbsolutePath ArtifactsPath = RootDirectory / "artifacts";
    
    [Parameter("Allow desktop build")]
    readonly bool AllowDesktopBuild = true;
    
    [Parameter("Allow android build")]
    readonly bool AllowAndroidBuild;
    
    [Parameter("Allow iOS build")]
    readonly bool AllowiOSBuild;
    
    [Solution] readonly Solution Solution;
    
    [GitRepository]
    readonly GitRepository GitRepository;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            OutputPath.DeleteDirectory();
            ArtifactsPath.DeleteDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (AllowDesktopBuild)
            {
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
            }
            
            if (AllowAndroidBuild)
            {
                Platforms.Add(new PlatformItem("NoteSHR.Android", "android", null));
            }
            
            if (AllowiOSBuild && IsOsx)
            {
                Platforms.Add(new PlatformItem("NoteSHR.iOS", "ios", null));
            }
            
            var projects = Solution.AllProjects.Where(x => Platforms.Select(p => p.Name).Contains(x.Name));
            
            foreach (var (project, projectDetail) in projects.Join(Platforms, p => p.Name, pD => pD.Name, (p, pD) => (p, pD)))
            {
                if (projectDetail.Architectures == null)
                {
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework("net8.0")
                        .SetOutput($"{OutputPath}/{project.Name}/{projectDetail.Platform}"));
                    continue;
                }
                
                foreach (var architecture in projectDetail.Architectures)
                {
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework("net8.0")
                        .SetRuntime($"{projectDetail.Platform}-{architecture}")
                        .SetOutput($"{OutputPath}/{project.Name}/{projectDetail.Platform}-{architecture}"));
                }
            }
        });
    
    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            foreach (var folder in OutputPath.GetDirectories())
            {
                foreach (var platformBuildFolder in folder.GetDirectories())
                {
                    platformBuildFolder.ZipTo(ArtifactsPath / $"{platformBuildFolder.Name}-{GitRepository.Branch}-{GitRepository.Commit}.zip",
                        compressionLevel: CompressionLevel.SmallestSize,
                        fileMode: FileMode.CreateNew);
                }
            }
        });
}
