using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace DotNetNotebook.SampleMagicCommand
{
    public class SampleMagicCommand : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            var loadEnvCommand = new Command("#!sample", "Say hello to you")
            {
                new Option<string>(new[]{ "-n", "--name" }, "Your name."),
            };

            loadEnvCommand.Handler = CommandHandler.Create(
                async (string name, KernelInvocationContext invocationContext) =>
                {
                    PocketView outputMessage = PocketViewTags.h3($"Magic Command gets your name, {name}");
                    invocationContext.Display(outputMessage);

                    var command = new SubmitCode($"Console.WriteLine(\"Hello {name}!\");");
                    await invocationContext.HandlingKernel.SendAsync(command);
                });

            kernel.AddDirective(loadEnvCommand);

            if (KernelInvocationContext.Current is { } context)
            {
                PocketView view = PocketViewTags.div(
                    PocketViewTags.code(nameof(SampleMagicCommand)), " is loaded.", PocketViewTags.br,
                    "Try it: ", PocketViewTags.code("#!sample -n [your-name]")
                );

                context.Display(view);
            }

            return Task.CompletedTask;
        }
    }
}
