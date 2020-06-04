//-----------------------------------------------------------------------------------
// <copyright file="SchemaTypeModelToVisibilityConverterTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Converters;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.Converters
{
    [TestClass]
    public class SchemaTypeModelToVisibilityConverterTests
    {
        [TestMethod]
        public void Convert_ShouldReturnVisible_ForSchemaTypeWithBoundOperations()
        {
            var schemaType = new SchemaTypeModel { Name = "Type1", IsSelected = false, BoundOperations = new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    IsSelected = true,
                    Name = "TestOperation(Type1)"
                }
            }};

            var result = new SchemaTypeModelToVisibilityConverter()
                .Convert(schemaType, typeof(Visibility), null, CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Visible);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_ForSchemaTypeWithoutBoundOperations()
        {
            var schemaType = new SchemaTypeModel
            {
                Name = "Type1",
                IsSelected = false,
                BoundOperations = new List<BoundOperationModel>()
            };

            var result = new SchemaTypeModelToVisibilityConverter()
                .Convert(schemaType, typeof(Visibility), null, CultureInfo.CurrentCulture);

            result.Should()
                .Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void Convert_ShouldThrowNotSupportedException_ForNonVisibilityTargetType()
        {
            Action convertAction = () =>
            {
                new SchemaTypeModelToVisibilityConverter()
                    .Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);
            };

            convertAction.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ConvertBack_ShouldThrowNotSupportedException()
        {
            Action convertBackAction = () =>
            {
                new SchemaTypeModelToVisibilityConverter()
                    .ConvertBack(null, typeof(bool), null, CultureInfo.CurrentCulture);
            };

            convertBackAction.ShouldThrow<NotSupportedException>();
        }
    }
}
