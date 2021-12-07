using System.CommandLine;
using System.Threading.Tasks;

namespace OData.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = new RootCommand
            {
                new Command("generate", "Command to generate proxy classes for OData endpoints.")
                {
                    new Option<string>(
                        new[] { "--metadata|-m"},
                        "The URI of the metadata document. The value must be set to a valid service document URI or a local file path."
                    ){IsRequired = true},
                    new Option<string>(
                        new[] { "--load-settings-from-file|-lsff" },
                        "The location of the configuration settings file."
                    ),
                    new Option<string>(
                        new[] { "--custom-headers|-h"},
                        "Headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue"
                    ),
                    new Option<string>(
                        new[] { "--proxy|-p"},
                        "Proxy settings. Format: domain\\user:password@SERVER:PORT."
                    ),
                    new Option<string>(
                        new[] { "--namespace|-ns"},
                        "The namespace of the client code generated. Example:ODataCliCodeGeneratorSample.NorthWindModel or ODataCliCodeGeneratorSample or it could be a name related to the OData endpoint."
                    ),
                    new Option<bool?>(
                        new[] { "--no-tracking|-n"},
                        "Disables entity and property tracking. Entity tracking enables the client to track an entity in DataServiceContext while property tracking enables the client to track only top-level properties."
                    ),
                    new Option<bool?>(
                        new[] { "--upper-camel-case|-ucc"},
                        "Disables upper camel casing."
                    ),
                    new Option<bool?>(
                        new[] { "--internal|-i"},
                        "Apply the \"internal\" class modifier on generated classes instead of \"public\" thereby making them invisible outside the assembly."
                    ),
                    new Option<bool?>(
                        new[] { "--multiple-files|-mf"},
                        "Split the generated classes into separate files instead of generating all the code in a single file."
                    ),
                    new Option<string>(
                        new[] { "--excluded-operation-imports|-eoi"},
                        "Comma-separated list of the names of operation imports to exclude from the generated code. Example: ExcludedOperationImport1,ExcludedOperationImport2"
                    ),
                    new Option<string>(
                        new[] { "--excluded-bound-operations|-ebo"},
                        "Comma-separated list of the names of bound operations to exclude from the generated code.Example: BoundOperation1,BoundOperation2"
                    ),
                    new Option<string>(
                        new[] { "--excluded-entity-types|-est"},
                        "Comma-separated list of the names of entity types to exclude from the generated code.Example: EntityType1,EntityType2,EntityType3"
                    ),
                    new Option<string>(
                        new[] { "--excluded-function-imports|-efi"},
                        "Comma-separated list of the names of function imports to exclude from the generated code.Example: FunctionImport1,FunctionImport2"
                    ),
                    new Option<string>(
                        new[] { "--excluded-action-imports|-eai"},
                        "Comma-separated list of the names of action imports to exclude from the generated code.Example: ActionImport1,ActionImport2"
                    ),
                    new Option<bool?>(
                        new[] { "--ignore-unexpected-elements|-iue"},
                        "This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any."
                    ),
                    new Option<string>(
                        new[] { "--file-name|-fn"},
                        "The file name of the generated proxy classes."
                    ),
                    new Option<string>(
                        new[] { "--outputdir|-o"},
                        "Full path to output directory."
                    )
                }
            };

            await app.InvokeAsync(args);
        }
    }
}
