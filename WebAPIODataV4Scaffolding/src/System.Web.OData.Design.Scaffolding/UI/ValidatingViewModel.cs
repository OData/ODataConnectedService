// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Web.OData.Design.Scaffolding.UI
{
    public class ValidatingViewModel : NotifyPropertyChanged, IDataErrorInfo
    {
        private bool _isValid;

        public ValidatingViewModel()
        {
            ErrorMessages = new Dictionary<string, string>();
            IsValid = true;
        }

        private Dictionary<string, string> ErrorMessages
        {
            get;
            set;
        }

        public string Error
        {
            get
            {
                if (ErrorMessages.Any())
                {
                    return ErrorMessages.First().Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == null)
                {
                    throw new ArgumentNullException("columnName");
                }

                string message;
                ErrorMessages.TryGetValue(columnName, out message);
                return message;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            private set
            {
                OnPropertyChanged<bool>(ref _isValid, value);
            }
        }

        protected static void DisplayErrorMessage(IDialogHost dialogHost, string errorMessage)
        {
            if (dialogHost == null)
            {
                throw new ArgumentNullException("dialogHost");
            }

            dialogHost.ShowErrorMessage(
                            errorMessage,
                            caption: null);
        }

        /// <summary>
        /// Sets the validation message for the given property. Set the message to null to clear a message
        /// for the property.
        /// </summary>
        /// <param name="message">The message, or null.</param>
        /// <param name="propertyName">The property name.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Using a default value is required for CallerMemberName")]
        protected void SetValidationMessage(string message, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            if (message == null)
            {
                ErrorMessages.Remove(propertyName);
                if (!ErrorMessages.Any())
                {
                    IsValid = true;
                }
            }
            else if (message != null)
            {
                ErrorMessages[propertyName] = message;
                IsValid = false;
            }
        }
    }
}
