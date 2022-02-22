using System.CommandLine;
using System.Threading.Tasks;

namespace Microsoft.OData.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GenerateCommand generateCommand = new GenerateCommand();
            RootCommand app = new RootCommand {
                generateCommand
            };
            await app.InvokeAsync(args);
        }
    }
}
