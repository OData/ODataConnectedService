using System;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.CodeGeneration;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceHandlerExport(Common.Constants.ProviderId, AppliesTo = "CSharp")]
    internal class ODataConnectedServiceHandler : ConnectedServiceHandler
    {
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
    }
}
