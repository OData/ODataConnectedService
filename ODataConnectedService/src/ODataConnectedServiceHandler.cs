using System;
using System.ComponentModel.Composition;
using System.Data.Services.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Microsoft.OData.ConnectedService.Common;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceHandlerExport("ODataConnectedService", AppliesTo = "CSharp")]
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

            if (codeGenInstance.GenByDataSvcUtil)
            {
                await this.AddNuGetPackagesAsync(context, project, new Version(3, 0));
                await this.GenerateClientCode(context, project);
            }
            else
            {
                await this.AddNuGetPackagesAsync(context, project, new Version(3, 0));
                await this.GenerateClientCodeWithoutDataSvcUtil(context, project);
            }

            AddServiceInstanceResult result = new AddServiceInstanceResult(
                context.ServiceInstance.Name,
                new Uri("https://github.com/odata/odata.net"));

            return result;
        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            if (codeGenInstance.GenByDataSvcUtil)
            {
                await this.AddNuGetPackagesAsync(context, project, new Version(3, 0));
                await this.GenerateClientCode(context, project);
            }
            else
            {
                await this.AddNuGetPackagesAsync(context, project, new Version(3, 0));
                await this.GenerateClientCodeWithoutDataSvcUtil(context, project);
            }

            UpdateServiceInstanceResult result = new UpdateServiceInstanceResult();
            return result;
        }

        private async Task AddNuGetPackagesAsync(ConnectedServiceHandlerContext context, Project currentProject, Version version)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages");
            Version packageVersion = null;
            if (version.Major.Equals(3))
            {
                var wcfDSInstallLocation = CodeGeneratorUtils.GetWCFDSInstallLocation();

                var packageSource = Path.Combine(wcfDSInstallLocation, @"bin\NuGet");
                if (Directory.Exists(packageSource))
                {
                    var files = Directory.EnumerateFiles(packageSource, "*.nupkg").ToList();
                    foreach (var nugetPackage in Constant.V3NuGetPackages)
                    {
                        if (!files.Any(f => Regex.IsMatch(f, nugetPackage + @"(.\d){2,4}.nupkg")))
                        {
                            packageSource = Constant.NuGetOnlineRepository;
                        }
                    }
                }
                else
                {
                    packageSource = Constant.NuGetOnlineRepository;
                }

                if (!PackageInstallerServices.IsPackageInstalled(currentProject, Constant.V3ClientNuGetPackage))
                {
                    PackageInstaller.InstallPackage(packageSource, currentProject, Constant.V3ClientNuGetPackage, packageVersion, false);
                }
            }
        }

        private async Task GenerateClientCode(ConnectedServiceHandlerContext context, Project project)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...");

            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            string command = Path.Combine(CodeGeneratorUtils.GetWCFDSInstallLocation(), @"bin\tools\DataSvcUtil.exe");

            var referenceFolderPath = await CheckAndAddReferenceFolder(context, project);

            string outputFile = Path.Combine(referenceFolderPath, "Reference.cs");

            StringBuilder arguments = new StringBuilder(string.Format("/c \"\"{0}\" /version:3.0 /language:{1} /out:\"{2}\" /uri:{3}", command, "CSharp", outputFile, codeGenInstance.Endpoint));

            if (codeGenInstance.UseDataServiceCollection)
            {
                arguments.Append("/dataServiceCollection");
            }
            arguments.Append("\"");

            ProcessHelper.ExecuteCommand(command, arguments.ToString());
            await context.HandlerHelper.AddFileAsync(outputFile, outputFile);
        }

        private async Task GenerateClientCodeWithoutDataSvcUtil(ConnectedServiceHandlerContext context, Project project)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...");

            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;
            var address = codeGenInstance.Endpoint;
            if (address.StartsWith("https:") || address.StartsWith("http"))
            {
                if (!address.EndsWith("$metadata"))
                {
                    address = address.TrimEnd('/') + "/$metadata";
                }
            }

            EntityClassGenerator generator = new EntityClassGenerator(LanguageOption.GenerateCSharpCode);
            generator.UseDataServiceCollection = codeGenInstance.UseDataServiceCollection;
            generator.Version = DataServiceCodeVersion.V3;

            var referenceFolderPath = await CheckAndAddReferenceFolder(context, project);

            string outputFile = Path.Combine(referenceFolderPath, "Reference.cs");

            XmlReaderSettings settings = new XmlReaderSettings();
            if (!String.IsNullOrEmpty(address))
            {
                XmlResolver resolver = new XmlUrlResolver();
                {
                    resolver.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }

                settings.XmlResolver = resolver;
            }

            using (XmlReader reader = XmlReader.Create(address, settings))
            {
                using (StreamWriter writer = File.CreateText(outputFile))
                {
                    var error = generator.GenerateCode(reader, writer, codeGenInstance.NamespacePrefix);
                }
            }
            await context.HandlerHelper.AddFileAsync(outputFile, outputFile);
        }

        private Task<string> CheckAndAddReferenceFolder(ConnectedServiceHandlerContext context, Project project)
        {
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;
            var serviceReferenceFolderName = context.HandlerHelper.GetServiceArtifactsRootFolder();

            ProjectItem serviceReferenceFolder;
            ProjectItem namespaceFolder;
            project.ProjectItems.TryGetFolder(serviceReferenceFolderName, out serviceReferenceFolder);

            if (codeGenInstance.NamespacePrefix != null)
            {
                if (!serviceReferenceFolder.ProjectItems.TryGetFolder(codeGenInstance.NamespacePrefix, out namespaceFolder))
                {
                    serviceReferenceFolder.ProjectItems.AddFolder(codeGenInstance.NamespacePrefix);
                }
            }

            var referenceFolderPath = Path.Combine(
                ProjectHelper.GetProjectFullPath(project),
                serviceReferenceFolderName,
                codeGenInstance.NamespacePrefix ?? "");

            return Task.FromResult<string>(referenceFolderPath);
        }
    }
}
