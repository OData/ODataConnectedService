namespace Microsoft.OData.ConnectedService.Compatibility
{
    public enum LogLevel
    {
        /// <summary>
        /// The message can be shown to the user and is just for information. It is shown in the Output window and in the progress dialog.
        /// </summary>
        Information,
        /// <summary>
        /// The message is only to be used to help debug the application. It is shown only in the Output window.
        /// </summary>
        Debug,
        /// <summary>
        /// Something non-critical went wrong or the user should be aware of a possible issue,
        /// but the operation can continue. It is shown in the Output window and in the progress dialog.
        /// </summary>
        Warning,
        /// <summary>
        /// Something went wrong and the operation can not continue. It is shown in the Output window and in the progress dialog.
        /// </summary>
        Error,
    }
}
