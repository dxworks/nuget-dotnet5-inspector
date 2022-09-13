﻿using Com.Synopsys.Integration.Nuget.Dotnet3.DependencyResolution.Nuget;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Model;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Project;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Solution;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Dispatch
{
    //Given a generic InspectionOptions, InspectorDispatch is responsible for instantiating the correct Inspector (Project or Solution)
    class InspectorDispatch
    {
        public InspectorDispatch()
        {
        }

        public List<InspectionResult> Inspect(InspectionOptions options, NugetSearchService nugetService)
        {
            return CreateInspectors(options, nugetService)?.Select(insp => insp.Inspect()).ToList();
        }

        public List<IInspector> CreateInspectors(InspectionOptions options, NugetSearchService nugetService)
        {
            var inspectors = new List<IInspector>();
            if (Directory.Exists(options.TargetPath))
            {
                Console.WriteLine("Searching for solution files to process...");
                var solutionPaths = Directory.GetFiles(options.TargetPath, "*.sln", SearchOption.AllDirectories)
                    .ToHashSet();

                if (solutionPaths is { Count: >= 1 })
                {
                    foreach (var solution in solutionPaths)
                    {
                        Console.WriteLine("Found Solution {0}", solution);
                        var solutionOp = new SolutionInspectionOptions(options);
                        solutionOp.TargetPath = solution;
                        inspectors.Add(new SolutionInspector(solutionOp, nugetService));
                    }
                }
                else
                {


                    Console.WriteLine("No Solution file found.  Searching for a project file...");
                    string[] projectPaths = SupportedProjectPatterns.AsList
                        .SelectMany(pattern =>
                            Directory.GetFiles(options.TargetPath, pattern, SearchOption.AllDirectories))
                        .Distinct().ToArray();
                    if (projectPaths is {Length: > 0})
                    {
                        foreach (var projectPath in projectPaths)
                        {
                            Console.WriteLine("Found project {0}", projectPath);
                            var projectOp = new ProjectInspectionOptions(options);
                            projectOp.TargetPath = projectPath;
                            inspectors.Add(new ProjectInspector(projectOp, nugetService));
                        }
                    }
                }
            }
            else if (File.Exists(options.TargetPath))
            {
                if (options.TargetPath.Contains(".sln"))
                {
                    var solutionOp = new SolutionInspectionOptions(options);
                    solutionOp.TargetPath = options.TargetPath;
                    inspectors.Add(new SolutionInspector(solutionOp, nugetService));
                }
                else
                {
                    var projectOp = new ProjectInspectionOptions(options);
                    projectOp.TargetPath = options.TargetPath;
                    inspectors.Add(new ProjectInspector(projectOp, nugetService));
                }
            }

            return inspectors;
        }
    }
}
