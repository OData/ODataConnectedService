// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Web.OData.Design.Scaffolding.Metadata;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.Core.Metadata;
using Microsoft.AspNet.Scaffolding.EntityFramework;

namespace System.Web.OData.Design.Scaffolding
{
    public class ODataControllerWithActionsScaffolder : ControllerScaffolder<ODataFrameworkDependency>
    {
        public ODataControllerWithActionsScaffolder(CodeGenerationContext context, CodeGeneratorInformation information)
            : base(context, information)
        {
        }

        protected internal override string TemplateFolderName
        {
            get
            {
                return "ODataControllerWithActions";
            }
        }

        protected override void OnModelCreated(ControllerScaffolderModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            base.OnModelCreated(model);
            model.ControllerName = null;

            // Async is always supported because ODataControllerWithActions does not depend on Entity framework.
            model.IsAsyncSupported = true;
            model.IsModelClassSupported = true;
        }

        protected override void AddTemplateParameters(IDictionary<string, object> templateParameters)
        {
            base.AddTemplateParameters(templateParameters);

            CodeType codeType = Model.ModelType.CodeType;
            ModelMetadata metadata = new CodeModelModelMetadata(codeType);
            if (metadata.PrimaryKeys.Length == 0)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.NoKeyDefinedError, codeType.Name));
            }
            templateParameters.Add("ModelMetadata", metadata);

            HashSet<string> requiredNamespaces = GetRequiredNamespaces(new List<CodeType>() { codeType });
            templateParameters.Add("RequiredNamespaces", requiredNamespaces);

            string modelTypeNamespace = codeType.Namespace != null ? codeType.Namespace.FullName : String.Empty;
            templateParameters.Add("ModelTypeNamespace", modelTypeNamespace);
            templateParameters.Add("ModelTypeName", codeType.Name);
            templateParameters.Add("UseAsync", Model.IsAsyncSelected);

            IEntityFrameworkService efService = Context.ServiceProvider.GetService<IEntityFrameworkService>();
            string entitySetName = efService.GetPluralizedWord(codeType.Name, CultureInfo.InvariantCulture);
            templateParameters.Add("EntitySetName", entitySetName);

            CodeDomProvider provider = ValidationUtil.GenerateCodeDomProvider(Model.ActiveProject.GetCodeLanguage());
            string modelVariable = provider.CreateEscapedIdentifier(codeType.Name.ToLowerInvariantFirstChar());
            string entitySetVariable = provider.CreateEscapedIdentifier(entitySetName.ToLowerInvariantFirstChar());
            templateParameters.Add("ModelVariable", modelVariable);
            templateParameters.Add("EntitySetVariable", entitySetVariable);

            templateParameters.Add("ODataModificationMessage", Resources.ScaffoldODataModificationMessage);
            templateParameters.Add("IsLegacyOdataVersion", Framework.IsODataLegacy(Context));
        }
    }
}
