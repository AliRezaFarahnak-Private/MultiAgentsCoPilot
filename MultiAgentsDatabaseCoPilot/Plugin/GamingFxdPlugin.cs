using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class GamingFxdPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on GamingFxd Table GamingFxd only accessible like this: DBContext.GamingFxdList<GamingFxd> GamingFxd = DBContext.GamingFxd;GamingFxd(DateTime Date_t, decimal TRAFFIC_TB, int USERS)")]
    public async Task<string> GetGamingFxdQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/*
What is the total traffic (TB) for gaming in 2022 for the top 5 days with most traffic, outline each day?  
Which 10 month had the highest number of gaming users in 2021?  
*/
