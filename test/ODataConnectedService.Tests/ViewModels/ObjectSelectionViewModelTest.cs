using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.ViewModels
{
    [TestClass]
    public class ObjectSelectionViewModelTests
    {
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

            Assert.AreEqual(2, excluded.Count);
            Assert.AreEqual("Func1", excluded[0]);
            Assert.AreEqual("Func3", excluded[1]);
        }

        [TestMethod]
        public void LoadOperationImports_ShouldSetOperationImportsAndSelectAll()
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
                            new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Int32), false)))
            };

            objectSelection.LoadOperationImports(listToLoad);

            Assert.AreEqual(2, objectSelection.OperationImports.Count);
            Assert.AreEqual("Update", objectSelection.OperationImports[0].Name);
            Assert.IsTrue(objectSelection.OperationImports[0].IsSelected);
            Assert.AreEqual("GetTotal", objectSelection.OperationImports[1].Name);
            Assert.IsTrue(objectSelection.OperationImports[1].IsSelected);
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

            Assert.AreEqual(4, objectSelection.OperationImports.Count);
            Assert.IsFalse(objectSelection.OperationImports[0].IsSelected);
            Assert.IsTrue(objectSelection.OperationImports[1].IsSelected);
            Assert.IsFalse(objectSelection.OperationImports[2].IsSelected);
            Assert.IsFalse(objectSelection.OperationImports[3].IsSelected);
        }
    }
}