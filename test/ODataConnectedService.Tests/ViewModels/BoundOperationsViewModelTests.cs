//-----------------------------------------------------------------------------------
// <copyright file="OperationImportsViewModelTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.ViewModels
{
    [TestClass]
    public class BoundOperationsViewModelTests
    {
        [TestMethod]
        public void LoadBoundOperations_ShouldSetBoundOperationsWithoutDuplicatesAndSelectAll()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel {Name = "Func1", IsSelected = false},
                    new BoundOperationModel {Name = "Func2", IsSelected = true},
                    new BoundOperationModel {Name = "Func3", IsSelected = false}
                }
            };

            var listToLoad = new Dictionary<IEdmType, List<IEdmOperation>>
            {
                {
                    new EdmComplexType("Test", "User"), new List<IEdmOperation>
                    {
                        new EdmAction("Test", "Update", null),
                        new EdmFunction("Test", "GetTotal",
                            new EdmTypeReferenceForTest(
                                new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Int32), false)),
                        new EdmAction("Test", "Update",
                            new EdmTypeReferenceForTest(
                                new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.String), false))
                    }
                },
                {
                    new EdmCollectionType(
                        new EdmTypeReferenceForTest(
                            new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Int32), false)),
                    new List<IEdmOperation>
                    {
                        new EdmAction("Test", "Create", null)
                    }
                }
            };

            objectSelection.LoadBoundOperations(listToLoad, new HashSet<string>(), new Dictionary<string, SchemaTypeModel>());

            objectSelection.BoundOperations.ShouldBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel { Name = "Create(Collection(Test.TypeDef))", IsSelected = true },
                new BoundOperationModel { Name = "GetTotal(Test.User)", IsSelected = true },
                new BoundOperationModel { Name = "Update(Test.User)", IsSelected = true }
            });
        }

        [TestMethod]
        public void ExcludeBoundOperations_ShouldDeselectTheSpecifiedBoundOperations()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel {Name = "Func1", IsSelected = true},
                    new BoundOperationModel {Name = "Func2", IsSelected = true},
                    new BoundOperationModel {Name = "Func3", IsSelected = false},
                    new BoundOperationModel {Name = "Func3", IsSelected = true}
                }
            };

            objectSelection.ExcludeBoundOperations(new string[] { "Func1", "Func3", "Func4" });

            objectSelection.BoundOperations.ShouldBeEquivalentTo(new List<OperationImportModel>
            {
                new OperationImportModel { Name = "Func1", IsSelected = false },
                new OperationImportModel { Name = "Func2", IsSelected = true },
                new OperationImportModel { Name = "Func3", IsSelected = false },
                new OperationImportModel { Name = "Func3", IsSelected = false }
            });
        }

        [TestMethod]
        public void ExcludedBoundOperations_ShouldReturnNamesOfDeselectedBoundOperations()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel {Name = "Func1", IsSelected = false},
                    new BoundOperationModel {Name = "Func2", IsSelected = true},
                    new BoundOperationModel {Name = "Func3", IsSelected = false}
                }
            };

            var excluded = objectSelection.ExcludedBoundOperationsNames.ToList();

            excluded.ShouldBeEquivalentTo(new List<string> { "Func1", "Func3" });
        }

        [TestMethod]
        public void EmptyList_ShouldResetBoundOperationsToAnEmptyCollection()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel { Name = "Func1", IsSelected = false },
                    new BoundOperationModel { Name = "Func2", IsSelected = true },
                    new BoundOperationModel { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.EmptyList();

            objectSelection.BoundOperations.Count().Should().Be(0);
        }

        [TestMethod]
        public void SelectAll_ShouldMarkAllBoundOperationsAsSelected()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel { Name = "Func1", IsSelected = false },
                    new BoundOperationModel { Name = "Func2", IsSelected = true },
                    new BoundOperationModel { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.SelectAll();

            objectSelection.BoundOperations.ShouldBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel { Name = "Func1", IsSelected = true },
                new BoundOperationModel { Name = "Func2", IsSelected = true },
                new BoundOperationModel { Name = "Func3", IsSelected = true }
            });
        }

        [TestMethod]
        public void DeselectAll_ShouldMarkAllBoundOperationsAsNotSelected()
        {
            var objectSelection = new BoundOperationsViewModel
            {
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel { Name = "Func1", IsSelected = false },
                    new BoundOperationModel { Name = "Func2", IsSelected = true },
                    new BoundOperationModel { Name = "Func3", IsSelected = false }
                }
            };

            objectSelection.DeselectAll();

            objectSelection.BoundOperations.ShouldBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel { Name = "Func1", IsSelected = false },
                new BoundOperationModel { Name = "Func2", IsSelected = false },
                new BoundOperationModel { Name = "Func3", IsSelected = false }
            });
        }
    }
}