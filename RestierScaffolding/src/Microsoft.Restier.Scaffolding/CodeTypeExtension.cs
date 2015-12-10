// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Globalization;
    using EnvDTE;
    using EnvDTE80;

    internal static class CodeTypeExtension
    {
        public static bool IsValidConfigType(this CodeType codeType)
        {
            if (codeType == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.ArgumentNullOrEmpty, "codeType"));
            }

            var cc = codeType as CodeClass2;
            if (cc != null && cc.IsShared && cc.Name.EndsWith("Config", StringComparison.Ordinal))
            {
                foreach (CodeElement ce in cc.Children)
                {
                    if (ce.Name.EndsWith("Register", StringComparison.Ordinal) && ce is CodeFunction)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
