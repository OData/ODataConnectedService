//-----------------------------------------------------------------------------
// <copyright file="ODataCliMessageLogger.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Logging;

namespace Microsoft.OData.Cli
{
    /// <summary>
    /// An implementation of the <see cref="IMessageLogger"/>
    /// </summary>
    public class ODataCliMessageLogger : IMessageLogger
    {
        private readonly IConsole console;

        /// <summary>
        /// Creates an instance of <see cref="ODataCliMessageLogger"/>
        /// </summary>
        /// <param name="console">IConsole property to use for logging</param>
        public ODataCliMessageLogger(IConsole console)
        {
            this.console = console;
        }

        /// <summary>
        /// Writes a message to the log
        /// </summary>
        /// <param name="logMessageCategory">The severity level of the message to be logged</param>
        /// <param name="message">The message to log</param>
        /// <param name="args">It contains any other object to format</param>
        /// <returns>A completed Task</returns>
        public Task WriteMessageAsync(LogMessageCategory logMessageCategory, string message, params object[] args)
        {
            if (logMessageCategory == LogMessageCategory.Error)
            {
                this.console.Error.Write(message + "\n");
            }
            else if (logMessageCategory == LogMessageCategory.Information)
            {
                string strMsg = string.Format(message, args.Length > 0 ? args[0].ToString() : string.Empty, args.Length > 1 ? args[1].ToString() : string.Empty);
                this.console.Out.Write($"{strMsg} \n");
            }

            return Task.CompletedTask;
        }
    }
}
