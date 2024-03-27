using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
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

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter("Output path")]
    readonly AbsolutePath OutputPath = RootDirectory / "out";
    
    [Parameter("Allow desktop build")]
    readonly bool AllowDesktopBuild = true;
    
    [Parameter("Allow android build")]
    readonly bool AllowAndroidBuild;
    
    [Parameter("Allow iOS build")]
    readonly bool AllowiOSBuild;
    
    [Solution] readonly Solution Solution;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            OutputPath.DeleteDirectory();
            DotNetTasks.DotNetClean();
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
            var projectNames = new List<string>();

            if (AllowDesktopBuild)
            {
                projectNames.Add("NoteSHR.Desktop");
            }
            
            if (AllowAndroidBuild)
            {
                projectNames.Add("NoteSHR.Android");
            }
            
            if (AllowiOSBuild && IsOsx)
            {
                projectNames.Add("NoteSHR.iOS");
            }

            var projects = Solution.AllProjects.Where(x => projectNames.Contains(x.Name));
            
            foreach (var project in projects)
            {
                DotNetTasks.DotNetPublish(_ => _.EnableNoRestore()
                    .SetConfiguration(Configuration)
                    .SetProject(project)
                    .SetOutput($"{OutputPath}/{project.Name}"));
            }
        });
}
