//-----------------------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverterTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.Converters
{
    [TestClass]
    public class BoolToVisibilityConverterTests
    {
        [TestMethod]
        public void Convert_ShouldReturnVisible_ForTrueValue()
        {
            var result = new BoolToVisibilityConverter()
                .Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Visible);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_ForFalseValue()
        {
            var result = new BoolToVisibilityConverter()
                .Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void Convert_ShouldReturnVisible_ForFalseValue_WithTrueParameter()
        {
            var result = new BoolToVisibilityConverter()
                .Convert(false, typeof(Visibility), "true", CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Visible);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_ForTrueValue_WithTrueParameter()
        {
            var result = new BoolToVisibilityConverter()
                .Convert(true, typeof(Visibility), "true", CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void Convert_ShouldThrowNotSupportedException_ForNonVisibilityTargetType()
        {
            Action convertAction = () =>
            {
                new BoolToVisibilityConverter()
                    .Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);
            };

            convertAction.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void Convert_ShouldThrowNotSupportedException_ForNonBooleanValue()
        {
            Action convertAction = () =>
            {
                new BoolToVisibilityConverter()
                    .Convert("string", typeof(Visibility), null, CultureInfo.CurrentCulture);
            };

            convertAction.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ConvertBack_ShouldThrowNotSupportedException()
        {
            Action convertBackAction = () =>
            {
                new BoolToVisibilityConverter()
                    .ConvertBack(null, typeof(bool), null, CultureInfo.CurrentCulture);
            };

            convertBackAction.ShouldThrow<NotSupportedException>();
        }
    }
}
