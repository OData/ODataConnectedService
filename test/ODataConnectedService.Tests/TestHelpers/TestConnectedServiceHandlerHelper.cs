//-----------------------------------------------------------------------------------
// <copyright file="TestConnectedServiceHandlerHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Tests.TestHelpers
{
    class TestConnectedServiceHandlerHelper : ConnectedServiceHandlerHelper
    {
        public TestConnectedServiceHandlerHelper() : base()
        {
            AddedFiles = new List<(string CreatedFile, string SourceFile)>();
        }

        public IList<(string CreatedFile, string SourceFile)> AddedFiles { get; private set; }
        // used to access the temp file that the generated code was written to
        public string AddedFileInputFileName { get; private set; }
        // used to find out which file the final output would be written to
        public string AddedFileTargetFilePath { get; private set; }
        public string ServicesRootFolder { get; set; }
        public override Task<string> AddFileAsync(string fileName, string targetPath, AddFileOptions addFileOptions = null)
        {
            AddedFileInputFileName = fileName;
            AddedFileTargetFilePath = targetPath;
            AddedFiles.Add((targetPath, fileName));
            return Task.FromResult(string.Empty);
        }
        public override IDictionary<string, string> TokenReplacementValues { get; }
        public override void AddAssemblyReference(string assemblyPath) =>
            throw new System.NotImplementedException();
        public override string GetServiceArtifactsRootFolder() => ServicesRootFolder;
        public override string PerformTokenReplacement(string input, IDictionary<string, string> additionalReplacementValues = null) =>
            throw new System.NotImplementedException();
    }
}
