// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.CodeGeneration;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceHandlerExport(Common.Constants.ProviderId, AppliesTo = "VB | CSharp | Web")]
    internal class ODataConnectedServiceHandler : ConnectedServiceHandler
    {
        private readonly ICodeGenDescriptorFactory codeGenDescriptorFactory;
        public ODataConnectedServiceHandler() : this(new CodeGenDescriptorFactory()) { }

        public ODataConnectedServiceHandler(ICodeGenDescriptorFactory codeGenDescriptorFactory)
            : base()
        {
            this.codeGenDescriptorFactory = codeGenDescriptorFactory;
        }


        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            var codeGenDescriptor = await SaveServiceInstanceAsync(context);
            var result = new AddServiceInstanceResult(
                context.ServiceInstance.Name,
                new Uri(codeGenDescriptor.ClientDocUri));
            return result;
        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            await SaveServiceInstanceAsync(context);
            return new UpdateServiceInstanceResult();
        }

        private async Task<BaseCodeGenDescriptor> SaveServiceInstanceAsync(ConnectedServiceHandlerContext context)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            var serviceInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCodeAsync(serviceInstance.ServiceConfig.Endpoint, serviceInstance.ServiceConfig.EdmxVersion, context, project);
            context.SetExtendedDesignerData(serviceInstance.ServiceConfig);
            return codeGenDescriptor;
        }

        private async Task<BaseCodeGenDescriptor> GenerateCodeAsync(string metadataUri, Version edmxVersion, ConnectedServiceHandlerContext context, Project project)
        {
            BaseCodeGenDescriptor codeGenDescriptor = codeGenDescriptorFactory.Create(edmxVersion, metadataUri, context, project);
            await codeGenDescriptor.AddNugetPackagesAsync();
            await codeGenDescriptor.AddGeneratedClientCodeAsync();
            return codeGenDescriptor;
        }
    }
}