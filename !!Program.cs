using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiAgentsCoPilot;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    private static readonly string DeploymentName = Configuration["AzureOpenAI:DeploymentName"] ?? throw new ArgumentNullException("AzureOpenAI:DeploymentName");
    private static readonly string EndPoint = Configuration["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:Endpoint");
    private static readonly string ApiKey = Configuration["AzureOpenAI:ApiKey"] ?? throw new ArgumentNullException("AzureOpenAI:ApiKey");

    private static Kernel Kernel = null!;

    static async Task Main(string[] args)
    {
        try
        {
            InitializeKernel();

            while (true)
            {
                DisplayWelcomeMessage();

                var softwareEngineerAgent = CreateAgent("SoftwareEngineer", SoftwareEngineer);
                var programManagerAgent = CreateAgent("ProgramManager", ProgramManager);
                var productOwnerAgent = CreateAgent("ProductOwner", ProductOwner);

                var chat = new AgentGroupChat(programManagerAgent, softwareEngineerAgent, productOwnerAgent)
                {
                    ExecutionSettings = new()
                    {
                        TerminationStrategy = new ApprovalTerminationStrategy
                        {
                            Agents = new List<ChatCompletionAgent> { productOwnerAgent },
                            MaximumIterations = 15,
                        }
                    }
                };

                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
                {
                    break;
                }

                chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));
                await ProcessChatAsync(chat);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static void InitializeKernel()
    {
        var kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(DeploymentName, EndPoint, ApiKey);
        Kernel = kernelBuilder.Build();
    }

    private static void DisplayWelcomeMessage()
    {
        WriteColored($"Multi-Agent Program Manager, Software Engineer, Product Owner{Environment.NewLine}", ConsoleColor.Yellow);
    }

    private static ChatCompletionAgent CreateAgent(string name, string instructions)
    {
        return new ChatCompletionAgent
        {
            Instructions = instructions,
            Name = name,
            Kernel = Kernel
        };
    }

    private static async Task ProcessChatAsync(AgentGroupChat chat)
    {
        bool appCreated = false;
        List<ChatMessageContent> messages = new List<ChatMessageContent>();

        await foreach (var content in chat.InvokeAsync())
        {
            messages.Add(content);
            ConsoleColor color = content.AuthorName switch
            {
                "ProgramManager" => ConsoleColor.Green,
                "SoftwareEngineer" => ConsoleColor.Blue,
                "ProductOwner" => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };

            WriteColored($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'", color);
        }

        if (chat.IsComplete)
        {
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                var content = messages[i];
                if (content.Content != null && TryExtractHtmlContent(content.Content, out var croppedHtml))
                {
                    WriteColored($"# {content.Role} - {content.AuthorName ?? "*"}: '{croppedHtml}'", ConsoleColor.DarkRed);
                    OpenHtmlInBrowser(croppedHtml);
                    appCreated = true;
                    break;
                }
            }
        }

        if (appCreated)
        {
            return;
        }
    }

    private static bool TryExtractHtmlContent(string content, out string? croppedHtml)
    {
        var match = Regex.Match(content, @"<html[\s\S]*?<\/html>", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            croppedHtml = match.Value;
            return true;
        }
        croppedHtml = null;
        return false;
    }

    private static void OpenHtmlInBrowser(string htmlContent)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "temp.html");
        File.WriteAllText(tempFilePath, htmlContent);
        Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
    }

    public static void WriteColored(string message, ConsoleColor color, bool writeLine = true)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        if (writeLine)
            Console.WriteLine(message);
        else
            Console.Write(message);

        Console.ForegroundColor = previousColor;
    }

    private const string ProgramManager = @"
        You are a Program Manager responsible for gathering detailed requirements from the user
        and creating a comprehensive plan for app development. Your tasks include:
        - Documenting user requirements clearly and comprehensively.
        - Ensuring all necessary requirements are extracted from the user.
        - Outlining each requirement clearly so the Software Engineer understands what to implement exactly.
        - Emphasizing that functionality and design are mandatory to be explained by the user.
        - Note: Cost is not important for this project.
    ";

    private const string SoftwareEngineer = @"
        You are a Software Engineer tasked with developing a web app using HTML and JavaScript (JS).
        Your tasks include:
        - Adhering to all the requirements specified by the Program Manager.
        - Implementing each requirement defined by the Program Manager.
        - Producing a single file containing the complete HTML and JS code that satisfies the requirements.
        - Ensuring that the functionality and design as explained by the user are implemented accurately.
    ";

    private const string ProductOwner = @"
        You are a Product Owner responsible for ensuring that the Software Engineer's code meets all Program Manager requirements.
        Your tasks include:
        - Reviewing the code thoroughly to verify that it aligns with the specified requirements.
        - Ensuring that the functionality and design as explained by the user are implemented accurately.
        - Approving the request by responding with 'approve' once you are satisfied that all requirements are met.
    ";

}

sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    public List<ChatCompletionAgent> Agents { get; set; } = [];
    public int MaximumIterations { get; set; }

    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
    {
        return Task.FromResult(history.Last().Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
    }
}
#pragma warning restore SKEXP0110, SKEXP0001, SKEXP0101

