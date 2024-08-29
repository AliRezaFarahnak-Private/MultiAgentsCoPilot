using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MultiAgentsDatabaseCoPilot.Plugin;

namespace MultiAgentsDatabaseCoPilot;

class Program
{
    public static OpenAIPromptExecutionSettings ExecutionSettings = new ()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        ChatSystemPrompt = "You are smart AI that analyze sales and answer general questions.",
        Temperature = 0.2f,
        TopP = 0.2f,
        MaxTokens = 4096
    };

    public static string DeploymentName = Utils.Configuration["AzureOpenAI:DeploymentName"];
    public static string  EndPoint = Utils.Configuration["AzureOpenAI:Endpoint"];
    public static string Key = Utils.Configuration["AzureOpenAI:ApiKey"];
    public static Kernel Kernel;

    private static ChatHistory _session = new();



    static async Task Main(string[] args)
    {
        Utils.WriteColored(@$"Real-Time Database CoPilot{Environment.NewLine}", ConsoleColor.Yellow);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(DeploymentName, EndPoint, Key);
        Kernel = kernelBuilder.Build();
        Kernel.ImportPluginFromType<SalesPlugin>();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        Kernel.AutoFunctionInvocationFilters.Add(new KernelFunctionInvocationFilter());
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        _session = new ChatHistory();

        DBContext.LoadData();

        while (true)
        {
            string user = Console.ReadLine();
            if (user.ToLower() == "exit")
            {
                break;
            }
            await TextCompletionAsync(user);
            _session.ScopeSessionToMaxMessages();
        }
    }

    public static async Task<string> TextCompletionAsync(string user)
    {
        _session.AddUserMessage(user);
        IAsyncEnumerable<StreamingChatMessageContent> responseStream = Kernel.GetRequiredService<IChatCompletionService>()
            .GetStreamingChatMessageContentsAsync(_session, ExecutionSettings, Kernel);

        string completeResponse = "";
        await foreach (var responsePart in responseStream)
        {
            string content = responsePart.Content;
            completeResponse += content;
            Utils.WriteColored(content, ConsoleColor.Green, false);
        }
        Utils.MakeNextLineInConsole();
        _session.AddAssistantMessage(completeResponse);
        return completeResponse;
    }
}
