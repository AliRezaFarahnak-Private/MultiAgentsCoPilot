﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiAgentsDatabaseCoPilot;
#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101

class Program
{
    public static string DeploymentName = Utils.Configuration["AzureOpenAI:DeploymentName"];
    public static string EndPoint = Utils.Configuration["AzureOpenAI:Endpoint"];
    public static string Key = Utils.Configuration["AzureOpenAI:ApiKey"];
    public static Kernel Kernel;

    private static ChatHistory _session = new();

    static async Task Main(string[] args)
    {
        /*
         * Make a world time application visualizing 8 round clocks with different time zones and the country name under it. 
         */
        Utils.WriteColored(@$"Multi-Agent ProgramManager SoftwareEngineer ProjectManager{Environment.NewLine}", ConsoleColor.Yellow);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(DeploymentName, EndPoint, Key);
        Kernel = kernelBuilder.Build();

        Kernel.AutoFunctionInvocationFilters.Add(new KernelFunctionInvocationFilter());

        string ProgramManager = """      
    You are a Program Manager responsible for gathering detailed requirements from the user   
    and creating a comprehensive plan for app development. You will document the user   
    requirements and provide cost estimates. Ensure you extract all necessary requirements   
    from the user.  
""";

        string SoftwareEngineer = """     
    You are a Software Engineer tasked with developing a web app using HTML and JavaScript (JS).   
    You must adhere to all the requirements specified by the Program Manager. Your output   
    should consist of a single file containing the complete HTML and JS code.  
""";

        string ProjectManager = """      
    You are a Project Manager who reviews the Software Engineer's code to ensure all client   
    requirements are met. Once all requirements are satisfied, you can approve the request   
    by simply responding with "approve".  
""";

        ChatCompletionAgent SoftwareEngineerAgent =
                   new()
                   {
                       Instructions = SoftwareEngineer,
                       Name = "SoftwareEngineerAgent",
                       Kernel = Kernel
                   };

        ChatCompletionAgent ProjectManagerAgent =
                   new()
                   {
                       Instructions = ProjectManager,
                       Name = "ProjectManagerAgent",
                       Kernel = Kernel
                   };

        AgentGroupChat chat =
                    new(new ChatCompletionAgent()
                    {
                        Instructions = ProgramManager,
                        Name = "ProgaramManagerAgent",
                        Kernel = Kernel
                    }, SoftwareEngineerAgent, ProjectManagerAgent)
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
            Console.WriteLine("Explain your detailed requirement for you html application.");
            string user = Console.ReadLine();
            if (user.ToLower() == "exit")
            {
                break;
            }

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, user));
            Console.WriteLine($"# {AuthorRole.User}: '{user}'");

            await foreach (var content in chat.InvokeAsync())
            {
                if (TryCropHtmlContent(content.Content, out string croppedHtml))
                {
                    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{croppedHtml}'");
                    OpenHtmlInBrowser(croppedHtml);
                }
                else
                {
                    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
                }
            }
        }
    }

    private static bool TryCropHtmlContent(string content, out string croppedHtml)
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
