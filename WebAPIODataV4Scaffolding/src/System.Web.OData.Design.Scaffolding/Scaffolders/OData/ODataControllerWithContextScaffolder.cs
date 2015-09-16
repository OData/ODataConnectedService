// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.Core.Metadata;


namespace System.Web.OData.Design.Scaffolding
{
    public class ODataControllerWithContextScaffolder : ControllerWithContextScaffolder<ODataFrameworkDependency>
    {
        public ODataControllerWithContextScaffolder(CodeGenerationContext context, CodeGeneratorInformation information)
            : base(context, information)
        {
        }

        protected internal override string TemplateFolderName
        {
            get
            {
                return "ODataControllerWithContext";
            }
        }

        protected override IDictionary<string, object> AddTemplateParameters(CodeType dbContextType, ModelMetadata modelMetadata)
        {
            IDictionary<string, object> parameters = base.AddTemplateParameters(dbContextType, modelMetadata);
            parameters.Add("ODataModificationMessage", Resources.ScaffoldODataModificationMessage);
            parameters.Add("IsLegacyOdataVersion", Framework.IsODataLegacy(Context));
            return parameters;
        }
    }
}
