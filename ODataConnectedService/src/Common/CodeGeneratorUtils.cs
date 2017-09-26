// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.Win32;

namespace Microsoft.OData.ConnectedService.Common
{
    internal static class CodeGeneratorUtils
    {
        public const string InstallLocationSubKeyName = "InstallLocation";

        /// <summary>
        /// Try to get the location of the installed WCF Data Service.
        /// </summary>
        /// <returns>Returns the location of the installed WCF Data Service if it exists, else returns empty string.</returns>
        public static string GetWCFDSInstallLocation()
        {
            string dataFxRegistryPath = 8 == IntPtr.Size
                ? @"SOFTWARE\Wow6432Node\Microsoft\Microsoft WCF Data Services\VS 2014 Tooling\"
                : @"SOFTWARE\Microsoft\Microsoft WCF Data Services\VS 2014 Tooling\";
            using (RegistryKey dataFxKey = Registry.LocalMachine.OpenSubKey(dataFxRegistryPath))
            {
                if (dataFxKey != null)
                {
                    string runtimePath = (string)dataFxKey.GetValue(InstallLocationSubKeyName);
                    if (!string.IsNullOrEmpty(runtimePath))
                    {
                        return runtimePath;
                    }
                }
            }

            return String.Empty;
        }
    }
}
