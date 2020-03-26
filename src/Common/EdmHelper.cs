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
        public static IEdmModel GetEdmModelFromFile(string path, ConnectedServiceContext context = null)
        {
            var xmlSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            var reader = XmlReader.Create(path, xmlSettings);

            var result = CsdlReader.TryParse(reader, true /* ignoreUnexpectedAttributes */, out var model, out var errors);

            if (result)
            {
                return model;
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

        public static IEnumerable<IEdmEntityType> GetEntityTypes(IEdmModel model)
        {
            var entityTypes = model.SchemaElements.OfType<IEdmEntityType>();

            foreach (var entityType in entityTypes)
            {
                    yield return entityType;
            }
        }
    }
}
