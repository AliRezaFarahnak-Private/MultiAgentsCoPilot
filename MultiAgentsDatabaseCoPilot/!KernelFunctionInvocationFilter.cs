using Microsoft.SemanticKernel;

namespace MultiAgentsDatabaseCoPilot;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class KernelFunctionInvocationFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        // Example: get function information
        var functionName = context.Function.Name;

        // Example: get chat history
        var chatHistory = context.ChatHistory;

        // Example: get information about all functions which will be invoked
        var functionCalls = FunctionCallContent.GetFunctionCalls(context.ChatHistory.Last());

        // Calling next filter in pipeline or function itself.
        // By skipping this call, next filters and function won't be invoked, and function call loop will proceed to the next function.
        await next(context);

        // Example: get function result
        var result = context.Result.GetValue<string>();

       // Utils.ConvertToEChartHtml(result);

        // Example: override function result value
       // context.Result = new FunctionResult(context.Result, "Result from auto function invocation filter");

        // Example: Terminate function invocation
       // context.Terminate = true;
    }
}
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
