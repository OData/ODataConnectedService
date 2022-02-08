//-----------------------------------------------------------------------------
// <copyright file="EdmHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Common
{
    public class EdmHelper
    {
        private static IDictionary<IEdmType, List<IEdmOperation>> _boundOperations = null;

        /// <summary>
        /// Gets all the operation imports in the model
        /// </summary>
        /// <param name="path">Edmx file path.</param>
        /// <param name="context">ConnectedServiceContext object.</param>
        /// <returns>Edm model</returns>
        public static IEdmModel GetEdmModelFromFile(string path, ConnectedServiceContext context = null)
        {
            var xmlSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using (var reader = XmlReader.Create(path, xmlSettings))
            {
                try
                {
                    var result = CsdlReader.TryParse(reader, true /* ignoreUnexpectedAttributes */, out var model, out var errors);
                    if (result)
                    {
                        return model;
                    }

                    if (context != null)
                    {
                        foreach (var error in errors)
                        {
                            var task = context.Logger?.WriteMessageAsync(LoggerMessageCategory.Warning,
                                error.ErrorMessage);
                            task?.RunSynchronously();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (context != null)
                    {
                        var task = context.Logger?.WriteMessageAsync(LoggerMessageCategory.Warning, ex.Message);
                        task?.RunSynchronously();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all the operation imports in the model
        /// </summary>
        /// <param name="model">Edm model.</param>
        /// <returns>A list of operation imports</returns>
        public static IEnumerable<IEdmOperationImport> GetOperationImports(IEdmModel model)
        {
            var containers = model?.SchemaElements?.OfType<IEdmEntityContainer>() ?? Array.Empty<IEdmEntityContainer>();
            foreach (var container in containers)
            {
                foreach (var operation in container.OperationImports())
                {
                    yield return operation;
                }
            }
        }

        /// <summary>
        /// Gets all the bound operations associated with all specific types
        /// </summary>
        /// <param name="model">Edm model.</param>
        /// <returns>a dictionary of types mapped to a list of bound operations</returns>
        public static IDictionary<IEdmType, List<IEdmOperation>> GetBoundOperations(IEdmModel model)
        {
            _boundOperations = new Dictionary<IEdmType, List<IEdmOperation>>();
            if (model == null)
            {
                return _boundOperations;
            }

            foreach (IEdmOperation operation in model.SchemaElements.OfType<IEdmOperation>())
            {
                if (operation.IsBound)
                {
                    IEdmType edmType = operation.Parameters.First().Type.Definition;

                    if (!_boundOperations.TryGetValue(edmType, out List<IEdmOperation> operations))
                    {
                        operations = new List<IEdmOperation>();
                    }

                    operations.Add(operation);
                    _boundOperations[edmType] = operations;
                }
            }

            return _boundOperations;
        }

        /// <summary>
        /// Gets the name of the type without the namespace
        /// </summary>
        /// <param name="model">Edm model.</param>
        /// <returns>All schema types in the model</returns>
        public static IEnumerable<IEdmSchemaType> GetSchemaTypes(IEdmModel model)
        {
            var schemaTypes = model?.SchemaElements?.OfType<IEdmSchemaType>() ?? Array.Empty<IEdmSchemaType>();

            foreach (var schemaType in schemaTypes)
            {
                yield return schemaType;
            }
        }

        /// <summary>
        /// Gets the name of the type without the namespace
        /// </summary>
        /// <param name="fullName">Full type name with namespace.</param>
        /// <returns>A schema type name without the namespace</returns>
        public static string GetTypeNameFromFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            string[] nameArr = fullName.Split('.');

            return nameArr[nameArr.Length - 1];
        }

        /// <summary>
        /// Gets the operation parameters string like '(Type1 par1, Type2 par2)'
        /// </summary>
        /// <param name="parameters">Collection of parameters.</param>
        /// <returns>An operation parameters string like '(Type1 par1, Type2, par2)'</returns>
        public static string GetParametersString(IEnumerable<IEdmOperationParameter> parameters)
        {
            var result = new StringBuilder("(");
            var isFirst = true;
            foreach (var parameter in parameters)
            {
                if (!isFirst)
                {
                    result.Append(", ");
                }
                else
                {
                    isFirst = false;
                }
                result.Append(parameter.Type?.Definition?.FullTypeName() ?? "void");
                result.Append($" {parameter.Name}");
            }

            result.Append(")");
            return result.ToString();
        }
    }
}
