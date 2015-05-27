// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace System.Web.OData.Design.Scaffolding
{
    internal static class OleServiceProviderExtensions
    {
        public static InterfaceType CreateSitedInstance<InterfaceType>(this IOleServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
        {
            using (ServiceProvider adapter = new ServiceProvider(serviceProvider))
            {
                return CreateSitedInstance<InterfaceType>(adapter, clsid);
            }
        }

        /// <summary>
        /// Helper to create an instance from the local registry given a CLSID Guid
        /// </summary>
        internal static InterfaceType CreateInstance<InterfaceType>(System.IServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
        {
            InterfaceType instance = null;

            if (clsid != Guid.Empty)
            {
                ILocalRegistry localRegistry = serviceProvider.GetService<ILocalRegistry>();
                if (localRegistry != null)
                {
                    IntPtr proxy = IntPtr.Zero;
                    Guid iid = NativeMethods.IID_IUnknown;

                    // This creates an IUnknown ptr to an object with the given CLSID - the problem is that the object is wrapped
                    // in a proxy behind an IntPtr. To get an object we can call from .Net we need to unwrap the proxy and then
                    // manually release the reference to the proxy.
                    //
                    // This method handles failures in creating an object or converting the appropriate type by returning null,
                    // which the surrounding code expects.
                    int hr = localRegistry.CreateInstance(clsid, null, ref iid, NativeMethods.CLSCTX_INPROC_SERVER, out proxy);
                    if (!NativeMethods.Succeeded(hr) || proxy == IntPtr.Zero)
                    {
                        return null;
                    }

                    instance = Marshal.GetObjectForIUnknown(proxy) as InterfaceType;

                    // We're throwing here on failure because we don't expect this to every happen under any circumstances.
                    hr = Marshal.Release(proxy);
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            return instance;
        }

        /// <summary>
        /// Helper to create an instance from the local registry given a CLSID Guid
        /// </summary>
        internal static InterfaceType CreateSitedInstance<InterfaceType>(System.IServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
        {
            InterfaceType instance = CreateInstance<InterfaceType>(serviceProvider, clsid);
            if (instance != null)
            {
                IObjectWithSite sitedObject = instance as IObjectWithSite;
                IOleServiceProvider site = serviceProvider.GetService<IOleServiceProvider>();
                if (sitedObject != null && site != null)
                {
                    sitedObject.SetSite(site);
                }
                else
                {
                    instance = null; // failed to site
                }
            }

            return instance;
        }
    }
}
