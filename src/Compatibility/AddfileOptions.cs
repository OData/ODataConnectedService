

using System.Collections.Generic;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    public class AddFileOptions
    {
        /// <summary>
        /// Gets or sets a dictionary of key/value pairs that will be used to replace tokens in the file.
        /// These values are additional to the values in TokenReplacementValues.  In the case of conflicts, the
        /// values specified in AdditionalReplacementValues will override the TokenReplacementValues.
        /// </summary>
        public IDictionary<string, string> AdditionalReplacementValues { get; }
        /// <summary>
        /// Gets or sets a value indicating whether to suppress prompting the end user if an existing file
        /// is detected and should be overwritten.  The default is false
        /// </summary>
        public bool SuppressOverwritePrompt { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the file should be opened after being added.  The default is false
        /// </summary>
        public bool OpenOnComplete { get; set; }
    }
}
