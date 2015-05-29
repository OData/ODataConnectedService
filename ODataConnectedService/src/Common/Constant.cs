using System;

namespace Microsoft.OData.ConnectedService.Common
{
    public class Constant
    {
        public static string NuGetOnlineRepository = "https://www.nuget.org/api/v2/";
        public static Version V3Version = new Version(3, 0);
        public static Version V4Version = new Version(4, 0);

        public static string V3ClientNuGetPackage = "Microsoft.Data.Services.Client";
        public static string V3ODataNuGetPackage = "Microsoft.Data.OData";
        public static string V3EdmNuGetPackage = "Microsoft.Data.Edm";
        public static string V3SpatialNuGetPackage = "System.Spatial";

        public static string V4ClientNuGetPackage = "Microsoft.OData.Client";
        public static string V4ODataNuGetPackage = "Microsoft.OData.Core";
        public static string V4EdmNuGetPackage = "Microsoft.OData.Edm";
        public static string V4SpatialNuGetPackage = "Microsoft.Spatial";

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
