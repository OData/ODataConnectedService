//-----------------------------------------------------------------------------
// <copyright file="GenerateOptions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Cli
{
    /// <summary>
    /// The Generate Command options
    /// </summary>
    public class GenerateOptions
    {
        /// <summary>
        /// The URI of the metadata document. The value must be set to a valid service document URI or a local file path
        /// </summary>
        public string MetadataUri { get; set; }

        /// <summary>
        /// Headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue.
        /// </summary>
        public string CustomHeaders { get; set; }

        /// <summary>
        /// Proxy settings. Format: domain\\user:password@SERVER:PORT.
        /// </summary>
        public string Proxy { get; set; }

        /// <summary>
        /// The name of the generated file name 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The namespace of the client code generated. 
        /// Example: ODataCliCodeGeneratorSample.NorthWindModel or ODataCliCodeGeneratorSample or it could be a name related to the OData endpoint.
        /// </summary>
        public string NamespacePrefix { get; set; }

        /// <summary>
        /// Disables/Enables upper camel casing
        /// </summary>
        public bool UpperCamelCase { get; set; }

        /// <summary>
        /// Apply the "internal" class modifier on generated classes instead of "public" thereby making them invisible outside the assembly.
        /// </summary>
        public bool EnableInternal { get; set; }

        /// <summary>
        /// Split the generated classes into separate files instead of generating all the code in a single file.
        /// </summary>
        public bool MultipleFiles { get; set; }

        /// <summary>
        /// Comma-separated list of the names of operation imports to exclude from the generated code. Example: ExcludedOperationImport1,ExcludedOperationImport2.
        /// </summary>
        public List<string> ExcludedOperationImports { get; set; }

        /// <summary>
        /// Comma-separated list of the names of bound operations to exclude from the generated code.Example: BoundOperation1,BoundOperation2.
        /// </summary>
        public List<string> ExcludedBoundOperations { get; set; }

        /// <summary>
        /// Comma-separated list of the names of entity types to exclude from the generated code.Example: EntityType1,EntityType2,EntityType3.
        /// </summary>
        public List<string> ExcludedSchemaTypes { get; set; }

        /// <summary>
        /// This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any.
        /// </summary>
        public bool IgnoreUnexpectedElements { get; set; }

        /// <summary>
        /// Full path to output directory
        /// </summary>
        public string OutputDir { get; set; }

        /// <summary>
        /// Web proxy network credentials username
        /// </summary>
        public string WebProxyNetworkCredentialsUsername { get; set; }

        /// <summary>
        /// Web proxy network credentials password
        /// </summary>
        public string WebProxyNetworkCredentialsPassword { get; set; }

        /// <summary>
        /// Web proxy network credentials domain
        /// </summary>
        public string WebProxyNetworkCredentialsDomain { get; set; }

        /// <summary>
        /// A flag to indicate whether to include the web proxy or not
        /// </summary>
        public bool IncludeWebProxy { get; set; }

        /// <summary>
        /// A web proxy host
        /// </summary>
        public string WebProxyHost { get; set; }

        /// <summary>
        /// A flag to indicate whether to include web proxy network credentials or not.
        /// </summary>
        public bool IncludeWebProxyNetworkCredentials { get; set; }

    }
}
