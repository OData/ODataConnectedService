//-----------------------------------------------------------------------------------
// <copyright file="SchemaTypesViewModelTests.cs" company=".NET Foundation">
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
    public class SchemaTypesViewModelTests
    {
        [TestMethod]
        public void LoadSchemaTypes_ShouldSetAllSchemaTypesAsSelectedAndOrderedByName()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {Name = "Type1", IsSelected = false},
                    new SchemaTypeModel {Name = "Type2", IsSelected = true},
                    new SchemaTypeModel {Name = "Type3", IsSelected = false}
                }
            };

            var listToLoad = new List<IEdmSchemaType>
            {
                new EdmEnumType("Test", "EnumType"),
                new EdmComplexType("Test", "ComplexType"),
                new EdmUntypedStructuredType("Test", "UntypedStructuredType"),
                new EdmEntityType("Test", "EntityType"),
                new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Boolean)
            };

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>());

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "ComplexType", Name = "Test.ComplexType", IsSelected = true },
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = true },
                new SchemaTypeModel { ShortName = "EnumType", Name = "Test.EnumType", IsSelected = true },
                new SchemaTypeModel { ShortName = "TypeDef", Name = "Test.TypeDef", IsSelected = true },
                new SchemaTypeModel { ShortName = "UntypedStructuredType", Name = "Test.UntypedStructuredType", IsSelected = true }
            });
        }

        [TestMethod]
        public void LoadSchemaTypes_ShouldAddRelatedTypesForStructuredTypesWithBaseType()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {Name = "Type1", IsSelected = false},
                    new SchemaTypeModel {Name = "Type2", IsSelected = true},
                    new SchemaTypeModel {Name = "Type3", IsSelected = false}
                }
            };

            var listToLoad = new List<IEdmSchemaType>
            {
                new EdmComplexType("Test", "ComplexType", new EdmComplexType("Test", "BaseComplexType")),
                new EdmEntityType("Test", "EntityType", new EdmEntityType("Test", "BaseEntityType")),
                new EdmEnumType("Test", "EnumType"),
                new EdmTypeDefinition("Test", "TypeDef", EdmPrimitiveTypeKind.Boolean),
                new EdmUntypedStructuredType("Test", "UntypedStructuredType")
            };

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>());

            var expectedRelatedTypes = new Dictionary<string, ICollection<string>>
            {
                {"Test.BaseComplexType", new List<string> {"Test.ComplexType"}},
                {"Test.BaseEntityType", new List<string> {"Test.EntityType"}},
            };

            objectSelection.RelatedTypes.ShouldBeEquivalentTo(expectedRelatedTypes);
        }

        [TestMethod]
        public void LoadSchemaTypes_ShouldAddRelatedPropertyTypeForStructuredType_WherePropertyIsCollectionOfEnum()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {Name = "Type1", IsSelected = false},
                    new SchemaTypeModel {Name = "Type2", IsSelected = true},
                    new SchemaTypeModel {Name = "Type3", IsSelected = false}
                }
            };

            var enumType = new EdmEnumType("Test", "EnumType");
            var entityType = new EdmEntityType("Test", "EntityType");
            entityType.AddStructuralProperty("collectionOfEnumProperty",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(new EdmEnumTypeReference(enumType, false))));
            var listToLoad = new List<IEdmSchemaType>
            {
                entityType,
                enumType
            };

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>());

            var expectedRelatedTypes = new Dictionary<string, ICollection<string>>
            {
                {"Test.EnumType", new List<string> {"Test.EntityType"}},
            };

            objectSelection.RelatedTypes.ShouldBeEquivalentTo(expectedRelatedTypes);
        }

        [TestMethod]
        public void SelectSchemaType_ShouldSelectItsRelatedTypes()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {Name = "Type1", IsSelected = false},
                    new SchemaTypeModel {Name = "Type2", IsSelected = true},
                    new SchemaTypeModel {Name = "Type3", IsSelected = false}
                }
            };

            var relatedEntityType = new EdmEntityType("Test", "RelatedEntityType");
            var listToLoad = new List<IEdmSchemaType>
            {
                relatedEntityType,
                new EdmComplexType("Test", "ComplexType", new EdmComplexType("Test", "BaseComplexType")),
                new EdmEntityType("Test", "EntityType", relatedEntityType)
            };

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>());
            objectSelection.DeselectAllSchemaTypes();

            objectSelection.SchemaTypes.First(x => x.ShortName == "EntityType").IsSelected = true;

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "ComplexType", Name = "Test.ComplexType", IsSelected = false },
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = true },
                new SchemaTypeModel { ShortName = "RelatedEntityType", Name = "Test.RelatedEntityType", IsSelected = true }
            });
        }

        [TestMethod]
        public void SelectBoundOperation_ShouldSelectItsRequiredType()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {Name = "Type1", IsSelected = false},
                    new SchemaTypeModel {Name = "Type2", IsSelected = true},
                    new SchemaTypeModel {Name = "Type3", IsSelected = false}
                }
            };

            var relatedEntityType = new EdmEntityType("Test", "RelatedEntityType");
            var listToLoad = new List<IEdmSchemaType>
            {
                relatedEntityType,
                new EdmComplexType("Test", "ComplexType", new EdmComplexType("Test", "BaseComplexType")),
                new EdmEntityType("Test", "EntityType", relatedEntityType)
            };

            var boundOperation = new EdmAction("Test", "BoundOperation",
                new EdmPrimitiveTypeReference(new EdmPrimitiveType(EdmPrimitiveTypeKind.Boolean), false), true,
                new EdmPathExpression(string.Empty));
            boundOperation.AddParameter("relatedEntityTypeParameter",
                new EdmEntityTypeReference(relatedEntityType, false));

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>
            {
                {
                    relatedEntityType, new List<IEdmOperation>
                    {
                        boundOperation
                    }
                }
            });
            objectSelection.DeselectAllSchemaTypes();

            objectSelection.SchemaTypes.FirstOrDefault(x => x.ShortName == "RelatedEntityType")?.IsSelected.Should()
                .BeFalse();
            objectSelection.SchemaTypes.First(x => x.ShortName == "RelatedEntityType").BoundOperations
                .First().IsSelected = true;
            objectSelection.SchemaTypes.FirstOrDefault(x => x.ShortName == "RelatedEntityType")?.IsSelected.Should()
                .BeTrue();

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel
                {
                    ShortName = "ComplexType", Name = "Test.ComplexType", IsSelected = false,
                    BoundOperations = new List<BoundOperationModel>()
                },
                new SchemaTypeModel
                {
                    ShortName = "EntityType", Name = "Test.EntityType", IsSelected = false,
                    BoundOperations = new List<BoundOperationModel>()
                },
                new SchemaTypeModel
                {
                    ShortName = "RelatedEntityType", Name = "Test.RelatedEntityType", IsSelected = true,
                    BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation(Test.RelatedEntityType)",
                            ShortName = "BoundOperation",
                            IsSelected = true
                        }
                    }
                }
            });
        }

        [TestMethod]
        public void DeselectSchemaType_ShouldDeselectItsRelatedTypes()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>()
            };

            var listToLoad = new List<IEdmSchemaType>
            {
                new EdmEntityType("Test", "EntityType", new EdmEntityType("Test", "BaseEntityType")),
                new EdmEntityType("Test", "BaseEntityType")
            };

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>());

            objectSelection.SchemaTypes.First(x => x.ShortName == "BaseEntityType").IsSelected = false;

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "BaseEntityType", Name = "Test.BaseEntityType", IsSelected = false },
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = false }
            });
        }

        [TestMethod]
        public void DeselectSchemaType_ShouldDeselectItsBoundOperations()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>()
            };

            var schemaType = new EdmEntityType("Test", "BaseEntityType");
            var listToLoad = new List<IEdmSchemaType>
            {
                new EdmEntityType("Test", "EntityType", new EdmEntityType("Test", "BaseEntityType")),
                schemaType
            };

            var boundOperation1 = new EdmAction("Test", "BoundOperation1",
                new EdmPrimitiveTypeReference(new EdmPrimitiveType(EdmPrimitiveTypeKind.Boolean), false), true,
                new EdmPathExpression(string.Empty));
            boundOperation1.AddParameter("value",
                new EdmEntityTypeReference(schemaType, false));

            var boundOperation2 = new EdmAction("Test", "BoundOperation2",
                new EdmPrimitiveTypeReference(new EdmPrimitiveType(EdmPrimitiveTypeKind.Boolean), false), true,
                new EdmPathExpression(string.Empty));
            boundOperation2.AddParameter("value",
                new EdmEntityTypeReference(schemaType, false));

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmType, List<IEdmOperation>>
            {
                {
                    schemaType, new List<IEdmOperation>
                    {
                        boundOperation1,
                        boundOperation2
                    }
                }
            });

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "BaseEntityType", Name = "Test.BaseEntityType", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel
                    {
                        Name = "BoundOperation1(Test.BaseEntityType)",
                        ShortName = "BoundOperation1",
                        IsSelected = true
                    },
                    new BoundOperationModel
                    {
                        Name = "BoundOperation2(Test.BaseEntityType)",
                        ShortName = "BoundOperation2",
                        IsSelected = true
                    }
                }},
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = true, BoundOperations = new List<BoundOperationModel>()}
            });

            objectSelection.SchemaTypes.First(x => x.ShortName == "BaseEntityType").IsSelected = false;

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "BaseEntityType", Name = "Test.BaseEntityType", IsSelected = false, BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel
                    {
                        Name = "BoundOperation1(Test.BaseEntityType)",
                        ShortName = "BoundOperation1",
                        IsSelected = false
                    },
                    new BoundOperationModel
                    {
                        Name = "BoundOperation2(Test.BaseEntityType)",
                        ShortName = "BoundOperation2",
                        IsSelected = false
                    }
                }},
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = false, BoundOperations = new List<BoundOperationModel>()}
            });
        }

        [TestMethod]
        public void ExcludeSchemaTypes_ShouldDeselectTheSpecifiedTypesWithSpecifiedBoundOperations()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel
                    {
                        ShortName = "Type1",
                        Name = "Test.Type1",
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>
                        {
                            new BoundOperationModel
                            {
                                IsSelected = true,
                                Name = "BoundOperation1(Type1)"
                            }
                        }
                    },
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type4", Name = "Test.Type4", IsSelected = true}
                }
            };

            objectSelection.ExcludeSchemaTypes(new string[] { "Test.Type1", "Test.Type3", "Test.Type4" }, new string[] { "BoundOperation1(Type1)" });

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>()
            {
                new SchemaTypeModel
                {
                    ShortName = "Type1",
                    Name = "Test.Type1",
                    IsSelected = false,
                    BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            IsSelected = false,
                            Name = "BoundOperation1(Type1)"
                        }
                    }
                },
                new SchemaTypeModel { ShortName = "Type2", Name = "Test.Type2", IsSelected = true },
                new SchemaTypeModel { ShortName = "Type3", Name = "Test.Type3", IsSelected = false },
                new SchemaTypeModel { ShortName = "Type4", Name = "Test.Type4", IsSelected = false }
            });
        }

        [TestMethod]
        public void ExcludeBoundOperations_ShouldDeselectTheSpecifiedBoundOperations()
        {
            var schemaTypeModel = new SchemaTypeModel
            {
                ShortName = "Type1",
                Name = "Test.Type1",
                IsSelected = true,
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel
                    {
                        Name = "BoundOperation1(Test.Type1)",
                        IsSelected = true
                    },
                    new BoundOperationModel
                    {
                        Name = "BoundOperation2(Test.Type1)",
                        IsSelected = true
                    }
                }
            };
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    schemaTypeModel,
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type4", Name = "Test.Type4", IsSelected = true}
                }
            };

            objectSelection.ExcludeBoundOperations(schemaTypeModel, new string[] { "BoundOperation1(Test.Type1)" });

            schemaTypeModel.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = false
                },
                new BoundOperationModel
                {
                    Name = "BoundOperation2(Test.Type1)",
                    IsSelected = true
                }
            });
        }

        [TestMethod]
        public void ExcludedSchemaTypeNames_ShouldReturnNamesOfDeselectedTypes()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            var excluded = objectSelection.ExcludedSchemaTypeNames.ToList();

            excluded.ShouldBeEquivalentTo(new List<string> { "Test.Type1", "Test.Type3" });
        }

        [TestMethod]
        public void ExcludedBoundOperationsNames_ShouldReturnNamesOfDeselectedBoundOperationsOrderedByName()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation1(Test.Type1)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation2(Test.Type2)",
                            IsSelected = true
                        },
                        new BoundOperationModel
                        {
                            Name = "BoundOperation0(Test.Type2)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation4(Test.Type3)",
                            IsSelected = true
                        }
                    }}
                }
            };

            var excluded = objectSelection.ExcludedBoundOperationsNames.ToList();

            excluded.ShouldBeEquivalentTo(new List<string> { "BoundOperation0(Test.Type2)", "BoundOperation1(Test.Type1)" });
        }

        [TestMethod]
        public void ClearSchemaTypes_ShouldResetSchemaTypesToAnEmptyCollection()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.ClearSchemaTypes();

            objectSelection.SchemaTypes.Count().Should().Be(0);
        }

        [TestMethod]
        public void ClearBoundOperationList_ShouldResetBoundOperationsForTheSpecificTypeToAnEmptyList()
        {
            var schemaType = new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false, BoundOperations = new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = false
                }
            }};
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    schemaType,
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.ClearBoundOperationList(schemaType);

            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type1")?.BoundOperations.Should().BeEmpty();
        }

        [TestMethod]
        public void SelectAllSchemaTypes_ShouldMarkAllTypesAsSelected()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.SelectAllSchemaTypes();

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = true},
                new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = true}
            });
        }

        [TestMethod]
        public void SelectAllBoundOperations_ShouldMarkAllBoundOperationsForAllTypesAsSelected()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation1(Test.Type1)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation2(Test.Type2)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.SelectAllBoundOperations();

            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type1")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = true
                }
            });
            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type2")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation2(Test.Type2)",
                    IsSelected = true
                }
            });
        }

        [TestMethod]
        public void SelectAllBoundOperationsForSchemaType_ShouldMarkAllBoundOperationsForTheSpecificTypeAsSelected()
        {
            var schemaType = new SchemaTypeModel
            {
                ShortName = "Type1",
                Name = "Test.Type1",
                IsSelected = false,
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel
                    {
                        Name = "BoundOperation1(Test.Type1)",
                        IsSelected = false
                    },
                    new BoundOperationModel
                    {
                        Name = "BoundOperation2(Test.Type1)",
                        IsSelected = false
                    }
                }
            };
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    schemaType,
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation3(Test.Type2)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.SelectAllBoundOperationsForSchemaType(schemaType);

            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type1")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = true
                },
                new BoundOperationModel
                {
                    Name = "BoundOperation2(Test.Type1)",
                    IsSelected = true
                }
            });
            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type2")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation3(Test.Type2)",
                    IsSelected = false
                }
            });
        }

        [TestMethod]
        public void DeselectAllSchemaTypes_ShouldMarkAllTypesAsNotSelected()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.DeselectAllSchemaTypes();

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false},
                new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = false},
                new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
            });
        }

        [TestMethod]
        public void DeselectAllBoundOperations_ShouldMarkBoundOperationsForAllTypesAsNotSelected()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = false, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation1(Test.Type1)",
                            IsSelected = true
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation2(Test.Type2)",
                            IsSelected = true
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.DeselectAllBoundOperations();

            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type1")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = false
                }
            });
            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type2")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation2(Test.Type2)",
                    IsSelected = false
                }
            });
        }

        [TestMethod]
        public void DeselectAllBoundOperationsForSchemaType_ShouldMarkBoundOperationsForTheSpecificTypeAsNotSelected()
        {
            var schemaType = new SchemaTypeModel
            {
                ShortName = "Type1",
                Name = "Test.Type1",
                IsSelected = false,
                BoundOperations = new List<BoundOperationModel>
                {
                    new BoundOperationModel
                    {
                        Name = "BoundOperation1(Test.Type1)",
                        IsSelected = true
                    },
                    new BoundOperationModel
                    {
                        Name = "BoundOperation2(Test.Type1)",
                        IsSelected = true
                    }
                }
            };
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    schemaType,
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true, BoundOperations = new List<BoundOperationModel>
                    {
                        new BoundOperationModel
                        {
                            Name = "BoundOperation3(Test.Type2)",
                            IsSelected = false
                        }
                    }},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false}
                }
            };

            objectSelection.DeselectAllBoundOperationsForSchemaType(schemaType);

            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type1")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation1(Test.Type1)",
                    IsSelected = false
                },
                new BoundOperationModel
                {
                    Name = "BoundOperation2(Test.Type1)",
                    IsSelected = false
                }
            });
            objectSelection.SchemaTypes.FirstOrDefault(s => s.Name == "Test.Type2")?.BoundOperations.ShouldAllBeEquivalentTo(new List<BoundOperationModel>
            {
                new BoundOperationModel
                {
                    Name = "BoundOperation3(Test.Type2)",
                    IsSelected = false
                }
            });
        }
    }
}
