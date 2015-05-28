// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
    [Export(typeof(CodeGeneratorFactory))]
    public class ODataControllerWithActionsScaffolderFactory : ScaffolderFactory<ODataFrameworkDependency>
    {
        private static CodeGeneratorInformation _info = new CodeGeneratorInformation(
            displayName: Resources.ScaffoldODataActions_Name,
            description: Resources.ScaffoldODataActions_Description,
            author: Resources.Scaffold_Auther,
            version: ScaffolderVersions.WebApiODataScaffolderVersion,
            id: typeof(ODataControllerWithActionsScaffolder).Name,
            icon: ToImageSource(Resources._TemplateIconSample),
            gestures: new[] { ScaffoldingGestures.Controller },
            categories: new[] { Categories.Common, Categories.MvcController, Categories.WebApi });

        public ODataControllerWithActionsScaffolderFactory()
            : base(_info)
        {
        }

        protected override ICodeGenerator CreateInstanceInternal(CodeGenerationContext context)
        {
            return new ODataControllerWithActionsScaffolder(context, Information);
        }

        //public override ICodeGenerator CreateInstance(CodeGenerationContext context)
        //{
        //    return new ODataControllerWithActionsScaffolder(context, Information);
        //}

        //public override bool IsSupported(CodeGenerationContext codeGenerationContext)
        //{
        //    if (codeGenerationContext.ActiveProject.CodeModel.Language != EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}
