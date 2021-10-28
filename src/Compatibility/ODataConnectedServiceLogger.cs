using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    class ODataConnectedServiceLogger: IConnectedServiceCompatibilityLogger
    {
        public ConnectedServiceLogger Logger { get; set; }

        public ODataConnectedServiceLogger(ConnectedServiceLogger logger)
        {
            this.Logger = logger;
        }

        public Task WriteMessageAsync(LogLevel level, string message, params object[] args)
        {
            var logLevel = (LoggerMessageCategory) Enum.Parse(typeof(LoggerMessageCategory), level.ToString());
            return this.Logger.WriteMessageAsync(logLevel, message, args);
        }
    }
}
