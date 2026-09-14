//-----------------------------------------------------------------------------------
// <copyright file="NuGetPackageVersionResolverTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using Microsoft.OData.CodeGen.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class NuGetPackageVersionResolverTests
    {
        [TestMethod]
        public void ParseTargetFramework_NormalizesLegacyFrameworkVersion()
        {
            var framework = NuGetPackageVersionResolver.ParseTargetFramework("v4.7.2");

            Assert.AreEqual(".NETFramework", framework.Framework);
            Assert.AreEqual("4.7.2.0", framework.Version.ToString());
        }

        [TestMethod]
        public void ParseTargetFramework_ParsesProjectHelperFramework()
        {
            var framework = NuGetPackageVersionResolver.ParseTargetFramework(".NETFramework,Version=net472");

            Assert.AreEqual(".NETFramework", framework.Framework);
            Assert.AreEqual("4.7.2.0", framework.Version.ToString());
        }

        [TestMethod]
        public void IsCompatibleWithAllFrameworks_RejectsPackageMissingTarget()
        {
            bool compatible = NuGetPackageVersionResolver.IsCompatibleWithAllFrameworks(
                new[] { NuGetFramework.ParseFolder("net8.0"), NuGetFramework.ParseFolder("net472") },
                new[] { NuGetFramework.ParseFolder("net8.0") });

            Assert.IsFalse(compatible);
        }

        [TestMethod]
        public void IsCompatibleWithAllFrameworks_AcceptsPackageSupportingEveryTarget()
        {
            bool compatible = NuGetPackageVersionResolver.IsCompatibleWithAllFrameworks(
                new[] { NuGetFramework.ParseFolder("net8.0"), NuGetFramework.ParseFolder("net472") },
                new[] { NuGetFramework.ParseFolder("net8.0"), NuGetFramework.ParseFolder("net472") });

            Assert.IsTrue(compatible);
        }

        [DataTestMethod]
        [DataRow(Constants.V4ClientNuGetPackage)]
        [DataRow(Constants.V4ODataNuGetPackage)]
        [DataRow(Constants.V4EdmNuGetPackage)]
        [DataRow(Constants.V4SpatialNuGetPackage)]
        public void UsesODataClientVersion_ReturnsTrueForVersionAlignedPackages(string packageId)
        {
            Assert.IsTrue(NuGetPackageVersionResolver.UsesODataClientVersion(packageId));
        }

        [DataTestMethod]
        [DataRow(Constants.V4SystemComponentModelAnnotationsNuGetPackage)]
        [DataRow(Constants.V3ClientNuGetPackage)]
        public void UsesODataClientVersion_ReturnsFalseForIndependentPackages(string packageId)
        {
            Assert.IsFalse(NuGetPackageVersionResolver.UsesODataClientVersion(packageId));
        }
    }
}
