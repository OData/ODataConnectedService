//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceMessageLogger.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    /// <summary>
    /// An implementation of the <see cref="IMessageLogger"/>
    /// </summary>
    public class ConnectedServiceMessageLogger : IMessageLogger
    {
        private ConnectedServiceHandlerContext Context;

        /// <summary>
        /// Creates an instance of <see cref="ConnectedServiceMessageLogger"/>
        /// </summary>
        /// <param name="context"></param>
        public ConnectedServiceMessageLogger(ConnectedServiceHandlerContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Writes a message to the log
        /// </summary>
        /// <param name="logMessageCategory">The severity level of the message to be logged</param>
        /// <param name="message">The message to log</param>
        /// <param name="args">It contains any other object to format</param>
        /// <returns></returns>
        public async Task WriteMessageAsync(LogMessageCategory logMessageCategory, string message, params object[] args)
        {
            await Context.Logger.WriteMessageAsync((LoggerMessageCategory)logMessageCategory, message, args).ConfigureAwait(false);
        }
    }
}
