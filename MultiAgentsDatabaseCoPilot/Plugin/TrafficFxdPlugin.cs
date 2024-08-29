using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class TrafficFxdPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on TrafficFxd Table TrafficFxd only accessible like this: DBContext.TrafficFxdList<TrafficFxd> TrafficFxd = DBContext.TrafficFxd;TrafficFxd(DateTime date_t, decimal _5G_Traffic_TB, int _5G_Total_Users_K, decimal _4G_Traffic_TB, int _4G_Total_Users_k)")]
    public async Task<string> GetTrafficFxdQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/*
What is the total 5G traffic (TB) per year from 2020 to 2030?  
Which 5 month had the highest number of 4G users in 2022?  
What is the average 4G traffic (TB) per month for the year 2010?  
*/
