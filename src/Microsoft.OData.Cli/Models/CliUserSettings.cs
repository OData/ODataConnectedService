//-----------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.OData.Cli.Models
{
    /// <summary>
    /// Represents CLI options in OData Connected Service Config File<br />
    /// See <seealso cref="Microsoft.OData.CodeGen.Models.UserSettings"/><br />
    /// in ODataConnectedService.Shared/Models/UserSettings.cs<br />
    /// Class created to avoid importing all of <see cref="ODataConnectedService.Shared"/>
    /// </summary>
    public class CliUserSettings
    {
        /// <summary>
        /// Human readable display name of the service instance.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// The URI of the metadata document. The value must be set to a valid service document URI or a local file path
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The name of the generated file name, defaults to Reference .vb/.cs
        /// </summary>
        public string GeneratedFileNamePrefix { get; set; }

        /// <summary>
        /// True if <see cref="NamespacePrefix"/> is used<br />
        /// This member appears in config files but is not used
        /// </summary>
        public bool UseNamespacePrefix { get; set; }

        /// <summary>
        /// The namespace of the client code generated. 
        /// Example: ODataCliCodeGeneratorSample.NorthWindModel or ODataCliCodeGeneratorSample or it could be a name related to the OData endpoint.
        /// </summary>
        public string NamespacePrefix { get; set; }

        /// <summary>
        /// Enables entity and property tracking
        /// </summary>
        public bool UseDataServiceCollection { get; set; }

        /// <summary>
        /// Apply the "internal" class modifier on generated classes instead of "public" thereby making them invisible outside the assembly.
        /// </summary>
        public bool MakeTypesInternal { get; set; }

        /// <summary>
        /// Split the generated classes into separate files instead of generating all the code in a single file.
        /// </summary>
        public bool GenerateMultipleFiles { get; set; }

        /// <summary>
        /// Headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue.
        /// </summary>
        [IgnoreDataMember] // Do not serialize - may contain authentication tokens
        public string CustomHttpHeaders { get; set; }

        /// <summary>
        /// Disables/Enables upper camel casing
        /// </summary>
        public bool EnableNamingAlias { get; set; }

        /// <summary>
        /// This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any.
        /// </summary>
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }

        /// <summary>
        /// This flag indicates that T4 file(s) should be generated and included
        /// </summary>
        public bool IncludeT4File { get; set; }

        /// <summary>
        /// A flag to indicate whether to include the web proxy or not
        /// </summary>
        public bool IncludeWebProxy { get; set; }

        /// <summary>
        /// This flag indicates that custom http headers should be stored
        /// </summary>
        public bool StoreCustomHttpHeaders { get; set; }

        /// <summary>
        /// A web proxy host
        /// </summary>
        public string WebProxyHost { get; set; }

        /// <summary>
        /// A flag to indicate whether to include web proxy network credentials or not.
        /// </summary>
        public bool IncludeWebProxyNetworkCredentials { get; set; }

        /// <summary>
        /// A flag to indicate whether to store web proxy network credentials or not<br />
        /// This member appears in config files but is not used
        /// </summary>
        public bool StoreWebProxyNetworkCredentials { get; set; }

        /// <summary>
        /// Web proxy network credentials username
        /// </summary>
        [IgnoreDataMember] // Do not serialize - security consideration
        public string WebProxyNetworkCredentialsUsername { get; set; }

        /// <summary>
        /// Web proxy network credentials password
        /// </summary>
        [IgnoreDataMember] // Do not serialize - security consideration
        public string WebProxyNetworkCredentialsPassword { get; set; }

        /// <summary>
        /// Web proxy network credentials domain
        /// </summary>
        public string WebProxyNetworkCredentialsDomain { get; set; }

        /// <summary>
        /// A flag to indicate whether to include custom headers or not<br />
        /// This member appears in config files but is not used
        /// </summary>
        public bool IncludeCustomHeaders { get; set; }

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

    }
}
