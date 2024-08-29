using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class ResidentAnalyticsPlugin : BasePlugin
{
    [KernelFunction, Description(@"Answers shortly on ResidentAnalytics Table ResidentAnalytics only accessible like this: DBContext.ResidentAnalyticsList<ResidentAnalytics> ResidentAnalytics = DBContext.ResidentAnalytics;ResidentAnalytics(long MSISDN, string TOP_INTEREST, string HOME_CITY_NAME, string HOME_HAI_NAME, string OFFICE_CITY_NAME, string OFFICE_HAI_NAME, string WEALTH_INDEX, string PRIMARY_BANK, decimal TOTAL_MONTHLY_SPENDING, string ROAMING_COUNTRY, decimal INSTAGRAM_DOWNLOAD_VOL, decimal INSTAGRAM_UPLOAD_VOL, int INSTAGRAM_CLICK_COUNT, decimal TWITTER_DOWNLOAD_VOL, decimal TWITTER_UPLOAD_VOL, int TWITTER_CLICK_COUNT, decimal SNAPCHAT_DOWNLOAD_VOL, decimal SNAPCHAT_UPLOAD_VOL, int SNAPCHAT_CLICK_COUNT, decimal TIKTOK_DOWNLOAD_VOL, decimal TIKTOK_UPLOAD_VOL, int TIKTOK_CLICK_COUNT, DateTime OBS_DATE)")]
    public async Task<string> GetResidentAnalyticsQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/* 
List the top 5 country for number of residents by country?
What is the average Instagram download per country in 2021? give me top 5 countries only?
What is the total monthly spending of residents in each country? only take top 10 countries?
*/
