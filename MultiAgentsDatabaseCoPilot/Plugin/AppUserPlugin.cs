using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class AppUserPlugin: BasePlugin
{
    [KernelFunction, Description(
@"
Answers shortly on AppUser Table
AppUsers only accesible like this: DBContext.AppUser

List<AppUser> AppUsers = DBContext.AppUsers;
AppUser(DateTime Date_t,decimal AppStore_M_users,decimal Facebook_M_users,decimal Instagram_M_users,decimal Netflix_M_users,decimal PlayStation_M_users,decimal SnapChat_M_users,decimal TikTok_M_users,decimal Twitter_M_users,decimal WhatsApp_M_users,decimal YouTube_M_users)")]
    public async Task<string> GetAppUserQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}
/*
What are the total active user counts per month for Facebook in 2023?
Which 5 months in 2022 had the highest number of Snapchat users?
What is the total number of YouTube users in 2022?
*/
