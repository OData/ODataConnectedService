// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace System.Web.OData.Design.Scaffolding.VisualStudio
{
    internal class ProjectSettings : IProjectSettings
    {
        public ProjectSettings(IVsBuildPropertyStorage storage)
        {
            Contract.Assert(storage != null);

            Storage = storage;
        }

        public string this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                string value;
                int hr = Storage.GetPropertyValue(key, null, (uint)_PersistStorageType.PST_USER_FILE, out value);

                // ignore this HR, it means that there's no value for this key
                if (hr != NativeMethods.E_XML_ATTRIBUTE_NOT_FOUND)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                if (value == null)
                {
                    int hr = Storage.RemoveProperty(key, null, (uint)_PersistStorageType.PST_USER_FILE);

                    // ignore this HR, it means that there's no value for this key
                    if (hr != NativeMethods.E_XML_ATTRIBUTE_NOT_FOUND)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }
                else
                {
                    int hr = Storage.SetPropertyValue(key, null, (uint)_PersistStorageType.PST_USER_FILE, value);
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        private IVsBuildPropertyStorage Storage
        {
            get;
            set;
        }
    }
}
