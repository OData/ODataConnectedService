//-----------------------------------------------------------------------------
// <copyright file="ODataFileOptions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

namespace Microsoft.OData.CodeGen.FileHandling
{
    /// <summary>
    /// The options that control the behavior when adding a file to a target path
    /// </summary>
    public class ODataFileOptions
    {
        /// <summary>
        /// Instantiates a new instance of the ODataFileOptions class.
        /// </summary>
        public ODataFileOptions()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress prompting the end user if
        /// an existing file is detected in the target path and should be overwritten. The default is false
        /// </summary>
        public bool SuppressOverwritePrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file should be opened after being
        /// added in the target path. The default is false
        /// </summary>
        public bool OpenOnComplete { get; set; }

    }
}
