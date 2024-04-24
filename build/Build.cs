using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using static Nuke.Common.EnvironmentInfo;

class Build : NukeBuild
{
    [Parameter("Allow android build")] readonly bool AllowAndroidBuild;

    [Parameter("Allow desktop build")] readonly bool AllowDesktopBuild = true;

    [Parameter("Allow iOS build")] readonly bool AllowIOSBuild;
    
    [Parameter("Allow browser build")] readonly bool AllowBrowserBuild;

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

            if (AllowAndroidBuild) Platforms.Add(new PlatformItem("NoteSHR.Android", "android", null));

            if (AllowIOSBuild && IsOsx) Platforms.Add(new PlatformItem("NoteSHR.iOS", "ios", null));
            if (AllowBrowserBuild) Platforms.Add(new PlatformItem("NoteSHR.Browser", string.Empty, null));

            var projects = Solution.AllProjects.Where(x => Platforms.Select(p => p.Name).Contains(x.Name));

            foreach (var (project, projectDetail) in projects.Join(Platforms, p => p.Name, pD => pD.Name,
                         (p, pD) => (p, pD)))
            {
                if (projectDetail.Architectures == null)
                {
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework($"net8.0{(projectDetail.Platform.IsNullOrEmpty() ? string.Empty : "-")}{projectDetail.Platform}")
                        .SetOutput($"{OutputPath}/{project.Name}"));
                    continue;
                }

                foreach (var architecture in projectDetail.Architectures)
                    DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                        .SetConfiguration(Configuration)
                        .SetProject(project)
                        .SetFramework("net8.0")
                        .SetRuntime($"{projectDetail.Platform}-{architecture}")
                        .SetOutput($"{OutputPath}/{project.Name}/{projectDetail.Platform}-{architecture}"));
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

    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Pack);
}