using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EnvDTE;
using Microsoft.OData.ConnectedService.CodeGeneration;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceHandlerExport(Common.Constants.ProviderId, AppliesTo = "CSharp")]
    internal class ODataConnectedServiceHandler : ConnectedServiceHandler
    {
        [Import]
        internal IVsPackageInstaller PackageInstaller { get; set; }
        [Import]
        internal IVsPackageInstallerServices PackageInstallerServices { get; set; }

        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCode(codeGenInstance.MetadataTempFilePath, codeGenInstance.EdmxVersion, context, project);
            return new AddServiceInstanceResult(
                context.ServiceInstance.Name,
                new Uri(codeGenDescriptor.ClientDocUri));

        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCode(codeGenInstance.MetadataTempFilePath, codeGenInstance.EdmxVersion, context, project);
            return new UpdateServiceInstanceResult();
        }

        public async Task<BaseCodeGenDescriptor> AddNugetPackagesAndGenerateClientCode(ConnectedServiceHandlerContext context, Project project)
        {
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;
            string address = codeGenInstance.Endpoint;
            if (String.IsNullOrEmpty(address))
            {
                throw new Exception("Please input the service endpoint");
            }

            if (address.StartsWith("https:") || address.StartsWith("http"))
            {
                if (!address.EndsWith("$metadata"))
                {
                    address = address.TrimEnd('/') + "/$metadata";
                }
            }

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                XmlResolver = new XmlUrlResolver()
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                }
            };
            using (XmlReader reader = XmlReader.Create(address, settings))
            {
                try
                {
                    var version = GetODataServiceVersion(reader);
                    return await GenerateCode(address, version, context, project);
                }
                catch (WebException e)
                {
                    throw new Exception(string.Format("Cannot access {0}", address), e);
                }
            }
        }

        private async Task<BaseCodeGenDescriptor> GenerateCode(string metadataUri, Version edmxVersion, ConnectedServiceHandlerContext context, Project project)
        {
            BaseCodeGenDescriptor codeGenDescriptor;

            if (edmxVersion == Common.Constants.EdmxVersion1
                || edmxVersion == Common.Constants.EdmxVersion2
                || edmxVersion == Common.Constants.EdmxVersion3)
            {
                codeGenDescriptor = new V3CodeGenDescriptor(metadataUri, context, project);
            }
            else if (edmxVersion == Common.Constants.EdmxVersion4)
            {
                codeGenDescriptor = new V4CodeGenDescriptor(metadataUri, context, project);
            }
            else
            {
                throw new Exception(string.Format("Not supported Edmx Version {0}", edmxVersion.ToString()));
            }

            await codeGenDescriptor.AddNugetPackages();
            await codeGenDescriptor.AddGeneratedClientCode();
            return codeGenDescriptor;
        }

        internal string GetReferenceFolderName(ConnectedServiceHandlerContext context, Project project)
        {
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;
            var serviceReferenceFolderName = context.HandlerHelper.GetServiceArtifactsRootFolder();

            var referenceFolderPath = Path.Combine(
                ProjectHelper.GetProjectFullPath(project),
                serviceReferenceFolderName,
                context.ServiceInstance.Name,
                codeGenInstance.NamespacePrefix ?? "");

            return referenceFolderPath;
        }

        private Version GetODataServiceVersion(XmlReader reader)
        {
            Version edmxVersion = null;
            while (reader.NodeType != XmlNodeType.Element)
            {
                reader.Read();
            }

            if (reader.EOF)
            {
                throw new Exception("The metadata is an empty file");
            }

            Common.Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
            return edmxVersion;
        }
    }
}
