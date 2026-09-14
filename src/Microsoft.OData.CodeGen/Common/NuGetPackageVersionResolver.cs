//-----------------------------------------------------------------------------
// <copyright file="NuGetPackageVersionResolver.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;

namespace Microsoft.OData.CodeGen.Common
{
    public static class NuGetPackageVersionResolver
    {
        /// <summary>
        /// Get the latest compatible version of a NuGet package from a given package source for a specified target framework.
        /// </summary>
        /// <param name="packageSource">The source of the NuGet package.</param>
        /// <param name="packageId">The ID of the NuGet package.</param>
        /// <param name="targetFramework">The target framework to check compatibility against.</param>
        /// <returns>The latest compatible version of the NuGet package as a string.</returns>
        public static Task<string> GetLatestCompatibleVersionAsync(string packageSource, string packageId, string targetFramework) =>
            GetLatestCompatibleVersionAsync(packageSource, packageId, new[] { targetFramework });

        /// <summary>
        /// Get the latest version of a NuGet package compatible with all specified target frameworks.
        /// </summary>
        /// <param name="packageSource">The source of the NuGet package.</param>
        /// <param name="packageId">The ID of the NuGet package.</param>
        /// <param name="targetFrameworks">The target frameworks to check compatibility against.</param>
        /// <returns>The latest compatible version of the NuGet package as a string.</returns>
        public static async Task<string> GetLatestCompatibleVersionAsync(string packageSource, string packageId, IEnumerable<string> targetFrameworks)
        {
            SourceRepository repository = new SourceRepository(
                new PackageSource(packageSource),
                // The core provider set includes both V3 and legacy V2 feed providers.
                Repository.Provider.GetCoreV3());
            PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);

            using (var cacheContext = new SourceCacheContext())
            {
                IEnumerable<IPackageSearchMetadata> metadata = await metadataResource
                    .GetMetadataAsync(packageId, includePrerelease: false, includeUnlisted: false, cacheContext, NullLogger.Instance, CancellationToken.None)
                    .ConfigureAwait(false);

                IReadOnlyList<IPackageSearchMetadata> packages = metadata.OrderByDescending(candidate => candidate.Identity.Version).ToList();
                IReadOnlyList<NuGetFramework> frameworks = targetFrameworks
                    .Where(targetFramework => !string.IsNullOrWhiteSpace(targetFramework))
                    .Select(ParseTargetFramework)
                    .ToList();

                if (frameworks.Count == 0)
                {
                    return packages.FirstOrDefault()?.Identity.Version.ToNormalizedString();
                }

                FindPackageByIdResource packageResource = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
                FrameworkReducer frameworkReducer = new FrameworkReducer();

                foreach (IPackageSearchMetadata package in packages)
                {
                    using (var packageStream = new MemoryStream())
                    {
                        bool downloaded = await packageResource
                            .CopyNupkgToStreamAsync(packageId, package.Identity.Version, packageStream, cacheContext, NullLogger.Instance, CancellationToken.None)
                            .ConfigureAwait(false);
                        if (!downloaded)
                        {
                            continue;
                        }

                        packageStream.Position = 0;
                        using (var packageReader = new PackageArchiveReader(packageStream))
                        {
                            IEnumerable<NuGetFramework> packageFrameworks = await packageReader
                                .GetSupportedFrameworksAsync(CancellationToken.None)
                                .ConfigureAwait(false);

                            if (IsCompatibleWithAllFrameworks(frameworks, packageFrameworks, frameworkReducer))
                            {
                                return package.Identity.Version.ToNormalizedString();
                            }
                        }
                    }
                }

                return packages.FirstOrDefault()?.Identity.Version.ToNormalizedString();
            }
        }

        /// <summary>
        /// Checks if the package frameworks are compatible with all the target frameworks.
        /// </summary>
        /// <param name="targetFrameworks">The target frameworks to check compatibility against.</param>
        /// <param name="packageFrameworks">The package frameworks to check for compatibility.</param>
        /// <param name="frameworkReducer">An optional framework reducer to use for compatibility checks.</param>
        /// <returns>True if the package frameworks are compatible with all the target frameworks; otherwise, false.</returns>
        internal static bool IsCompatibleWithAllFrameworks(
            IEnumerable<NuGetFramework> targetFrameworks, IEnumerable<NuGetFramework> packageFrameworks, FrameworkReducer frameworkReducer = null)
        {
            frameworkReducer = frameworkReducer ?? new FrameworkReducer();
            return targetFrameworks.All(
                targetFramework => frameworkReducer.GetNearest(targetFramework, packageFrameworks) != null);
        }

        internal static NuGetFramework ParseTargetFramework(string targetFramework)
        {
            if (targetFramework.StartsWith("v4.", StringComparison.OrdinalIgnoreCase))
            {
                targetFramework = $".NETFramework,Version={targetFramework}";
            }

            int versionSeparator = targetFramework.LastIndexOf('=');
            if (versionSeparator >= 0 && targetFramework.Substring(versionSeparator + 1).StartsWith("net", StringComparison.OrdinalIgnoreCase))
            {
                return NuGetFramework.ParseFolder(targetFramework.Substring(versionSeparator + 1));
            }

            return targetFramework.StartsWith(".", StringComparison.Ordinal)
                ? NuGetFramework.ParseFrameworkName(targetFramework, DefaultFrameworkNameProvider.Instance)
                : NuGetFramework.ParseFolder(targetFramework);
        }
    }
}
