using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.OData.ConnectedService.Common
{
    internal class EdmHelper
    {
        private static IDictionary<IEdmStructuredType, List<IEdmOperation>> _boundOperations = null;

        private static IEdmModel _model = null;

        public static IEdmModel GetEdmModelFromFile(string path, ConnectedServiceContext context = null)
        {

            if(_model != null)
            {
                return _model;
            }

            var xmlSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            var reader = XmlReader.Create(path, xmlSettings);

            var result = CsdlReader.TryParse(reader, true /* ignoreUnexpectedAttributes */, out  _model, out var errors);

            if (result)
            {
                return _model;
            } 

            if (context != null)
            {
                foreach (var error in errors)
                {
                    var task = context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning,
                        error.ErrorMessage);
                    task.RunSynchronously();
                }
            }

            return null;
        }

        public static IEnumerable<IEdmOperationImport> GetOperationImports(IEdmModel model)
        {
            var containers = model.SchemaElements.OfType<IEdmEntityContainer>();
            foreach (var container in containers)
            {
                foreach (var operation in container.OperationImports())
                {
                    yield return operation;
                }
            }
        }

        public static IDictionary<IEdmStructuredType,List<IEdmOperation>> GetBoundOperations(IEdmModel model)
        {
            if(_boundOperations != null)
            {
                return _boundOperations;
            }

            _boundOperations = new Dictionary<IEdmStructuredType, List<IEdmOperation>>();
            foreach (IEdmOperation operation in model.SchemaElements.OfType<IEdmOperation>())
            {
                if (operation.IsBound)
                {
                    IEdmType edmType = operation.Parameters.First().Type.Definition;
                    if (edmType is IEdmStructuredType edmStructuredType)
                    {
                        if (!_boundOperations.TryGetValue(edmStructuredType, out List<IEdmOperation> operations))
                        {
                            operations = new List<IEdmOperation>();
                        }

                        operations.Add(operation);
                        _boundOperations[edmStructuredType] = operations;
                    }
                }
            }

            return _boundOperations;
        }

        public static IEnumerable<IEdmSchemaType> GetEntityTypes(IEdmModel model)
        {
            var entityTypes = model.SchemaElements.OfType<IEdmSchemaType>();

            foreach (var entityType in entityTypes)
            {
                    yield return entityType;
            }
        }

        public static string GetTypeNameFromFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            string[] nameArr = fullName.Split('.');

            return nameArr[nameArr.Length - 1];
        }
    }
}
