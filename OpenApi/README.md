Convert OData to Open API
============= 

# Introduction 

This folder is for the tools to converting OData to Open API.

## Open API SDK

The OpenAPI.NET SDK contains a useful object model for OpenAPI documents in .NET along with common serializers to extract raw OpenAPI JSON and YAML documents from the model. 

The Open API SDK Source is at [here](https://github.com/microsoft/openapi.net)

## OData to Open API SDK

The OpenAPI.NET.OData SDK contains a functionalities to convert the Edm model to Open API DOM (Document Object Model). 

The Open API OData SDK Source is at [here](https://github.com/microsoft/openapi.net.OData)

## OData to Open API tool

The OData to Open API tool is a console application based on OData to Open API SDK.

### Synopsis

`OData2OpenApi.exe [options]`.

### Options

Options is a pair liking `--key=value`. Belows are the options:

#### --csdl=[value]

Input the OData CSDL, file or Url. It's required.

#### --output=[value]

Set the output file, with extension as 'yaml' or 'json'. It's required.

#### --OperationPath=[true/false]

Enable Edm operation path or not.

#### --OperationImportPath=[true/false]

Enable Edm operation import path or not.

#### --NavigationPath=[true/false]

Enable Edm navigation property path or not.

#### --UnqualifiedCall=[true/false]

Enable unqualified function/action call or not.

#### --KeyAsSegment=[true/false]

Enable key as segment or not.

#### --OperationId=[true/false]

Enable operation ID for Open API operation.

#### --PrefixTypeBeforeKey=[true/false]

Enable prefix entity type name before single key.

---
**Be noted**, all boolean options are optional, the default value are true.
	
## Examples

`OData2OpenApi.exe --csld=http://services.odata.org/TrippinRESTierService -output=trip.json`



