using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Shell = Microsoft.VisualStudio.Shell;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        public IVsPackageInstaller PackageInstaller { get; private set; }
        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }

        public ConnectedServiceHandlerContext Context { get; private set; }
        public Project Project { get; private set; }
        public string MetadataUri { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        internal string ClientDocUri { get; set; }

        public BaseCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext Context, Project project)
        {
            this.Init();

            this.MetadataUri = metadataUri;
            this.Context = Context;
            this.Project = project;
        }

        private void Init()
        {
            var componentModel = (IComponentModel)Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public abstract Task AddNugetPackages();
        public abstract Task AddGeneratedClientCode();

        public string GetReferenceFilePath()
        {
            ODataConnectedServiceInstance codeGenInstance = (ODataConnectedServiceInstance)this.Context.ServiceInstance;
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();

            var referenceFolderPath = Path.Combine(
                ProjectHelper.GetProjectFullPath(this.Project),
                serviceReferenceFolderName,
                this.Context.ServiceInstance.Name,
                codeGenInstance.NamespacePrefix ?? "",
                "Reference.cs");

            return referenceFolderPath;
        }
    }
}
