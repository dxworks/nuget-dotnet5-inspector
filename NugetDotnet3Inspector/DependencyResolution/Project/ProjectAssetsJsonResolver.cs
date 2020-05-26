using Com.Synopsys.Integration.Nuget.Dotnet3.DependencyResolution.Nuget;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Synopsys.Integration.Nuget.Dotnet3.DependencyResolution.Project
{
    class ProjectAssetsJsonResolver : DependencyResolver
    {
        private string ProjectAssetsJsonPath;

        public ProjectAssetsJsonResolver(string projectAssetsJsonPath)
        {
            ProjectAssetsJsonPath = projectAssetsJsonPath;
        }

        public DependencyResult Process()
        {

            NuGet.ProjectModel.LockFile lockFile = NuGet.ProjectModel.LockFileUtilities.GetLockFile(ProjectAssetsJsonPath, null);

            var resolver = new NugetLockFileResolver(lockFile);

            return resolver.Process();
        }

    }
}
