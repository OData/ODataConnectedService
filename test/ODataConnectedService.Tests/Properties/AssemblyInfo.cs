using ODataConnectedService.Tests;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

[assembly: AssemblyTitle("ODataConnectedService.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ODataConnectedService.Tests")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("903b31d0-be14-4d9e-ba76-186fa82b3a37")]

// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// disable pararellization to ensure tests don't touch UI code from background threads
[assembly: CollectionBehavior(DisableTestParallelization = true)]
