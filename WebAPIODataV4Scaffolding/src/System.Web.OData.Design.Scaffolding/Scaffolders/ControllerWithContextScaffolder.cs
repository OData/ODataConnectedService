// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.Core.Metadata;
using Microsoft.AspNet.Scaffolding.EntityFramework;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace System.Web.OData.Design.Scaffolding
{
    public abstract class ControllerWithContextScaffolder<TFramework> : ControllerScaffolder<TFramework>
        where TFramework : IFrameworkDependency
    {
        protected ControllerWithContextScaffolder(CodeGenerationContext context, CodeGeneratorInformation information)
            : base(context, information)
        {
        }

        protected override void OnModelCreated(ControllerScaffolderModel model)
        {
            base.OnModelCreated(model);
            model.ControllerName = null;
            model.IsModelClassSupported = true;
            model.IsDataContextSupported = true;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is an internal API.")]
        protected internal override void AddScaffoldDependencies(List<NuGetPackage> packages)
        {
            base.AddScaffoldDependencies(packages);

            IEntityFrameworkService efService = Context.ServiceProvider.GetService<IEntityFrameworkService>();
            packages.AddRange(efService.Dependencies);
        }

        protected internal override void Scaffold()
        {
            try
            {
                GenerateContextAndController();
                tc.TrackEvent(TelemetryEventNames.AddControllerWithContext);
            }
            catch (Exception e)
            {
                tc.TrackEvent(TelemetryEventNames.ContextScaffolderFailure);
                tc.TrackException(e);
                throw;
            }
            finally
            {
                // We want to record SQM data even on failure
                Framework.RecordControllerTelemetryOptions(Context, Model);
                tc.Flush();
            }
        }

        protected ModelMetadata GenerateContextAndController()
        {
            string modelTypeName = Model.ModelType.TypeName;
            string dbContextTypeName = Model.DataContextType.TypeName;

            // First Scaffold the DB Context
            IEntityFrameworkService efService = Context.ServiceProvider.GetService<IEntityFrameworkService>();

            ModelMetadata modelMetadata = efService.AddRequiredEntity(Context, dbContextTypeName, modelTypeName);

            // After the above step the dbContext must have been created.
            CodeType dbContextType = Context.ServiceProvider.GetService<ICodeTypeService>().GetCodeType(Context.ActiveProject, dbContextTypeName);

            IDictionary<string, object> templateParameters = AddTemplateParameters(dbContextType, modelMetadata);

            // scaffold the controller
            GenerateController(templateParameters);
            return modelMetadata;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "We're using this to lowercase a string for generated code.")]
        protected virtual IDictionary<string, object> AddTemplateParameters(
            CodeType dbContextType,
            ModelMetadata modelMetadata)
        {
            if (dbContextType == null)
            {
                throw new ArgumentNullException("dbContextType");
            }

            if (modelMetadata == null)
            {
                throw new ArgumentNullException("modelMetadata");
            }

            if (String.IsNullOrEmpty(Model.ControllerName))
            {
                throw new InvalidOperationException(Resources.InvalidControllerName);
            }

            IDictionary<string, object> templateParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CodeType modelType = Model.ModelType.CodeType;
            templateParameters.Add("ModelMetadata", modelMetadata);

            string modelTypeNamespace = modelType.Namespace != null ? modelType.Namespace.FullName : String.Empty;
            templateParameters.Add("ModelTypeNamespace", modelTypeNamespace);

            HashSet<string> requiredNamespaces = GetRequiredNamespaces(new List<CodeType>() { modelType, dbContextType });
            templateParameters.Add("RequiredNamespaces", requiredNamespaces);
            templateParameters.Add("ModelTypeName", modelType.Name);
            templateParameters.Add("ContextTypeName", dbContextType.Name);
            templateParameters.Add("UseAsync", Model.IsAsyncSelected);

            CodeDomProvider provider = ValidationUtil.GenerateCodeDomProvider(Model.ActiveProject.GetCodeLanguage());
            string modelVariable = provider.CreateEscapedIdentifier(Model.ModelType.ShortTypeName.ToLowerInvariantFirstChar());
            templateParameters.Add("ModelVariable", modelVariable);

            templateParameters.Add("EntitySetVariable", modelMetadata.EntitySetName.ToLowerInvariantFirstChar());

            // Overposting protection is only for MVC - and only when the model doesn't already have [Bind]
            if (Model.IsViewGenerationSupported)
            {
                bool isOverpostingProtectionRequired = OverpostingProtection.IsOverpostingProtectionRequired(modelType);
                templateParameters.Add("IsOverpostingProtectionRequired", isOverpostingProtectionRequired);

                if (isOverpostingProtectionRequired)
                {
                    templateParameters.Add("OverpostingWarningMessage", OverpostingProtection.WarningMessage);
                    templateParameters.Add("BindAttributeIncludeText", OverpostingProtection.GetBindAttributeIncludeText(modelMetadata));
                }
            }

            return templateParameters;
        }
    }
}
