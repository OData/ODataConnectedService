// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.ConnectedService.Templates
{
    class ODataT4CodeGeneratorFactory: IODataT4CodeGeneratorFactory
    {
        public ODataT4CodeGenerator Create()
        {
            return new ODataT4CodeGenerator();
        }
    }
}
