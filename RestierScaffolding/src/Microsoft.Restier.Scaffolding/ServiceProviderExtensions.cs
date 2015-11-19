// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;

    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            return (T)serviceProvider.GetService(typeof(T));
        }

        public static TInterface GetService<TInterface, TService>(this IServiceProvider serviceProvider)
            where TInterface : class
            where TService : class
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            return (TInterface)serviceProvider.GetService(typeof(TService));
        }
    }
}
