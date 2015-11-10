using System;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.Templates;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    internal class V4CodeGenDescriptor : BaseCodeGenDescriptor
    {
        public V4CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
            : base(metadataUri, context, project)
        {
            this.ClientNuGetPackageName = Common.Constants.V4ClientNuGetPackage;
            this.ClientDocUri = Common.Constants.V4DocUri;
        }

        public override async Task AddNugetPackages()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages");

            if (!PackageInstallerServices.IsPackageInstalled(this.Project, this.ClientNuGetPackageName))
            {
                Version packageVersion = null;
                PackageInstaller.InstallPackage(Common.Constants.NuGetOnlineRepository, this.Project, this.ClientNuGetPackageName, packageVersion, false);
            }
        }

        public override async Task AddGeneratedClientCode()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...");

            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)this.Context.ServiceInstance;

            ODataT4CodeGenerator t4CodeGenerator = new ODataT4CodeGenerator();
            t4CodeGenerator.MetadataDocumentUri = MetadataUri;
            t4CodeGenerator.UseDataServiceCollection = codeGenInstance.UseDataServiceCollection;
            t4CodeGenerator.TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp;
            t4CodeGenerator.IgnoreUnexpectedElementsAndAttributes = codeGenInstance.IgnoreUnexpectedElementsAndAttributes;
            t4CodeGenerator.EnableNamingAlias = codeGenInstance.EnableNamingAlias;
            t4CodeGenerator.NamespacePrefix = codeGenInstance.NamespacePrefix;

            string tempFile = Path.GetTempFileName();

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                await writer.WriteAsync(t4CodeGenerator.TransformText());
                if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                {
                    foreach (var err in t4CodeGenerator.Errors)
                    {
                        await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.ToString());
                    }
                }
            }

            string outputFile = GetReferenceFilePath();
            await this.Context.HandlerHelper.AddFileAsync(tempFile, outputFile);
        }
    }
}
