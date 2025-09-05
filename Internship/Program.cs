using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using SemanticKernelPlugins;
class Program
{
    static async Task Main(string[] args)
    {
        // // TEMPORARY ONLY: Bypass SSL issues if needed
        // ServicePointManager.ServerCertificateValidationCallback +=
        //     (sender, cert, chain, sslPolicyErrors) => true;

        try
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            // Add Ollama Chat Completion
            kernelBuilder.AddOllamaChatCompletion(
                modelId: "llama3.1:8b",
                endpoint: new Uri("http://localhost:11434") // Change if needed
            );

            var kernel = kernelBuilder.Build();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // Add a plugin for demonstration (optional)
            kernel.ImportPluginFromType<TextPlugin>("Text");
            kernel.ImportPluginFromType<TodoPlugin>("API");

            Console.WriteLine("\n📋 Registered Functions:");
            foreach (var plugin in kernel.Plugins)
            {
                Console.WriteLine($"Plugin: {plugin.Name}");
                foreach (var function in plugin)
                {
                    Console.WriteLine($"  - {function.Name}: {function.Description}");
                }
            }

            // IMPORTANT: Use OllamaPromptExecutionSettings instead of generic PromptExecutionSettings
            var settings = new OllamaPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            var history = new ChatHistory();

            Console.WriteLine("Type your messages. Type 'exit' to quit.\n");

            string? userInput;
            do
            {
                Console.Write("User > ");
                userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                history.AddUserMessage(userInput);

                try
                {
                    var response = await chatService.GetChatMessageContentAsync(
                        history,
                        settings,
                        kernel: kernel);

                    Console.WriteLine("Ollama > " + response.Content);
                    history.AddMessage(response.Role, response.Content ?? string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Chat error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Details: {ex.InnerException.Message}");

                    // Remove failed message
                    if (history.Count > 0)
                        history.RemoveAt(history.Count - 1);
                }

            } while (true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Kernel initialization failed: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Details: {ex.InnerException.Message}");
        }
    }
}