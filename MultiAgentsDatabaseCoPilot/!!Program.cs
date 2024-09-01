using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiAgentsDatabaseCoPilot;
#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101

class Program
{
    public static string DeploymentName = Utils.Configuration["AzureOpenAI:DeploymentName"] ?? throw new ArgumentNullException("AzureOpenAI:DeploymentName");
    public static string EndPoint = Utils.Configuration["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:Endpoint");
    public static string Key = Utils.Configuration["AzureOpenAI:ApiKey"] ?? throw new ArgumentNullException("AzureOpenAI:ApiKey");
    public static Kernel Kernel = null!;

    private static ChatHistory _session = new();

    static async Task Main(string[] args)
    {
        /*
         * Nake a very simple calculator app
         * 
         * Make a very simple world time app visualizing 4 round clocks with different time zones and the country name under it 
         * 
         * 
         */
        string ProgramManager = """      
    You are a Program Manager responsible for gathering detailed requirements from the user   
    and creating a comprehensive plan for app development. You will document the user   
    requirements and provide cost estimates. Ensure you extract all necessary requirements   
    from the user.  
""";
        string SoftwareEngineer = """     
    You are a Software Engineer tasked with developing a web app using HTML and JavaScript (JS).   
    You must adhere to all the requirements specified by the Program Manager. Your output   
    should consist of a single file containing the complete HTML and JS code that satisfies the requirements from the program manager.
""";

        string ProductOwner = """      
    You are a Project Manager who reviews the Software Engineer's code to ensure all client   
    requirements are met. Once all requirements are satisfied, you can approve the request   
    by simply responding with "approve".  
""";

        Utils.WriteColored(@$"Multi-Agent {nameof(ProgramManager)} {nameof(SoftwareEngineer)} {nameof(ProductOwner)}{Environment.NewLine}", ConsoleColor.Yellow);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(DeploymentName, EndPoint, Key);
        Kernel = kernelBuilder.Build();

        Kernel.AutoFunctionInvocationFilters.Add(new KernelFunctionInvocationFilter());


        while (true)
        {
            ChatCompletionAgent SoftwareEngineerAgent =
                       new()
                       {
                           Instructions = SoftwareEngineer,
                           Name = nameof(SoftwareEngineerAgent),
                           Kernel = Kernel
                       };


            ChatCompletionAgent ProgramManagerAgent =
                        new()
                        {
                            Instructions = ProgramManager,
                            Name = nameof(ProgramManagerAgent),
                            Kernel = Kernel
                        };

            ChatCompletionAgent ProductOwnerAgent =
                       new()
                       {
                           Instructions = ProductOwner,
                           Name = nameof(ProductOwnerAgent),
                           Kernel = Kernel
                       };

            AgentGroupChat chat =
                        new(ProgramManagerAgent, SoftwareEngineerAgent, ProductOwnerAgent)
                        {
                            ExecutionSettings =
                                new()
                                {
                                    TerminationStrategy =
                                        new ApprovalTerminationStrategy()
                                        {
                                            Agents = [ProductOwnerAgent],
                                            MaximumIterations = 15,
                                        }
                                }
                        };

            string? user = Console.ReadLine();
            if (user == null || user.ToLower() == "exit")
            {
                break;
            }

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, user));

            bool appCreated = false;

            await foreach (var content in chat.InvokeAsync())
            {
                if (content.Content != null && TryCropHtmlContent(content.Content, out string? croppedHtml))
                {
                    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{croppedHtml}'");
                    OpenHtmlInBrowser(croppedHtml);
                    appCreated = true;
                    break;
                }
                else
                {
                    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
                }
            }

            if (appCreated)
            {
                continue; // Restart the loop
            }
        }
    }

    private static bool TryCropHtmlContent(string content, out string? croppedHtml)
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
        string tempFilePath = Path.Combine(Path.GetTempPath(), "temp.html");
        File.WriteAllText(tempFilePath, htmlContent);
        Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
    }
}

sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
}

#pragma warning restore SKEXP0110, SKEXP0001, SKEXP0101
