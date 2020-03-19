﻿using System;
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
    public class ObjectSelectionViewModelTests
    {
        [TestMethod]
        public void LoadOperationImports_ShouldSetOperationImportsWithoutDuplicatesAndSelectAll()
        {
            var objectSelection = new ObjectSelectionViewModel();
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

            objectSelection.LoadOperationImports(listToLoad);

            objectSelection.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetTotal", IsSelected = true },
                new OperationImportModel() { Name = "Update", IsSelected = true }
            });
        }

        [TestMethod]
        public void ExcludeOperationImports_ShouldUnselectTheSpecifiedOperations()
        {
            var objectSelection = new ObjectSelectionViewModel();
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
            var objectSelection = new ObjectSelectionViewModel();
            objectSelection.OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "Func1", IsSelected = false },
                new OperationImportModel() { Name = "Func2", IsSelected = true },
                new OperationImportModel() { Name = "Func3", IsSelected = false }
            };

            var excluded = objectSelection.ExcludedOperationImportsNames.ToList();

            excluded.ShouldBeEquivalentTo(new List<string>() { "Func1", "Func3" });
        }

    }
}