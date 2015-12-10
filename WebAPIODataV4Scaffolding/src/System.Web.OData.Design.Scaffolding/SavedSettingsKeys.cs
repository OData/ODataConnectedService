// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding
{
    internal static class SavedSettingsKeys
    {
        private const string Prefix = "WebStackScaffolding_";

        public const string IsLayoutPageSelectedKey = Prefix + "IsLayoutPageSelected";
        public const string IsReferencingScriptLibrariesSelectedKey = Prefix + "IsReferencingScriptLibrariesSelected";
        public const string IsPartialViewSelectedKey = Prefix + "IsPartialViewSelected";
        public const string IsAsyncSelectedKey = Prefix + "IsAsyncSelected";
        public const string LayoutPageFileKey = Prefix + "LayoutPageFile";
        public const string IsViewGenerationSelectedKey = Prefix + "IsViewGenerationSelected";
        public const string DbContextTypeFullNameKey = Prefix + "DbContextTypeFullName";
        public const string AreaDialogWidthKey = Prefix + "AreaDialogWidth";
        public const string ControllerDialogWidthKey = Prefix + "ControllerDialogWidth";
        public const string ViewDialogWidthKey = Prefix + "ViewDialogWidth";
        public const string DbContextDialogWidthKey = Prefix + "DbContextDialogWidth";
        public const string DependencyDialogWidthKey = Prefix + "DependencyDialogWidth";
    }
}
