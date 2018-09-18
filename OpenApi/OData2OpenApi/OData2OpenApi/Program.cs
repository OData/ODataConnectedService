// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            // just for test
            if (args.Length == 0)
            {
                // args = new[] { @"--csdl=E:\github\microsoft\OpenAPI.NET.OData\docs\csdl\graph1.0.xml", @"--output=E:\work\OpenApi\output\Graph1.0.json" };
            }

            Configuration config = ParseArguments(args);
            if (config == null)
            {
                return 1;
            }

            if (OpenApiGenerator.Run(config))
            {
                Console.WriteLine("Passed!\nExit Code: 0");
                return 0;
            }
            else
            {
                Console.WriteLine("Failed!");
                return 1;
            }
        }

        private static Configuration ParseArguments(string[] args)
        {
            PrintCopyright();

            if (args.Length == 0)
            {
                PrintUsage(CommandLineParser.Options);
                Environment.Exit(0);
            }

            Configuration config = null;
            try
            {
                config = CommandLineParser.Parse(args);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                PrintUsage(CommandLineParser.Options);
                Environment.Exit(1);
            }

            try
            {
                config.InitializeAndValidate();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                Environment.Exit(1);
            }

            return config;
        }

        private static void WriteException(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(exception.Message);
            Console.ResetColor();
        }

        private static void PrintCopyright()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Microsoft (R) OData To Open API Utilities, Version { version } \n" +
                "Copyright(C) Microsoft Corporation.  All rights reserved.\n");
        }

        private static void PrintUsage(IEnumerable<CommandOptionAttribute> options)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Usage: OData2OpenApi.exe [options]\n");
            sb.Append("\nOptions:\n");
            foreach (var option in options)
            {
                sb.Append("\t").Append(option.Raw).Append("\n");
            }
            sb.Append("\nExamples:\n");
            sb.Append("    OData2OpenApi.exe --csld=http://services.odata.org/TrippinRESTierService --output=trip.yaml\n");
            sb.Append("    OData2OpenApi.exe --csdl=c:\\csdl.xml --output=trip.json\n");
            Console.WriteLine(sb.ToString());
        }
    }
}
