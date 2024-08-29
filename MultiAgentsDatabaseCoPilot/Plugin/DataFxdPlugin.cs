using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class DataFxdPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on DataFxd Table DataFxd only accessible like this: DBContext.DataFxdList<DataFxd> DataFxd = DBContext.DataFxd;DataFxd(DateTime date_t, decimal perceived_call_success_rate, decimal perceived_call_drop_rate)")]
    public async Task<string> GetDataFxdQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/* 
Give me the 5 days that had the highest perceived call success rate in 2021?  
List the 10 dates  with the lowest perceived call success rates in 2020? 
*/
