// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.AspNet.Scaffolding.EntityFramework;
    using Microsoft.AspNet.Scaffolding.NuGet;
    using Microsoft.Restier.Scaffolding.UI;

    public abstract class ConfigScaffolder<TFramework> : InteractiveScaffolder<ConfigScaffolderModel, TFramework>
        where TFramework : IFrameworkDependency
    {
        protected ConfigScaffolder(CodeGenerationContext context, CodeGeneratorInformation information)
            : base(context, information)
        {
        }

        protected sealed override ConfigScaffolderModel CreateModel()
        {
            ConfigScaffolderModel model = new ConfigScaffolderModel(Context);
            // model.ConfigName = model.GetModifiedName(MvcProjectUtil.ConfigName, model.CodeFileExtension);
            model.IsDataContextSupported = true;
            return model;
        }

        protected override ValidatingDialogWindow CreateDialog()
        {
            return new ConfigScaffolderDialog();
        }

        protected override object CreateViewModel(ConfigScaffolderModel model)
        {
            return new ConfigScaffolderViewModel(model);
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is an internal API.")]
        protected internal void AddScaffoldDependencies(List<NuGetPackage> packages)
        {
            base.AddScaffoldDependencies(packages);

            IEntityFrameworkService efService = Context.ServiceProvider.GetService<IEntityFrameworkService>();
            packages.AddRange(efService.Dependencies);
        }
    }
}
