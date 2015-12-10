// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    internal static class NuGetPackages
    {
        // Web Api assembly
        public static readonly string WebApiNuGetPackageId = "Microsoft.AspNet.WebApi";
        public static readonly string WebApiClientNuGetPackageId = "Microsoft.AspNet.WebApi.Client";
        public static readonly string WebApiCoreNuGetPackageId = "Microsoft.AspNet.WebApi.Core";
        public static readonly string WebApiWebHostNuGetPackageId = "Microsoft.AspNet.WebApi.WebHost";
        public static readonly string NewtonsoftJsonNuGetPackageId = "Newtonsoft.Json";

        // EntityFramework assembly
        public static readonly string EntityFrameworkId = "EntityFramework";

        // OData Lib assembly
        public static readonly string ODataEdmNuGetPackageId = "Microsoft.OData.Edm";
        public static readonly string ODataMicrosoftODataNuGetPackageId = "Microsoft.OData.Core";
        public static readonly string ODataSpatialNuGetPackageId = "Microsoft.Spatial";

        // OData Web Api assembly
        public static readonly string ODataWebApiNuGetPackageId = "Microsoft.AspNet.OData";

        // OData Restier assembly
        public static readonly string ODataRestierNugetPackageId = "Microsoft.Restier";
        public static readonly string ODataRestierCoreNugetPackageId = "Microsoft.Restier.Core";
        public static readonly string ODataRestierEntityFramweorkNugetPackageId = "Microsoft.Restier.EntityFramework";
        public static readonly string ODataRestierWebApiNugetPackageId = "Microsoft.Restier.WebApi";

        public static readonly string[] WebApiPackageSet = new string[]
        {
            NewtonsoftJsonNuGetPackageId,
            WebApiClientNuGetPackageId,
            WebApiCoreNuGetPackageId,
            WebApiWebHostNuGetPackageId,
            WebApiNuGetPackageId,
        };

        public static readonly string[] RestierPackageSet = new string[]
        {
            ODataEdmNuGetPackageId,
            ODataMicrosoftODataNuGetPackageId,
            ODataSpatialNuGetPackageId,
            ODataWebApiNuGetPackageId,
            EntityFrameworkId,
            ODataRestierNugetPackageId,
            ODataRestierCoreNugetPackageId,
            ODataRestierEntityFramweorkNugetPackageId,
            ODataRestierWebApiNugetPackageId,
        };
    }
}
