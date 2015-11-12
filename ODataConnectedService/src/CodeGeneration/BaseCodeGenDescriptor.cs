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
        public ODataConnectedServiceInstance CodeGenInstance { get; set; }
        public Project Project { get; private set; }
        public string MetadataUri { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        public string ClientDocUri { get; set; }
        protected string GeneratedFileNamePrefix
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.CodeGenInstance.GeneratedFileNamePrefix)
                    ? "Reference" : this.CodeGenInstance.GeneratedFileNamePrefix;
            }
        }

        protected string CurrentAssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(this.GetType().Assembly.Location);
            }
        }

        public BaseCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext Context, Project project)
        {
            this.Init();

            this.MetadataUri = metadataUri;
            this.Context = Context;
            this.Project = project;
            this.CodeGenInstance = (ODataConnectedServiceInstance)this.Context.ServiceInstance;
        }

        private void Init()
        {
            var componentModel = (IComponentModel)Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public abstract Task AddNugetPackages();
        public abstract Task AddGeneratedClientCode();

        protected string GetReferenceFileFolder()
        {
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();

            var referenceFolderPath = Path.Combine(
                ProjectHelper.GetProjectFullPath(this.Project),
                serviceReferenceFolderName,
                CodeGenInstance.Name,
                CodeGenInstance.NamespacePrefix ?? "");

            return referenceFolderPath;
        }
    }
}
