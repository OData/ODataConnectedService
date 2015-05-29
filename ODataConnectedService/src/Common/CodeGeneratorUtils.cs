using System;
using Microsoft.Win32;

namespace Microsoft.OData.ConnectedService.Common
{
    public static class CodeGeneratorUtils
    {
        internal const string InstallLocationSubKeyName = "InstallLocation";
        internal static string GetWCFDSInstallLocation()
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

            return null;
        }
    }
}
