using System;

namespace Microsoft.OData.ConnectedService.Common
{
    public class Constants
    {
        public static Version V3Version = new Version(3, 0);
        public static Version V4Version = new Version(4, 0);

        public const string V3ClientNuGetPackage = "Microsoft.Data.Services.Client";
        public const string V3ODataNuGetPackage = "Microsoft.Data.OData";
        public const string V3EdmNuGetPackage = "Microsoft.Data.Edm";
        public const string V3SpatialNuGetPackage = "System.Spatial";

        public const string V4ClientNuGetPackage = "Microsoft.OData.Client";
        public const string V4ODataNuGetPackage = "Microsoft.OData.Core";
        public const string V4EdmNuGetPackage = "Microsoft.OData.Edm";
        public const string V4SpatialNuGetPackage = "Microsoft.Spatial";

        public const string ProviderId = "Microsoft.OData.ConnectedService";

        public const string V3DocUri = "https://msdn.microsoft.com/en-us/library/cc668772(v=vs.110).aspx";
        public const string V4DocUri = "http://odata.github.io/odata.net/";

        public const string NuGetOnlineRepository = "https://www.nuget.org/api/v2/";

        public static string[] V3NuGetPackages = new string[]
        {
            V3ClientNuGetPackage,
            V3ODataNuGetPackage,
            V3EdmNuGetPackage,
            V3SpatialNuGetPackage
        };

        public static string[] V4NuGetPackages = new string[]
        {
            V4ClientNuGetPackage,
            V4ODataNuGetPackage,
            V4EdmNuGetPackage,
            V4SpatialNuGetPackage
        };
    }
}
