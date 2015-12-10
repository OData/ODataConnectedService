// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnvDTE;
    using Microsoft.AspNet.Scaffolding.Core.Metadata;

    /// <summary>
    /// Support functions for adding overposting protection to MVC controllers that use EF via
    /// the bind attribute.
    /// </summary>
    internal static class OverpostingProtection
    {
        private const string BindAttributeTypeName = "System.Web.Mvc.BindAttribute";
        private const string OverpostingFWLink = "http://go.microsoft.com/fwlink/?LinkId=317598";

        public static string WarningMessage
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture, Resources.OverpostingWarningMessage, OverpostingFWLink);
            }
        }

        public static bool IsOverpostingProtectionRequired(CodeType model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return !model.Attributes
                    .OfType<CodeAttribute>()
                    .Any(a => String.Equals(a.FullName, BindAttributeTypeName, StringComparison.Ordinal));
        }

        public static string GetBindAttributeIncludeText(ModelMetadata model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            IEnumerable<string> propertyNames = model.Properties.Where(p => !p.IsAssociation).Select(p => p.PropertyName);
            return String.Join(",", propertyNames);
        }
    }
}
