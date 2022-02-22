//---------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Data.Services.Design;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.CodeGeneration;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceHandlerExport(Microsoft.OData.CodeGen.Common.Constants.ProviderId, AppliesTo = "VB | CSharp | Web")]
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
            var codeGenDescriptor = await SaveServiceInstanceAsync(context).ConfigureAwait(false);
            var result = new AddServiceInstanceResult(
                context.ServiceInstance.Name,
                new Uri(codeGenDescriptor.ClientDocUri));
            return result;
        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            await SaveServiceInstanceAsync(context).ConfigureAwait(false);
            return new UpdateServiceInstanceResult();
        }

        private async Task<BaseCodeGenDescriptor> SaveServiceInstanceAsync(ConnectedServiceHandlerContext context)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            var serviceInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCodeAsync(serviceInstance.ServiceConfig.Endpoint, serviceInstance.ServiceConfig.EdmxVersion, context, project).ConfigureAwait(false);            
            if (!serviceInstance.ServiceConfig.StoreCustomHttpHeaders)
            {
                serviceInstance.ServiceConfig.CustomHttpHeaders = null;
            }
            if (!serviceInstance.ServiceConfig.StoreWebProxyNetworkCredentials)
            {
                serviceInstance.ServiceConfig.WebProxyNetworkCredentialsUsername = null;
                serviceInstance.ServiceConfig.WebProxyNetworkCredentialsPassword = null;
                serviceInstance.ServiceConfig.WebProxyNetworkCredentialsDomain = null;
            }

            context.SetExtendedDesignerData(serviceInstance.ServiceConfig);
            return codeGenDescriptor;
        }

        private async Task<BaseCodeGenDescriptor> GenerateCodeAsync(string metadataUri, Version edmxVersion, ConnectedServiceHandlerContext context, Project project)
        {
            string outputDirectory = "";
            if (context.HandlerHelper != null)
            {
                string serviceReferenceFolderName = context.HandlerHelper.GetServiceArtifactsRootFolder();

                outputDirectory = Path.Combine(
                    project.GetFullPath(),
                    serviceReferenceFolderName,
                    context.ServiceInstance.Name);
            }

            LanguageOption languageOption;
            if (project != null)
            {
                languageOption = project.GetLanguageOption();
            }
            else 
            {
                languageOption = LanguageOption.GenerateCSharpCode;
            }

            BaseCodeGenDescriptor codeGenDescriptor = codeGenDescriptorFactory.Create(edmxVersion, new ConnectedServiceFileHandler(context, project), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));
            await codeGenDescriptor.AddNugetPackagesAsync().ConfigureAwait(false);
            await codeGenDescriptor.AddGeneratedClientCodeAsync(metadataUri, outputDirectory, (Microsoft.OData.CodeGen.Common.LanguageOption)languageOption, ((ODataConnectedServiceInstance)context.ServiceInstance).ServiceConfig).ConfigureAwait(false);
            return codeGenDescriptor;
        }
    }
}