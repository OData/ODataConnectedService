// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Web.OData.Design.Scaffolding.Metadata;
using System.Web.OData.Design.Scaffolding.Scaffolders;
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

    //public class ODataControllerWithActionsScaffolder : CodeGenerator
    //{
    //    CustomViewModel _viewModel;

    //    /// <summary>
    //    /// Constructor for the custom code generator
    //    /// </summary>
    //    /// <param name="context">Context of the current code generation operation based on how scaffolder was invoked(such as selected project/folder) </param>
    //    /// <param name="information">Code generation information that is defined in the factory class.</param>
    //    public ODataControllerWithActionsScaffolder(
    //        CodeGenerationContext context,
    //        CodeGeneratorInformation information)
    //        : base(context, information)
    //    {
    //        _viewModel = new CustomViewModel(Context);
    //    }

    //    // accessible for unit tests
    //    protected internal string TemplateFolderName
    //    {
    //        get
    //        {
    //            return "ODataControllerWithActions";
    //        }
    //    }

    //    public override IEnumerable<string> TemplateFolders
    //    {
    //        get
    //        {
    //            return GetSearchFolders(TemplateFolderName);
    //        }
    //    }

    //    /// <summary>
    //    /// Any UI to be displayed after the scaffolder has been selected from the Add Scaffold dialog.
    //    /// Any validation on the input for values in the UI should be completed before returning from this method.
    //    /// </summary>
    //    /// <returns></returns>
    //    public override bool ShowUIAndValidate()
    //    {
    //        // Bring up the selection dialog and allow user to select a model type
    //        SelectModelWindow window = new SelectModelWindow(_viewModel);
    //        bool? showDialog = window.ShowDialog();
    //        return showDialog ?? false;
    //    }

    //    /// <summary>
    //    /// This method is executed after the ShowUIAndValidate method, and this is where the actual code generation should occur.
    //    /// In this example, we are generating a new file from t4 template based on the ModelType selected in our UI.
    //    /// </summary>
    //    public override void GenerateCode()
    //    {
    //        // Get the selected code type
    //        var codeType = _viewModel.SelectedModelType.CodeType;

    //        // Setup the scaffolding item creation parameters to be passed into the T4 template.
    //        var parameters = new Dictionary<string, object>()
    //        {
    //            { "ModelType", codeType},
    //            { "ModelMetadata", new CodeModelModelMetadata(codeType)},
    //        };

    //        // Add the custom scaffolding item from T4 template.
    //        this.AddFileFromTemplate(Context.ActiveProject,
    //            "CustomCode",
    //            "Controller",
    //            parameters,
    //            skipIfExists: false);
    //    }

    //    protected string[] GetSearchFolders(string templateFolderName)
    //    {
    //        if (templateFolderName == null)
    //        {
    //            throw new ArgumentNullException("templateFolderName");
    //        }

    //        return (new string[]
    //        {
    //            Path.Combine(TemplateSearchDirectories.GetProjectTemplateRoot(Context.ActiveProject), templateFolderName),
    //            Path.Combine(TemplateSearchDirectories.InstalledTemplateRoot, templateFolderName),
    //        }).Where(folder => Directory.Exists(folder)).ToArray();
    //    }


    //}
}
