namespace MultiAgentsDatabaseCoPilot.Server;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;


public class SemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly ChatHistory _session;
    private readonly OpenAIPromptExecutionSettings _executionSettings;

    public SemanticKernelService()
    {
        string deploymentName = "gpt-4o";
        string endPoint = "https://dalle3-alfarahn.openai.azure.com";
        string key = "2e3819d5b7db4f58b062df0304077ad1";

        _executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            ChatSystemPrompt = "You are a smart AI that analyzes sales and answers general questions.",
            Temperature = 0.2f,
            TopP = 0.2f,
            MaxTokens = 4096
        };

        var kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(deploymentName, endPoint, key);
        _kernel = kernelBuilder.Build();
        //  _kernel.ImportPluginFromType<ClinicalDataPlugin>();
#pragma warning disable SKEXP0001
        //  _kernel.AutoFunctionInvocationFilters.Add(new KernelFunctionInvocationFilter());
#pragma warning restore SKEXP0001

        _session = new ChatHistory();
    }

    public async Task<string> TextCompletionAsync(string user)
    {
        try { 
        _session.AddUserMessage(user);
        IAsyncEnumerable<StreamingChatMessageContent> responseStream = _kernel.GetRequiredService<IChatCompletionService>()
            .GetStreamingChatMessageContentsAsync(_session, _executionSettings, _kernel);

        string completeResponse = "";
        await foreach (var responsePart in responseStream)
        {
            completeResponse += responsePart.Content;
        }
        _session.AddAssistantMessage(completeResponse);
        return completeResponse;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
