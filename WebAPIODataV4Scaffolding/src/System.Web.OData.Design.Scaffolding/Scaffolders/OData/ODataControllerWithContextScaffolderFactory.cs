// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
    [Export(typeof(CodeGeneratorFactory))]
    internal class ODataControllerWithContextScaffolderFactory : ScaffolderFactory<ODataFrameworkDependency>
    {
        private static CodeGeneratorInformation _info = new CodeGeneratorInformation(
            displayName: Resources.ScaffoldODataContext_Name,
            description: Resources.ScaffoldODataContext_Description,
            author: Resources.Scaffold_Auther,
            version: ScaffolderVersions.WebApiODataScaffolderVersion,
            id: Resources.ScaffoldODataContext_Id,
            icon: ToImageSource(Resources._TemplateIconSample),
            gestures: new[] { ScaffoldingGestures.Controller },
            categories: new[] { Categories.Common, Categories.MvcController, Categories.WebApi });

        public ODataControllerWithContextScaffolderFactory()
            : base(_info)
        {
        }

        protected override ICodeGenerator CreateInstanceInternal(CodeGenerationContext context)
        {
            return new ODataControllerWithContextScaffolder(context, Information);
        }
    }
}
