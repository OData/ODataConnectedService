// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding
{
    internal static class NuGetPackages
    {
        public const string WebApiNuGetPackageId = "Microsoft.AspNet.WebApi";
        public const string WebApiClientNuGetPackageId = "Microsoft.AspNet.WebApi.Client";
        public const string WebApiCoreNuGetPackageId = "Microsoft.AspNet.WebApi.Core";
        public const string WebApiWebHostNuGetPackageId = "Microsoft.AspNet.WebApi.WebHost";
        public const string NewtonsoftJsonNuGetPackageId = "Newtonsoft.Json";

        public const string ODataNuGetPackageId = "Microsoft.AspNet.OData";
        public const string EdmNuGetPackageId = "Microsoft.OData.Edm";
        public const string MicrosoftODataNuGetPackageId = "Microsoft.OData.Core";
        public const string SpatialNuGetPackageId = "Microsoft.Spatial";

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
