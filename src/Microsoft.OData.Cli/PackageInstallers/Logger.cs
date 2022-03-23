//-----------------------------------------------------------------------------
// <copyright file="Logger.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Logging;
using NuGet.Common;

namespace Microsoft.OData.Cli.PackageInstallers
{
    /// <summary>
    /// An implementation of the <see cref="ILogger"/> interface to use in logging
    /// </summary>
    internal class Logger : ILogger
    {
        private List<string> logs = new List<string>();
        private IMessageLogger messageLogger;

        /// <summary>
        /// Creates an instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="messageLogger">An instance of the <see cref="IMessageLogger"/> to use for logging.</param>
        internal Logger(IMessageLogger messageLogger)
        { 
            this.messageLogger = messageLogger;
        }

        public void LogDebug(string data)
        {
            logs.Add(data);
        }

        public void LogVerbose(string data)
        {
            logs.Add(data);
        }

        public void LogInformation(string data)
        {
            logs.Add(data);
        }

        public void LogMinimal(string data)
        {
            logs.Add(data);
        }

        public void LogWarning(string data)
        {
            logs.Add(data);
        }

        public void LogError(string data)
        {
            logs.Add(data);
        }

        public void LogInformationSummary(string data)
        {
            logs.Add(data);
        }

        public void LogErrorSummary(string data)
        {
            logs.Add(data);
        }

        public void Log(LogLevel level, string data)
        {
            this.Log(level, data);
        }

        public async Task LogAsync(LogLevel level, string data)
        {
            await this.messageLogger.WriteMessageAsync((LogMessageCategory)level, data);
        }

        public void Log(ILogMessage message)
        {
            this.Log(message);
        }

        public async Task LogAsync(ILogMessage message)
        {
            await this.LogAsync(message).ConfigureAwait(false);
        }
    }
}
