// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding
{
    public class ViewTemplate : IEquatable<ViewTemplate>
    {
        public ViewTemplate(string name, bool isModelRequired)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Resources.EmptyTemplateName);
            }

            Name = name;
            IsModelRequired = isModelRequired;

            if (IsModelRequired)
            {
                DisplayName = Name;
            }
            else
            {
                DisplayName = Name + " " + Resources.ViewTemplateWithoutModel;
            }
        }

        public string DisplayName { get; private set; }

        public bool IsModelRequired { get; private set; }

        public string Name { get; private set; }

        public bool Equals(ViewTemplate other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                // Name is based on the filename, so case-insensitive comparison is used
                return
                    String.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                    IsModelRequired == other.IsModelRequired;
            }
        }

        public override bool Equals(object obj)
        {
            ViewTemplate other = obj as ViewTemplate;
            if (other == null)
            {
                return false;
            }
            else
            {
                return Equals(other);
            }
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Name) ^ IsModelRequired.GetHashCode();
        }
    }
}
