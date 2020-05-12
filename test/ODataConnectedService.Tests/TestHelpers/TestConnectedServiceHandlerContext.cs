//-----------------------------------------------------------------------------------
// <copyright file="TestConnectedServiceHandlerContext.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;

namespace Microsoft.OData.ConnectedService.Tests.TestHelpers
{
    class TestConnectedServiceHandlerContext : ConnectedServiceHandlerContext
    {
        public TestConnectedServiceHandlerContext(ConnectedServiceInstance serviceInstance = null,
            ConnectedServiceHandlerHelper handlerHelper = null, IVsHierarchy projectHierarchy = null) : base()
        {
            ServiceInstance = serviceInstance;
            HandlerHelper = handlerHelper;
            ProjectHierarchy = projectHierarchy;

            var mockLogger = new Mock<ConnectedServiceLogger>();
            mockLogger.Setup(l => l.WriteMessageAsync(It.IsAny<LoggerMessageCategory>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            Logger = mockLogger.Object;
        }

        public object SavedExtendedDesignData { get; private set; }

        public override void SetExtendedDesignerData<TData>(TData data)
        {
            SavedExtendedDesignData = data;
        }
        public override IDictionary<string, object> Args =>
            throw new System.NotImplementedException();

        public override EditableXmlConfigHelper CreateEditableXmlConfigHelper() =>
            throw new System.NotImplementedException();

        public override XmlConfigHelper CreateReadOnlyXmlConfigHelper() =>
            throw new System.NotImplementedException();

        public override TData GetExtendedDesignerData<TData>() =>
            throw new System.NotImplementedException();
    }
}
