// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    internal static class NuGetPackages
    {
        // Web Api assembly
        public const string WebApiNuGetPackageId = "Microsoft.AspNet.WebApi";
        public const string WebApiClientNuGetPackageId = "Microsoft.AspNet.WebApi.Client";
        public const string WebApiCoreNuGetPackageId = "Microsoft.AspNet.WebApi.Core";
        public const string WebApiWebHostNuGetPackageId = "Microsoft.AspNet.WebApi.WebHost";
        public const string NewtonsoftJsonNuGetPackageId = "Newtonsoft.Json";

        // EntityFramework assembly
        public const string EntityFrameworkId = "EntityFramework";

        // OData Lib assembly
        public const string ODataEdmNuGetPackageId = "Microsoft.OData.Edm";
        public const string ODataMicrosoftODataNuGetPackageId = "Microsoft.OData.Core";
        public const string ODataSpatialNuGetPackageId = "Microsoft.Spatial";

        // OData Web Api assembly
        public const string ODataWebApiNuGetPackageId = "Microsoft.AspNet.OData";

        // OData Restier assembly
        public const string ODataRestierNugetPackageId = "Microsoft.Restier";
        public const string ODataRestierCoreNugetPackageId = "Microsoft.Restier.Core";
        public const string ODataRestierEntityFramweorkNugetPackageId = "Microsoft.Restier.EntityFramework";
        public const string ODataRestierWebApiNugetPackageId = "Microsoft.Restier.WebApi";

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
