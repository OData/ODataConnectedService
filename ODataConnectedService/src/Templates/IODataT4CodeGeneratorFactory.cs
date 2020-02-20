// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.ConnectedService.Templates
{
    interface IODataT4CodeGeneratorFactory
    {
        ODataT4CodeGenerator Create();
    }
}
