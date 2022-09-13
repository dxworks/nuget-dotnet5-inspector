using System;
using System.IO;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Model;
using Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Util;
using Com.Synopsys.Integration.Nuget.Dotnet3.Model;
using Newtonsoft.Json;

namespace Com.Synopsys.Integration.Nuget.Dotnet3.Inspection.Writer
{
    internal class InspectionResultJsonWriter
    {
        private readonly InspectionOutput InspectionOutput;
        private readonly InspectionResult Result;

        public InspectionResultJsonWriter(InspectionResult result)
        {
            Result = result;
            InspectionOutput = new InspectionOutput();
            InspectionOutput.Containers = result.Containers;
        }

        public string FilePath()
        {
            return PathUtil.Combine(Result.OutputDirectory, Result.ResultName + "_inspection.json");
        }

        private static bool IsFolderPath(string fileOrFolderPath)
        {
            return !Path.HasExtension(fileOrFolderPath);
        }

        public void Write()
        {
            Write(Result.OutputDirectory);
        }

        public void Write(string outputDirectory)
        {
            if (IsFolderPath(Result.OutputDirectory))
                Write(outputDirectory, FilePath());
            else
                Write(GetDirectoryFromFilePath(outputDirectory), outputDirectory);
        }

        private static string GetDirectoryFromFilePath(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            return directoryName;
        }

        public void Write(string outputDirectory, string outputFilePath)
        {
            if (outputDirectory == null)
            {
                Console.WriteLine("Could not create output directory: " + outputDirectory);
            }
            else
            {
                Console.WriteLine("Creating output directory: " + outputDirectory);
                Directory.CreateDirectory(outputDirectory);
            }

            Console.WriteLine("Creating output file path: " + outputFilePath);
            using (var fs = new FileStream(outputFilePath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    var writer = new JsonTextWriter(sw);
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, InspectionOutput);
                }
            }
        }
    }
}