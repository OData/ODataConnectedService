// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    public abstract class ScaffolderFactory<TFramework> : CodeGeneratorFactory
        where TFramework : IFrameworkDependency
    {
        protected ScaffolderFactory(CodeGeneratorInformation information)
            : base(information)
        {
        }

        [Import]
        private TFramework Framework
        {
            get;
            set;
        }

        [Import]
        private INuGetRepository Repository
        {
            get;
            set;
        }

        [Import]
        private IVisualStudioIntegration VisualStudioIntegration
        {
            get;
            set;
        }

        public sealed override ICodeGenerator CreateInstance(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!context.Items.ContainsProperty(typeof(TFramework)))
            {
                context.Items.AddProperty(typeof(TFramework), Framework);
            }

            if (!context.Items.ContainsProperty(typeof(INuGetRepository)))
            {
                context.Items.AddProperty(typeof(INuGetRepository), Repository);
            }

            if (!context.Items.ContainsProperty(typeof(IVisualStudioIntegration)))
            {
                context.Items.AddProperty(typeof(IVisualStudioIntegration), VisualStudioIntegration);
            }

            return CreateInstanceInternal(context);
        }

        protected abstract ICodeGenerator CreateInstanceInternal(CodeGenerationContext context);

        public override bool IsSupported(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.ActiveProject.CodeModel.Language != EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp)
            {
                return false;
            }

            return Framework.IsSupported(context);
        }

        public static ImageSource ToImageSource(Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}
