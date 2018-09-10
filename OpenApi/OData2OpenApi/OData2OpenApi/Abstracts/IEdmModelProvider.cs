// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OData.Edm;

namespace Microsoft.OData2OpenApi.ConsoleApp.Abstracts
{
    /// <summary>
    /// Interface to provide the <see cref="IEdmModel"/>
    /// </summary>
    public interface IEdmModelProvider
    {
        /// <summary>
        /// Gets the <see cref="IEdmModel"/>.
        /// </summary>
        /// <returns>The Edm model.</returns>
        IEdmModel GetEdmModel();
    }
}