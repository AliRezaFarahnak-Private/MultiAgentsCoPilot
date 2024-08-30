using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace MultiAgentsDatabaseCoPilot;
#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101

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
        Utils.WriteColored(@$"Multi-Agent Database CoPilot{Environment.NewLine}", ConsoleColor.Yellow);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(DeploymentName, EndPoint, Key);
        Kernel = kernelBuilder.Build();
     //   Kernel.ImportPluginFromType<ClinicalDataPlugin>();

      Kernel.AutoFunctionInvocationFilters.Add(new KernelFunctionInvocationFilter());

        _session = new ChatHistory();

       
        // DBContext.LoadData();


        string ProgamManager = """
    You are a program manager which will take the requirement and create a plan for creating app. Program Manager understands the 
    user requirements and form the detail documents with requirements and costing. 
""";

        string SoftwareEngineer = """
   You are Software Engieer, and your goal is develop web app using HTML and JavaScript (JS) by taking into consideration all
   the requirements given by Program Manager. 
""";

        string ProjectManager = """
    You are manager which will review software engineer code, and make sure all client requirements are completed.
     Once all client requirements are completed, you can approve the request by just responding "approve"
""";


        Microsoft.SemanticKernel.Agents.ChatCompletionAgent ProgaramManagerAgent =
                   new()
                   {
                       Instructions = ProgamManager,
                       Name = "ProgaramManagerAgent",
                       Kernel = Kernel
                   };

        Microsoft.SemanticKernel.Agents.ChatCompletionAgent SoftwareEngineerAgent =
                   new()
                   {
                       Instructions = SoftwareEngineer,
                       Name = "SoftwareEngineerAgent",
                       Kernel = Kernel
                   };

        Microsoft.SemanticKernel.Agents.ChatCompletionAgent ProjectManagerAgent =
                   new()
                   {
                       Instructions = ProjectManager,
                       Name = "ProjectManagerAgent",
                       Kernel = Kernel
                   };

        AgentGroupChat chat =
                    new(ProgaramManagerAgent, SoftwareEngineerAgent, ProjectManagerAgent)
                    {
                        ExecutionSettings =
                            new()
                            {
                                TerminationStrategy =
                                    new ApprovalTerminationStrategy()
                                    {
                                        Agents = [ProjectManagerAgent],
                                        MaximumIterations = 6,
                                    }
                            }
                    };

        while (true)
        {
            string user = Console.ReadLine();
            if (user.ToLower() == "exit")
            {
                break;
            }


            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, user));
            Console.WriteLine($"# {AuthorRole.User}: '{user}'");

            await foreach (var content in chat.InvokeAsync())
            {
                Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
            }
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

sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    // Terminate when the final message contains the term "approve"
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
}

#pragma warning restore SKEXP0110, SKEXP0001, SKEXP0101
