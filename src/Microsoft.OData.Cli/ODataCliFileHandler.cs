//-----------------------------------------------------------------------------
// <copyright file="ODataCliFileHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;

namespace Microsoft.OData.Cli
{
    /// <summary>
    /// An implementation of the <see cref="IFileHandler"/>
    /// </summary>
    public class ODataCliFileHandler : IFileHandler
    {
        private readonly IMessageLogger logger;
        private Project project;

        /// <summary>
        /// Creates an instance of <see cref="ODataCliFileHandler"/>
        /// </summary>
        /// <param name="logger">A <see cref="IMessageLogger"/> to use for logging</param>
        /// <param name="project">A <see cref="Project"/> instance </param>
        public ODataCliFileHandler(IMessageLogger logger, Project project)
        {
            this.logger = logger;
            this.project = project;
        }

        /// <summary>
        /// Copies a file to a target path.
        /// </summary>
        /// <param name="fileName">The name of the source file to copy.</param>
        /// <param name="targetPath">The target path to copy the file to.</param>
        /// <param name="oDataFileOptions">The options to use when adding a file to a target path.</param>
        /// <returns>Returns the path to the file that was added</returns>
        public Task<string> AddFileAsync(string fileName, string targetPath, ODataFileOptions oDataFileOptions = null)
        {
            if (!File.Exists(fileName))
            {
                return Task.FromException<string>(new FileNotFoundException("The filename provided does not exist"));
            }

            try
            {
                File.Copy(fileName, targetPath, true);

                if (this.project != null)
                {
                    this.project = ProjectHelper.ReloadProject(this.project.DirectoryPath);
                    string[] projectTargetFrameworks = this.project.GetProjectTargetFrameworks();

                    foreach (string projectTargetFramework in projectTargetFrameworks)
                    {
                        if (projectTargetFramework.Contains("net4"))
                        {
                            //If the filename being copied to the project folder is not 'Csdl.xml' and it has not already been copied,
                            //then add it to the project file. 
                            //Example of what the if statement below will output if true:
                            //<ItemGroup>
                            //    <Compile Include="Reference.cs" />
                            //</ItemGroup>
                            if (!Path.GetFileName(targetPath).Equals(Constants.DefaultServiceName+"Csdl.xml") && project.GetItemsByEvaluatedInclude(Path.GetFileName(targetPath)).Count == 0)
                            {
                                ProjectHelper.AddProjectItem(this.project, "Compile", Path.GetFileName(targetPath));
                            }
                        }
                    }
                }

                string path = Path.GetFullPath(fileName);

                return Task.FromResult(path);
            }
            catch (Exception e)
            {
                return Task.FromException<string>(new Exception(e.Message));
            }

        }

        /// <summary>
        /// Emits the container property attribute. The lastest version gets installed. so we'll always emit this property.
        /// </summary>
        /// <returns>A bool indicating whether to emit the container property or not</returns>
        public bool EmitContainerPropertyAttribute()
        {
            return true;
        }

        /// <summary>
        /// Sets the CSDL file as an embedded resource
        /// </summary>
        /// <param name="fileName">The name of the file to set as embedded resource</param>
        public void SetFileAsEmbeddedResource(string fileName)
        {
            if (this.project != null)
            {
                this.project = ProjectHelper.ReloadProject(this.project.DirectoryPath);

                if (this.project.GetItems("EmbeddedResource").Count == 0)
                {
                    ProjectHelper.AddProjectItem(this.project, "EmbeddedResource", fileName);
                }
            }
        }
    }
}
