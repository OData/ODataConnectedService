// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding
{
    internal static class NuGetPackages
    {
        public static readonly string WebApiNuGetPackageId = "Microsoft.AspNet.WebApi";
        public static readonly string WebApiClientNuGetPackageId = "Microsoft.AspNet.WebApi.Client";
        public static readonly string WebApiCoreNuGetPackageId = "Microsoft.AspNet.WebApi.Core";
        public static readonly string WebApiWebHostNuGetPackageId = "Microsoft.AspNet.WebApi.WebHost";
        public static readonly string NewtonsoftJsonNuGetPackageId = "Newtonsoft.Json";

        public static readonly string ODataNuGetPackageId = "Microsoft.AspNet.OData";
        public static readonly string EdmNuGetPackageId = "Microsoft.OData.Edm";
        public static readonly string MicrosoftODataNuGetPackageId = "Microsoft.OData.Core";
        public static readonly string SpatialNuGetPackageId = "Microsoft.Spatial";

        public static readonly string[] WebApiPackageSet = new string[]
        {
            NewtonsoftJsonNuGetPackageId,
            WebApiClientNuGetPackageId,
            WebApiCoreNuGetPackageId,
            WebApiWebHostNuGetPackageId,
            WebApiNuGetPackageId,
        };

        public static readonly string[] ODataPackageSet = new string[]
        {
            EdmNuGetPackageId,
            SpatialNuGetPackageId,
            MicrosoftODataNuGetPackageId,
            ODataNuGetPackageId,
        };
    }
}
