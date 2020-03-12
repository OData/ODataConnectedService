ODataConnectedService
 ============= 
 Component | Build  | Status 
--------|--------- |---------
Connected Service|Rolling | <img src="https://identitydivision.visualstudio.com/_apis/public/build/definitions/2cfe7ec3-b94f-4ab9-85ab-2ebff928f3fd/338/badge"/> 

 ## Introduction 
This is the official OData Connected Service repository which allows users to generate proxy classes from the OData Metadata.

### OData Connected Service Provider
The OData Connected Service release can be found in the two following places. 
* [Source Code](https://github.com/OData/ODataConnectedService)
* [Visual Studio Extension](https://visualstudiogallery.msdn.microsoft.com/b343d0eb-6493-44c2-b558-13a0408d013f)

### Building
In order to build the application Visual Studio 2019/2017 is the recommended development environment with the visx development tools installed.

The repository has 2 projects which are 
1. The main ODataConnectedService project `ODataConnectedService.sln`
2. The ODataConnectedServiceTests project `ODataConnectedService.Tests.sln`

#### Building in visual studio
Open the `ODataConnectedService.sln` in visual studio and install any suggested extensions that may be missing in your instance.
Build and run the project. 

While developing if you encounter any issues do check the FAQ section below for frequently asked questions
#### Unit Tests

The `ODataConnectedService.Tests.sln` contains the unit tests for the OData connected service. The folder structure of the tests mirrors the folder structure
of the `ODataConnectedService` project. The base namespace for tests is `Microsoft.OData.ConnectedService.Tests`. Furthermore each test class has the same name
as the class it is testing, followed by the suffix `Test`.

Example: for some class `Microsoft.OData.ConnectedService.X.Y` located in `src\X\Y.cs`,
the test class would be `Microsoft.OData.Tests.ConnectedService.Tests.X.YTest` located in `test\X\YTest.cs`

### Building Commandline 
The project can also be build using on the commandline by relying on the provided build.cmd

### FAQ
  **Question**: The extension module is not loaded when debugging in the Visual Studio experimental instance, or it is not listed as part of the "Add Connected Service" options
  **Workaround**: Disable signing by going to the Project Properties > Signing and disabling both "Sign the assembly" and "Delay sign only" options.
  
  **Question**: In Visual Studio 2017, upon configuring the service endpoint in the OData Connected Services extension and clicking "Finish", I get an error message that says "Value cannot be null.\r\nParameter name: path1".  
  **Workaround**: Download the [Microsoft WCF ToolKit](https://download.microsoft.com/download/1/C/A/1CAA41C7-88B9-42D6-9E11-3C655656DAB1/WcfDataServices.exe) and install it. Then go to the registry and find the following key: `[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Microsoft WCF Data Services]`. Create a duplicate of "VS 2010 Tooling" (if this doesn't exist, use "5.6" instead) named "VS 2014 Tooling". Then try again. (Special thanks to [mohsenno1](https://github.com/mohsenno1) for pointing this out.)
  
  **Question**: In Visual Studio 2017, upon configuring the service endpoint in the OData Connected Services extension and clicking "Finish", I get an error message that says "Cannot access".  
  **Workaround**: Most reported issues for this error are related to authentication-based endpoints. This extension does not currently support authentication. To work around, download the metadata as a text file from the endpoint and then point the OData Connected Services URI to the downloaded file.


### Contribution

There are many ways for you to contribute to OData Connected services. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/ODataConnectedService/CONTRIBUTING.md) for more details.

###  Support

- Issues: Report issues on [Github issues](https://github.com/OData/ODataConnectedService/issues).
- Questions: Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- Feedback: Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- Team blog: Please visit [https://docs.microsoft.com/odata/](https://docs.microsoft.com/odata/) and [http://www.odata.org/blog/](http://www.odata.org/blog/).

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.