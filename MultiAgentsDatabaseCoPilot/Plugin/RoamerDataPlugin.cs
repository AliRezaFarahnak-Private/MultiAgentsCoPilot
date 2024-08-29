using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class RoamerDataPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on RoamersData Table RoamersData only accessible like this: DBContext.RoamerDataList<RoamerData> RoamersData = DBContext.RoamersData;RoamerData(DateTime TXN_DT, string NATIONAL_FLAG, string OPRTR_Cntry, int TXN_DUR, int TXN_CNT, decimal INC_DATA_VOL, decimal OUT_DATA_VOL, long MSISDN, string roaming_type, string usage_type)")]
    public async Task<string> GetRoamerDataQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/*
Give me top 5 countries had the highest incoming data volume in 2020?  
List the top 5 operators by transaction count in 2020?  
What is the average transaction duration for roaming in 2022?  
*/
