// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.Core.Metadata;

namespace System.Web.OData.Design.Scaffolding.Metadata
{
    /// <summary>
    /// This class provides model metatadata based on the <see cref="CodeType"/>. The model metadata is computed based on common convensions
    /// and propery attributes. If the user provides the datacontext in the UI, entity framework computes the <see cref="ModelMetadata"/>. Else, this class 
    /// provides the model metadata information from the specified <see cref="CodeType"/>.
    /// </summary>
    [Serializable]
    public sealed class CodeModelModelMetadata : ModelMetadata
    {
        public CodeModelModelMetadata(CodeType model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Properties = GetModelProperties(model).ToArray();
            PrimaryKeys = GetModelProperties(model).Where(mp => mp.IsPrimaryKey).ToArray();
        }

        public CodeModelModelMetadata()
        {
        }

        // Change this list to include any non-primitive types you think should be eligible for display/edit
        private static Type[] _bindableNonPrimitiveTypes = new[] 
                                                           {
                                                           typeof(string),
                                                           typeof(decimal),
                                                           typeof(Guid),
                                                           typeof(DateTime),
                                                           typeof(DateTimeOffset),
                                                           typeof(TimeSpan),
                                                           };

        private static IList<PropertyMetadata> GetModelProperties(CodeType codeType)
        {
            IList<PropertyMetadata> results = new List<PropertyMetadata>();
            foreach (CodeProperty property in codeType.GetPublicMembers().OfType<CodeProperty>())
            {
                if (property.HasPublicGetter() && !property.IsIndexerProperty() && IsBindableType(property.Type))
                {
                    results.Add(new CodeModelPropertyMetadata(property));
                }
            }

            return results;
        }

        private static bool IsBindableType(CodeTypeRef type)
        {
            return type.IsPrimitiveType() || _bindableNonPrimitiveTypes.Any(x => type.IsMatchForReflectionType(x));
        }
    }
}
