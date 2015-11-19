// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;

    internal static class NativeMethods
    {
        public const int E_FAIL = unchecked((int)0x80004005);

        /// <summary>
        /// This is the HResult returned by IVsBuildPropertyStorage when attempting to read a property that's
        /// not in the file.
        /// </summary>
        /// <remarks>
        /// See: http://connect.microsoft.com/VisualStudio/feedback/details/466725/visual-studio-interoperability-request-for-documentation
        /// </remarks>
        public const int E_XML_ATTRIBUTE_NOT_FOUND = unchecked((int)0x8004C738);

        public const int S_OK = unchecked((int)0x00000000);

        public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

        public const int CLSCTX_INPROC_SERVER = 0x1;

        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }
    }
}
