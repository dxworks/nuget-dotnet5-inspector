using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Synopsys.Integration.Nuget.Dotnet3.DependencyResolution
{
    interface DependencyResolver
    {
        DependencyResult Process();
    }
}
