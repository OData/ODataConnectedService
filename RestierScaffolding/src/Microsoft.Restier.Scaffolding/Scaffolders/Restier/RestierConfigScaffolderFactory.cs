// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System.ComponentModel.Composition;
    using Microsoft.AspNet.Scaffolding;

    [Export(typeof(CodeGeneratorFactory))]
    internal class RestierConfigScaffolderFactory : ScaffolderFactory<ODataFrameworkDependency>
    {
        private static CodeGeneratorInformation _info = new CodeGeneratorInformation(
            displayName: Resources.RestierScaffolderName,
            description: Resources.RestierScaffolderDescription,
            author: Resources.Scaffold_Auther,
            version: ScaffolderVersions.RestierScaffolderVersion,
            id: Resources.RestierScaffolderId,
            icon: ToImageSource(Resources._TemplateIconSample),
            gestures: new[] { ScaffoldingGestures.Config },
            categories: new[] { Categories.WebApi });

        public RestierConfigScaffolderFactory()
            : base(_info)
        {
        }

        protected override ICodeGenerator CreateInstanceInternal(CodeGenerationContext context)
        {
            return new RestierConfigScaffolder(context, Information);
        }
    }
}
