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

            if (codeType is CodeClass2)
            {
                var cc = (CodeClass2)codeType;
                if ((cc.IsShared) && cc.Name.EndsWith("Config"))
                {
                    foreach (CodeElement ce in cc.Children)
                    {
                        if (ce.Name.EndsWith("Register") && ce is CodeFunction)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
