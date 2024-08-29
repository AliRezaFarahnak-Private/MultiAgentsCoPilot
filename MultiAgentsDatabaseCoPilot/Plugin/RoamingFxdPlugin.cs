using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class RoamingFxdPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on RoamingFxd Table RoamingFxd only accessible like this: DBContext.RoamingFxdList<RoamingFxd> RoamingFxd = DBContext.RoamingFxd;RoamingFxd(DateTime date_t, decimal Int_IB_Traffic_TB, int Int_IB_Roaming_Numbers, decimal Int_OB_Traffic_TB, int Int_OB_Roaming_Numbers)")]
    public async Task<string> GetRoamingFxdQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/*
Which year had the highest incoming roaming traffic (TB)?  
What is the total number of outgoing roaming numbers for 2020?  
List the top 3 months with the highest outgoing roaming traffic in 2021?  
*/
