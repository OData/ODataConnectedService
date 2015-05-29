using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.ConnectedService.Common
{
    public static class ProcessHelper
    {
        public static void ExecuteCommand(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "cmd";
            startInfo.Arguments = arguments;
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
    }
}
