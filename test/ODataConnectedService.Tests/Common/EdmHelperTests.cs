//-----------------------------------------------------------------------------------
// <copyright file="EdmHelperTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class EdmHelperTests
    {
        readonly string MetadataPath = Path.GetFullPath("TestMetadataCsdl.xml");
        [TestMethod]
        public void TestGetEdmFromFile()
        {
            var model = EdmHelper.GetEdmModelFromFile(MetadataPath);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestGetTypeNameFromFullName()
        {
            var employeeFullName = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee";
            var personFullName = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person";
            var personGenderFullName = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender";

            var employeeTypeName = EdmHelper.GetTypeNameFromFullName(employeeFullName);
            var personTypeName = EdmHelper.GetTypeNameFromFullName(personFullName);
            var personGenderTypeName = EdmHelper.GetTypeNameFromFullName(personGenderFullName);

            Assert.AreEqual(employeeTypeName, "Employee");
            Assert.AreEqual(personTypeName, "Person");
            Assert.AreEqual(personGenderTypeName, "PersonGender");
        }

        [TestMethod]
        public void TestGetSchemaTypes()
        {
            var model = EdmHelper.GetEdmModelFromFile(MetadataPath);
            var schemaTypes = EdmHelper.GetSchemaTypes(model);
            var actualSchemaTypes = new List<string>();

            foreach(var type in schemaTypes)
            {
                actualSchemaTypes.Add(type.FullName());
            }

            var expectedSchemaTypes = new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.City",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PublicTransportation",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip",
            };

            // Sort the Lists in ascending order
            expectedSchemaTypes.Sort();
            actualSchemaTypes.Sort();

            //In order to get compare lists you should use the CollectionAssert
            CollectionAssert.AreEqual(expectedSchemaTypes, actualSchemaTypes);
        }

        [TestMethod]
        public void TestGetOperationImports()
        {
            var model = EdmHelper.GetEdmModelFromFile(MetadataPath);
            var operationImports = EdmHelper.GetOperationImports(model);

            var actualOperationImports = new List<string>();

            foreach (var operation in operationImports)
            {
                actualOperationImports.Add(operation.Name);
            }

            var expectedOperationImports = new List<string>()
            {
                "GetNearestAirport",
                "GetPersonWithMostFriends",
                "ResetDataSource",
            };

            // Sort the Lists in ascending order
            expectedOperationImports.Sort();
            actualOperationImports.Sort();

            //In order to get compare lists you should use the CollectionAssert
            CollectionAssert.AreEqual(expectedOperationImports, actualOperationImports);
        }

        [TestMethod]
        public void TestGetBoundOperations()
        {
            var model = EdmHelper.GetEdmModelFromFile(MetadataPath);
            var boundOperations = EdmHelper.GetBoundOperations(model);

            var actualBoundOperations = new List<string>();

            foreach (var operation in boundOperations)
            {
                foreach(var boundValue in operation.Value)
                {
                    actualBoundOperations.Add(boundValue.FullName());
                }
            }

            var expectedBoundOperations = new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.GetFavoriteAirline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.GetFriendsTrips",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.UpdatePersonLastName",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.ShareTrip",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.GetInvolvedPeople"
            };

            // Sort the Lists in ascending order
            actualBoundOperations.Sort();
            expectedBoundOperations.Sort();

            //In order to get compare lists you should use the CollectionAssert
            CollectionAssert.AreEqual(expectedBoundOperations, actualBoundOperations);
        }

        [TestMethod]
        public void TestGetParametersString_WithEmptyParameters()
        {
            string actualResult = EdmHelper.GetParametersString(new List<IEdmOperationParameter>());

            string expectedResult = "()";

            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void TestGetParametersString_WithSingleParameter()
        {
            var typeReference = new EdmTypeReferenceForTest(new EdmSchemaTypeForTest("Type1", "Test"), false);
            List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>
            {
                new EdmOperationParameter(new EdmAction("Test", "Action", typeReference, false, new EdmPropertyPathExpression("TestPath")), "par1", typeReference)
            };
            string actualResult = EdmHelper.GetParametersString(parameters);

            string expectedResult = "(Test.Type1 par1)";

            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void TestGetParametersString_WithAFewParameters()
        {
            var typeReference = new EdmTypeReferenceForTest(new EdmSchemaTypeForTest("Type1", "Test"), false);
            List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>
            {
                new EdmOperationParameter(new EdmAction("Test", "Action", typeReference, false, new EdmPropertyPathExpression("TestPath")), "par1", typeReference),
                new EdmOperationParameter(new EdmAction("Test", "Action", typeReference, false, new EdmPropertyPathExpression("TestPath")), "par2", typeReference)
            };
            string actualResult = EdmHelper.GetParametersString(parameters);

            string expectedResult = "(Test.Type1 par1, Test.Type1 par2)";

            actualResult.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
