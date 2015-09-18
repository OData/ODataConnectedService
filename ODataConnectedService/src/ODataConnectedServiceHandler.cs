using System;
using System.ComponentModel.Composition;
using System.Data.Services.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EnvDTE;
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
                new Uri(Common.Constants.V3DocUri));

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
                    foreach (var nugetPackage in Common.Constants.V3NuGetPackages)
                    {
                        if (!files.Any(f => Regex.IsMatch(f, nugetPackage + @"(.\d){2,4}.nupkg")))
                        {
                            packageSource = Common.Constants.NuGetOnlineRepository;
                        }
                    }
                }
                else
                {
                    packageSource = Common.Constants.NuGetOnlineRepository;
                }

                if (!PackageInstallerServices.IsPackageInstalled(currentProject, Common.Constants.V3ClientNuGetPackage))
                {
                    PackageInstaller.InstallPackage(packageSource, currentProject, Common.Constants.V3ClientNuGetPackage, packageVersion, false);
                }
            }
        }

        private async Task GenerateClientCode(ConnectedServiceHandlerContext context, Project project)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...");

            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)context.ServiceInstance;

            string command = Path.Combine(CodeGeneratorUtils.GetWCFDSInstallLocation(), @"bin\tools\DataSvcUtil.exe");

            string referenceFolderPath = GetReferenceFolderName(context, project);
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
            string address = codeGenInstance.Endpoint;
            if (address == null)
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

            EntityClassGenerator generator = new EntityClassGenerator(LanguageOption.GenerateCSharpCode);
            generator.UseDataServiceCollection = codeGenInstance.UseDataServiceCollection;
            generator.Version = DataServiceCodeVersion.V3;

            string referenceFolderPath = GetReferenceFolderName(context, project);
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
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(address, settings);

                string tempFile = Path.GetTempFileName();

                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    var errors = generator.GenerateCode(reader, writer, codeGenInstance.NamespacePrefix);
                    if (errors != null && errors.Count() > 0)
                    {
                        foreach (var err in errors)
                        {
                            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.Message);
                        }
                    }
                }

                reader.Close();
                await context.HandlerHelper.AddFileAsync(tempFile, outputFile);
            }
            catch (WebException e)
            {
                throw new Exception(string.Format("Cannot access {0}", address), e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }

        private string GetReferenceFolderName(ConnectedServiceHandlerContext context, Project project)
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
    }
}
