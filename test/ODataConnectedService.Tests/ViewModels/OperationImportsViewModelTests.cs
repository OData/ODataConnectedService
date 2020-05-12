//-----------------------------------------------------------------------------------
// <copyright file="OperationImportsViewModelTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace ODataConnectedService.Tests.ViewModels
{
    [TestClass]
    public class OperationImportsViewModelTests
    {
        [TestMethod]
        public void LoadOperationImports_ShouldSetOperationImportsWithoutDuplicatesAndSelectAll()
        {
            var objectSelection = new OperationImportsViewModel();
            objectSelection.OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = false },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = false }
            };

            var container = new EdmEntityContainer("Test", "Default");
            var listToLoad = new List<IEdmOperationImport>()
            {
                new EdmActionImport(container, "Update", new EdmAction("Test", "Update", null)),
                new EdmFunctionImport(container, "GetTotal",
                    new EdmFunction("Test", "GetTotal",
                        new EdmTypeReferenceForTest(
                            new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Int32), false))),
                new EdmActionImport(container, "Update",
                    new EdmAction("Test", "Update",
                        new EdmTypeReferenceForTest(
                            new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.String), false)))
            };

            objectSelection.LoadOperationImports(listToLoad, new HashSet<string>(),new Dictionary<string,SchemaTypeModel>());

            objectSelection.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetTotal", IsSelected = true },
                new OperationImportModel() { Name = "Update", IsSelected = true }
            });
        }

        [TestMethod]
        public void ExcludeOperationImports_ShouldUnselectTheSpecifiedOperations()
        {
            var objectSelection = new OperationImportsViewModel();
            objectSelection.OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = true },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = false },
                new OperationImportModel() { Name = "Func3", IsSelected = true }
            };

            objectSelection.ExcludeOperationImports(new string[] { "Func1", "Func3", "Func4" });

            objectSelection.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = false },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = false },
                new OperationImportModel() { Name = "Func3", IsSelected = false }
            });
        }

        [TestMethod]
        public void ExcludedOperationImports_ShouldReturnNamesOfUnselectedOperations()
        {
            var objectSelection = new OperationImportsViewModel();
            objectSelection.OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = false },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = false }
            };

            var excluded = objectSelection.ExcludedOperationImportsNames.ToList();

            excluded.ShouldBeEquivalentTo(new List<string>() { "Func1", "Func3" });
        }

        [TestMethod]
        public void EmptyList_ShouldResetOperationImportsToAnEmptyCollection()
        {
            var objectSelection = new OperationImportsViewModel()
            {
                OperationImports = new List<OperationImportModel>()
                {
                    new OperationImportModel() { Name = "Func1", IsSelected = false },
                    new OperationImportModel() { Name = "Func2", IsSelected = true },
                    new OperationImportModel() { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.EmptyList();

            objectSelection.OperationImports.Count().Should().Be(0);
        }
        
        [TestMethod]
        public void SelectAll_ShouldMarkAllOperationsAsSelected()
        {
            var objectSelection = new OperationImportsViewModel()
            {
                OperationImports = new List<OperationImportModel>()
                {
                    new OperationImportModel() { Name = "Func1", IsSelected = false },
                    new OperationImportModel() { Name = "Func2", IsSelected = true },
                    new OperationImportModel() { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.SelectAll();

            objectSelection.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = true },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = true }
            });
        }

        [TestMethod]
        public void UnSelectAll_ShouldMarkAllOperationsAsNotSelected()
        {
            var objectSelection = new OperationImportsViewModel()
            {
                OperationImports = new List<OperationImportModel>()
                {
                    new OperationImportModel() { Name = "Func1", IsSelected = false },
                    new OperationImportModel() { Name = "Func2", IsSelected = true },
                    new OperationImportModel() { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.UnselectAll();

            objectSelection.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = false },
                new OperationImportModel() { Name = "Func2", IsSelected = false },
                new OperationImportModel() { Name = "Func3", IsSelected = false }
            });
        }
    }
}