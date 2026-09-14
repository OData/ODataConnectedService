//-----------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using Xunit;

// The CLI tests share process-wide MSBuild state, including the global project
// collection. Running test classes concurrently can unload projects while another
// class is evaluating them and cause a native testhost crash.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
