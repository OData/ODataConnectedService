using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;

namespace Microsoft.OData.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            RegisterMsBuild();
            GenerateCommand generateCommand = new GenerateCommand();
            RootCommand app = new RootCommand {
                generateCommand
            };
            await app.InvokeAsync(args);
        }

        /// <summary>
        /// Tries to register MSBuild from Visual Studio install folder. If not available, register defaults.
        /// </summary>
        private static void RegisterMsBuild()
        {
            string pathToMsBuildExeInLatestVisualStudioVersion = string.Empty;
            const string defaultInstallDirOfVisualStudio = @"C:\Program Files\Microsoft Visual Studio\";
            if (Directory.Exists(defaultInstallDirOfVisualStudio))
            {
                var installDirOfLatestVisualStudio = Directory.GetDirectories(defaultInstallDirOfVisualStudio, "????", SearchOption.TopDirectoryOnly)
                    .Where(x => Path.GetFileName(x).All(char.IsDigit))
                    .MaxBy(x => Path.GetFileName(x));

                pathToMsBuildExeInLatestVisualStudioVersion = Path.Combine(
                    Directory.GetDirectories(installDirOfLatestVisualStudio, "*", SearchOption.TopDirectoryOnly).FirstOrDefault() ?? string.Empty,
                    "MSBuild", "Current", "Bin", "MSBuild.exe");
            }

            if (File.Exists(pathToMsBuildExeInLatestVisualStudioVersion))
            {
                MSBuildLocator.RegisterMSBuildPath(Path.GetDirectoryName(pathToMsBuildExeInLatestVisualStudioVersion));
            }
            else
            {
                MSBuildLocator.RegisterDefaults();
            }
        }
    }
}
