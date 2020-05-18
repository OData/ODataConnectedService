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

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmStructuredType, List<IEdmOperation>>());

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

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmStructuredType, List<IEdmOperation>>());

            var expectedRelatedTypes = new Dictionary<string, ICollection<string>>
            {
                {"Test.BaseComplexType", new List<string> {"Test.ComplexType"}},
                {"Test.BaseEntityType", new List<string> {"Test.EntityType"}},
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

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmStructuredType, List<IEdmOperation>>());
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

            objectSelection.LoadSchemaTypes(listToLoad, new Dictionary<IEdmStructuredType, List<IEdmOperation>>());

            objectSelection.SchemaTypes.First(x => x.ShortName == "BaseEntityType").IsSelected = false;

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
            {
                new SchemaTypeModel { ShortName = "BaseEntityType", Name = "Test.BaseEntityType", IsSelected = false },
                new SchemaTypeModel { ShortName = "EntityType", Name = "Test.EntityType", IsSelected = false }
            });
        }

        [TestMethod]
        public void ExcludeSchemaTypes_ShouldDeselectTheSpecifiedTypes()
        {
            var objectSelection = new SchemaTypesViewModel
            {
                SchemaTypes = new List<SchemaTypeModel>
                {
                    new SchemaTypeModel {ShortName = "Type1", Name = "Test.Type1", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type2", Name = "Test.Type2", IsSelected = true},
                    new SchemaTypeModel {ShortName = "Type3", Name = "Test.Type3", IsSelected = false},
                    new SchemaTypeModel {ShortName = "Type4", Name = "Test.Type4", IsSelected = true}
                }
            };

            objectSelection.ExcludeSchemaTypes(new string[] { "Test.Type1", "Test.Type3", "Test.Type4" });

            objectSelection.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>()
            {
                new SchemaTypeModel { ShortName = "Type1", Name = "Test.Type1", IsSelected = false },
                new SchemaTypeModel { ShortName = "Type2", Name = "Test.Type2", IsSelected = true },
                new SchemaTypeModel { ShortName = "Type3", Name = "Test.Type3", IsSelected = false },
                new SchemaTypeModel { ShortName = "Type4", Name = "Test.Type4", IsSelected = false }
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
    }
}
