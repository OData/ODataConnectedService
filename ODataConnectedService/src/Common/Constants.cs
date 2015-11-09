using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.ConnectedService.Common
{
    internal class Constants
    {
        public static Version EdmxVersion1 = new Version(1, 0);
        public static Version EdmxVersion2 = new Version(2, 0);
        public static Version EdmxVersion3 = new Version(3, 0);
        public static Version EdmxVersion4 = new Version(4, 0);

        public const string V3ClientNuGetPackage = "Microsoft.Data.Services.Client";
        public const string V3ODataNuGetPackage = "Microsoft.Data.OData";
        public const string V3EdmNuGetPackage = "Microsoft.Data.Edm";
        public const string V3SpatialNuGetPackage = "System.Spatial";

        public const string V4ClientNuGetPackage = "Microsoft.OData.Client";
        public const string V4ODataNuGetPackage = "Microsoft.OData.Core";
        public const string V4EdmNuGetPackage = "Microsoft.OData.Edm";
        public const string V4SpatialNuGetPackage = "Microsoft.Spatial";

        public const string EdmxVersion1Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx";
        public const string EdmxVersion2Namespace = "http://schemas.microsoft.com/ado/2008/10/edmx";
        public const string EdmxVersion3Namespace = "http://schemas.microsoft.com/ado/2009/11/edmx";
        public const string EdmxVersion4Namespace = "http://docs.oasis-open.org/odata/ns/edmx";

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

        public static Dictionary<Version, string> SupportedEdmxVersions = new Dictionary<Version, string>
        {
            { EdmxVersion1, EdmxVersion1Namespace},
            { EdmxVersion2, EdmxVersion2Namespace},
            { EdmxVersion3, EdmxVersion3Namespace},
            { EdmxVersion4, EdmxVersion4Namespace}
        };

        internal static Dictionary<string, Version> SupportedEdmxNamespaces = SupportedEdmxVersions.ToDictionary(v => v.Value, v => v.Key);
    }
}
