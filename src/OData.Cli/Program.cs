using Microsoft.Extensions.CommandLineUtils;

namespace OData.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "odata-cli";
            app.Description = "Client Proxy generator for OData endpoints.";

            // Set the arguments to display the description and help text
            app.HelpOption("-?|-h|--help");
        }
    }
}
