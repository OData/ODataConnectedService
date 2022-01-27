//-----------------------------------------------------------------------------
// <copyright file="IMessageLogger.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.OData.CodeGen.Logging
{
    /// <summary>
    /// Message logging interface.
    /// </summary>
    public interface IMessageLogger
    {
        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="logMessageCategory">The severity level of the message to be logged</param>
        /// <param name="message">The message to log</param>
        /// <param name="args">It contains any other object to format</param>
        /// </summary
        Task WriteMessageAsync(LogMessageCategory logMessageCategory, string message, params object[] args);
    }
}
