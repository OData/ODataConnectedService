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
        public static async Task<string> GetLatestCompatibleVersionAsync(string packageSource, string packageId, string targetFramework)
        {
            SourceRepository repository = new SourceRepository(new PackageSource(packageSource), Repository.Provider.GetCoreV3());
            PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);

            using (var cacheContext = new SourceCacheContext())
            {
                IEnumerable<IPackageSearchMetadata> metadata = await metadataResource
                    .GetMetadataAsync(packageId, includePrerelease: false, includeUnlisted: false, cacheContext, NullLogger.Instance, CancellationToken.None)
                    .ConfigureAwait(false);

                IReadOnlyList<IPackageSearchMetadata> packages = metadata.OrderByDescending(candidate => candidate.Identity.Version).ToList();

                if (string.IsNullOrWhiteSpace(targetFramework))
                {
                    return packages.FirstOrDefault()?.Identity.Version.ToNormalizedString();
                }

                FindPackageByIdResource packageResource = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
                NuGetFramework framework = targetFramework.StartsWith(".", StringComparison.Ordinal)
                    ? NuGetFramework.ParseFrameworkName(targetFramework, DefaultFrameworkNameProvider.Instance)
                    : NuGetFramework.ParseFolder(targetFramework);

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

                            if (new FrameworkReducer().GetNearest(framework, packageFrameworks) != null)
                            {
                                return package.Identity.Version.ToNormalizedString();
                            }
                        }
                    }
                }

                return packages.FirstOrDefault()?.Identity.Version.ToNormalizedString();
            }
        }
    }
}
