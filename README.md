OData CodeGenerator Tools
 ============= 
 Component | Build  | Status 
--------|--------- |---------
Connected Service|Rolling | [![Build Status](https://dev.azure.com/dotnet/OData/_apis/build/status/OData.ODataConnectedService?branchName=master)](https://dev.azure.com/dotnet/OData/_build/latest?definitionId=136&branchName=master)

## Introduction 
This is the official OData CodeGenerator tools repository. The OData CodeGenerator tools allow users to generate proxy classes from OData Metadata. These proxy classes are used by OData client to interact with an OData service. These tools work for both VB.NET and C# projects. 

The OData Code Generator tools in this repository are: 
1. OData Connected Service
2. OData CLI. 

These tools lets app developers connect their applications to OData Services (both V3 & V4) and generate the client proxy files for the services.

OData Connected Service supports Visual Studio 2017, Visual Studio 2019 and Visual Studio 2022.  

This project is ready to be used in production environment and is currently actively being supported.

To learn more about OData please visit the [OData website](https://www.odata.org/).
To learn more about Microsoft OData Libraries see [documention here](https://docs.microsoft.com/en-us/odata/).

## Why OData Code Generator Tools? 
Do you have an OData service you would like to integrate in your Microsoft .NET application? The OData Code Generator tools make your life easier since you can use them to automatically 
generate proxy classes which are much easier to call. The ability of these tools to rapidly generate and update these classes makes your development faster.
Please check on the documentation on:

1. [How to install and use Connected service to generate these classes](https://docs.microsoft.com/odata/connectedservice/getting-started).

2.  [How to install and use OData CLI to generate these classes](https://docs.microsoft.com/en-us/odata/odatacli/getting-started).


## Getting started

To get started with these OData Code Generator tools, please check on the documentation links shared above.

### OData Connected Service Extension
The OData Connected Service documentation and other artifacts can be found in the two following places:
* [Source Code](https://github.com/OData/ODataConnectedService)
* [Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=marketplace.ODataConnectedService)
* [Releases](https://github.com/OData/ODataConnectedService/releases)
* [Documentation](https://docs.microsoft.com/odata/connectedservice/getting-started)

### OData CLI 
The OData CLI documentation and other artifacts can be found in the two following places:
* [Source Code](https://github.com/OData/ODataConnectedService)
* [dotnet tool](https://www.nuget.org/packages/Microsoft.OData.Cli/)
* [EXE](https://www.nuget.org/packages/Microsoft.OData.Cli.Exe/)
* [Documentation](https://docs.microsoft.com/en-us/odata/odatacli/getting-started)

### Building
In order to build the application Visual Studio 2019/2017/2022 is the recommended development environment with the VSIX development tools installed.

The repository has 6 projects in one solution. The projects are: 

1. Microsoft.OData.Cli
2. Microsoft.OData.CodeGen
3. ODataConnectedService
4. ODataConnectedService.Shared
5. ODataConnectdService.Tests
6. ODataConnectedService.VS2022Plus

#### Building in visual studio
Open the `ODataCodeGenTools.sln` in visual studio and install any suggested extensions that may be missing in your instance.
Build and run the project. 

While developing if you encounter any issues do check the FAQ section below for frequently asked questions

#### Unit Tests

The `ODataConnectedService.Tests` project contains the unit tests for the OData connected service. The folder structure of the tests mirrors the folder structure
of the `ODataConnectedService.Shared` project. The base namespace for tests is `Microsoft.OData.ConnectedService.Tests`. Furthermore each test class has the same name
as the class it is testing, followed by the suffix `Test`.

Example: for some class `Microsoft.OData.ConnectedService.X.Y` located in `src\X\Y.cs`,
the test class would be `Microsoft.OData.Tests.ConnectedService.Tests.X.YTest` located in `test\X\YTest.cs`

### Building Command-line 
The project can also be build using on the command-line by relying on the provided build.cmd

### Other related Projects

*  [Microsoft OData Libraries](https://github.com/OData/odata.net)
*  [Microsoft WebAPi Library](https://github.com/OData/WebApi)
*  [Microsoft Restier](https://github.com/OData/RESTier)

### FAQ
  **Question**: The extension module is not loaded when debugging in the Visual Studio experimental instance, or it is not listed as part of the "Add Connected Service" options
  **Workaround**: Disable strong name verification for the main and test assemblies. You can do that using the `sn.exe` tool from
  the Visual Studio Developer Command Prompt. First, build the solution using Visual Studio, then open the developer command
  prompt with Adminstrator privileges and run the following commands:

  For the main assembly:

  ```
  sn.exe -Vr path\to\ODataConnectedService\src\bin\Debug\Microsoft.OData.ConnectedService.dll
  ```

  For the test assembly:
  ```
  sn.exe -Vr path\to\ODataConnectedService\test\ODataConnectedService.Tests\bin\Debug\ODataConnectedServiceTests.dll
  ```
  Then restart Visual Studio.

  If disabling verification is not an option, then you can disable signing by going to the Project Properties > Signing and disabling both "Sign the assembly" and "Delay sign only" options. Do this for both the main and test projects. Then open the `AssemblyInfo.cs` file
  under the main project's properties in solution explorer and remove the public key from the `[assembly: InternalsVisibleTo("ODataConnectedService.Tests, PublicKey=...")]` attribute so that it only reads as `[assembly: InternalsVisibleTo("ODataConnectedService.Tests")]`. Remember to re-enable delay-signing and to restore the public key before commiting your changes.
  
  **Question**: In Visual Studio 2017, upon configuring the service endpoint in the OData Connected Services extension and clicking "Finish", I get an error message that says "Cannot access".  
  **Workaround**: Most reported issues for this error are related to authentication-based endpoints. This extension does not currently support authentication. To work around, download the metadata as a text file from the endpoint and then point the OData Connected Services URI to the downloaded file.

### High Level roadmap for OData Connected Service
OData Connected Service is considered a stable product. Below are some planned improvements:
* Make code Generator cross-platform. This is to take care of developers using Visual Studio code in non-Windows environments.
* Make it more seamless to generate code from the command line and consequently in build pipelines to enhance automations.

### Contribution

There are many ways for you to contribute to OData Connected services. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/ODataConnectedService/blob/master/CONTRIBUTING.MD) for more details.

###  Support

- Issues: Report issues on [Github issues](https://github.com/OData/ODataConnectedService/issues).
- Questions: Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- Feedback: Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- Team blog: Please visit [https://docs.microsoft.com/odata/](https://docs.microsoft.com/odata/), [https://www.odata.org/blog/](http://www.odata.org/blog/) and [https://devblogs.microsoft.com/odata/](https://devblogs.microsoft.com/odata/).

### Code of Conduct

This project has adopted the [.NET Foundation Contributor Covenant Code of Conduct](https://dotnetfoundation.org/about/policies/code-of-conduct). For more information see the [Code of Conduct FAQ](https://dotnetfoundation.org/about/faq).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).

OData Connected Service is a Copyright &copy; .NET Foundation and other contributors. It is licensed under [MIT License](https://github.com/OData/ODataConnectedService/blob/master/License.txt)