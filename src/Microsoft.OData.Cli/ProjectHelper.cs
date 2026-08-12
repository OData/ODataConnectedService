//-----------------------------------------------------------------------------
// <copyright file="ProjectHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Logging;
using NuGet.Versioning;

namespace Microsoft.OData.Cli
{
    internal static class ProjectHelper
    {
        /// <summary>
        /// Creates a project instance from the project path passed
        /// </summary>
        /// <param name="projectPath">A path to the directory with .csproj file</param>
        /// <returns>A <see cref="Project"/> instance</returns>
        internal static Project CreateProjectInstance(string projectPath)
        {
            Project project = null;

            string[] extensions = { ".csproj", ".vbproj" };

            string[] path = Directory.GetFiles(projectPath, "*.*")
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower())).ToArray();
            if (path.Length > 0)
            {
                string pathProject = path[0];
                ProjectCollection buildEngine = new ProjectCollection();
                project = new Project(pathProject);
            }

            return project;
        }

        /// <summary>
        /// Reloads the project to accomodate new changes made to the .csproj file.
        /// </summary>
        /// <param name="projectPath">A path to the directory with .csproj file</param>
        /// <returns>A <see cref="Project"/> instance</returns>
        internal static Project ReloadProject(string projectPath)
        {
            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            Project project = CreateProjectInstance(projectPath);

            return project;
        }

        /// <summary>
        /// Adds a project item to the project
        /// </summary>
        /// <param name="project">An instance of the loaded <see cref="Project"/>/></param>
        /// <param name="item">The project item to add to the project</param>
        /// <param name="value">The value of the project item being added</param>
        internal static void AddProjectItem(Project project, string item, string value)
        {
            if (project != null)
            {
                project.AddItem(item, value);
                project.Save();
            }
        }

        /// <summary>
        /// This method determines whether the .sln file and .csproj file are in the same folder.
        /// This is important in determining where to create the nuget packages folder for a project.
        /// For a project that uses the packages folder structure.
        /// </summary>
        /// <param name="projectPath">A path to the directory with .csproj file</param>
        /// <returns>True or False.</returns>
        internal static bool CheckIfSolutionAndProjectFilesAreInSameFolder(string projectPath)
        {
            string[] projectExtensions = { ".csproj", ".vbproj" };

            string[] checkIfProjectExtensionExists = Directory.GetFiles(projectPath, "*.*")
                .Where(f => projectExtensions.Contains(Path.GetExtension(f).ToLower())).ToArray();

            string[] checkIfSlnExtensionExists = Directory.GetFiles(projectPath, "*.sln");

            if (checkIfProjectExtensionExists.Length > 0 && checkIfSlnExtensionExists.Length > 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// This method gets all the targetframeworks of the provided project. 
        /// Limiting the package installation to only .net framework and .netcore targetframeworks.
        /// </summary>
        /// <param name="project">An instance of the loaded <see cref="Project"/>.</param>
        /// <returns>A string array of the target frameworks for the provided project</returns>
        internal static string[] GetProjectTargetFrameworks(this Project project)
        {
            string[] targetFrameworks = new string[] { };
            string targetFramework = project.GetPropertyValue("TargetFramework");
            if (string.IsNullOrEmpty(targetFramework))
            {
                targetFramework = project.GetPropertyValue("TargetFrameworkVersion");
            }

            if (string.IsNullOrEmpty(targetFramework) || targetFramework.Equals("v4.0"))
            {
                string multipleTargetFrameworks = project.GetPropertyValue("TargetFrameworks");
                if (!string.IsNullOrEmpty(multipleTargetFrameworks))
                {
                    IEnumerable<string> frameworks = multipleTargetFrameworks.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim());
                    List<string> validFrameworks = new List<string>();
                    foreach (string framework in frameworks)
                    {
                        if (framework.StartsWith("net4"))
                        {
                            validFrameworks.Add(".NETFramework,Version="+framework);
                        }
                        else if (!framework.StartsWith("netstandard"))
                        {
                            validFrameworks.Add(".NETCore,Version="+framework);
                        }
                    }

                    targetFrameworks = validFrameworks.ToArray();
                }
            }
            else
            {
                if (targetFramework.StartsWith("v4"))
                {
                    targetFrameworks = new[] { ".NETFramework,Version="+targetFramework.Replace("v", "net") };
                }
                else
                {
                    targetFrameworks = new[] { ".NETCore,Version="+targetFramework};
                }
            }

            return targetFrameworks;
        }

        /// <summary>
        /// Checks if the Microsoft.OData.Client package version in the project is at least 9.0.0.
        /// </summary>
        /// <param name="project">An instance of the loaded <see cref="Project"/>.</param>
        /// <returns>True if the Microsoft.OData.Client version is at least 9.0.0; otherwise, false.</returns>
        internal static async Task<bool> CheckODataClientVersionAsync(this Project project, IMessageLogger logger)
        {
            if (project == null)
            {
                return false;
            }

            ProjectItem packageReference = project.GetItems("PackageReference")
                .FirstOrDefault(pr => pr.EvaluatedInclude.Equals(Constants.V4ClientNuGetPackage, StringComparison.OrdinalIgnoreCase));

            if (packageReference == null)
            {
                return false;
            }

            if (TryGetODataClientVersion(project, packageReference, out Version version))
            {
                return ODataClientVersionChecker.SupportsNativeDateTimeTypes(version);
            }

            if (logger != null)
            {
                await logger.WriteMessageAsync(
                    LogMessageCategory.Warning,
                    "Microsoft.OData.Client is referenced, but its version could not be resolved. Legacy date and time types will be generated.")
                    .ConfigureAwait(false);
            }

            return false;
        }

        private static bool TryGetODataClientVersion(Project project, ProjectItem packageReference, out Version version)
        {
            if (TryGetVersionFromAssetsFile(project, out version))
            {
                return true;
            }

            if (TryParseVersionExpression(packageReference.GetMetadataValue("VersionOverride"), out version))
            {
                return true;
            }

            ProjectItem packageVersion = project.GetItems("PackageVersion")
                .FirstOrDefault(item => item.EvaluatedInclude.Equals(Constants.V4ClientNuGetPackage, StringComparison.OrdinalIgnoreCase));
            if (packageVersion != null && TryParseVersionExpression(packageVersion.GetMetadataValue("Version"), out version))
            {
                return true;
            }

            return TryParseVersionExpression(packageReference.GetMetadataValue("Version"), out version);
        }

        private static bool TryGetVersionFromAssetsFile(Project project, out Version version)
        {
            version = null;
            string assetsFile = project.GetPropertyValue("ProjectAssetsFile");
            if (string.IsNullOrWhiteSpace(assetsFile))
            {
                assetsFile = Path.Combine(project.DirectoryPath, "obj", "project.assets.json");
            }

            if (!File.Exists(assetsFile))
            {
                return false;
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(assetsFile)))
                {
                    if (!document.RootElement.TryGetProperty("libraries", out JsonElement libraries))
                    {
                        return false;
                    }

                    foreach (JsonProperty library in libraries.EnumerateObject())
                    {
                        int separatorIndex = library.Name.LastIndexOf('/');
                        if (separatorIndex <= 0 ||
                            !library.Name.Substring(0, separatorIndex).Equals(Constants.V4ClientNuGetPackage, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        return TryParseVersionExpression(library.Name.Substring(separatorIndex + 1), out version);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static bool TryParseVersionExpression(string versionExpression, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return false;
            }

            if (NuGetVersion.TryParse(versionExpression, out NuGetVersion nuGetVersion))
            {
                version = nuGetVersion.Version;
                return true;
            }

            if (VersionRange.TryParse(versionExpression, out VersionRange versionRange) && versionRange.MinVersion != null)
            {
                version = versionRange.MinVersion.Version;
                return true;
            }

            return false;
        }
    }
}
