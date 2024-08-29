using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RealTimeDatabaseCoPilot
{
    public static class Utils
    {
        public static IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables().Build();

        public static void ScopeSessionToMaxMessages(this ChatHistory session, int maxMessages = 10)
        {
            if (session.Count > maxMessages)
            {
                int firstUserMessage = -1;
                int secondUserMessage = -1;

                // Find the index of the first User message after index 0  
                for (int i = 0; i < session.Count; i++)
                {
                    if (session[i].Role == AuthorRole.User)
                    {
                        firstUserMessage = i;
                        break;
                    }
                }

                // Find the index of the second User message after the first User message  
                for (int j = firstUserMessage + 1; j < session.Count; j++)
                {
                    if (session[j].Role == AuthorRole.User)
                    {
                        secondUserMessage = j;
                        break;
                    }
                }

                session.RemoveRange(firstUserMessage, secondUserMessage);
            }
        }

        public static void WriteColored(string message, ConsoleColor color, bool doWriteLine = true)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.InputEncoding = Encoding.UTF8; // Ensure console supports Unicode  
            Console.OutputEncoding = Encoding.UTF8; // Ensure console supports Unicode
            if (doWriteLine)
                Console.WriteLine(message);
            else
                Console.Write(message); 
            Console.ForegroundColor = previousColor;
        }

        public async static void ConvertToEChartHtml(string json)
        {
            try
            {
                ChatHistory messages = new ChatHistory();
                messages.AddUserMessage(@$"Convert this data to be presented as a echart in a html file, give me the complete HTML so it can be displayed, also add a small script that reloads the page every 3 second.
                                           If it doesnt make sense to display it as a echart, just give me the HTML and message that says that the data is not feasible for a visual chart.
                                           Try to make the echarts colorful and beautiful, and if it is just one number then say also is not feasible to display.
                                           Only give answer with the clean html. This is the data: {json}");

                var response = await Program.Kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(messages, Program.ExecutionSettings, Program.Kernel);
                var content = response.Content;

                // Extract HTML content using regex  
                string pattern = @"<html[\s\S]*<\/html>";
                Match match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                string htmlContent = match.Value;

                // Save the content to an HTML file named "echart.html" three directories up from the current executable  
                SaveHtmlToFile(htmlContent, "echart.html");
                Utils.WriteColored(@$"{Environment.NewLine}Visualization handled", ConsoleColor.Cyan);
                Console.WriteLine(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while converting the data to an EChart: {ex.Message}");
            }
        }

        private static void SaveHtmlToFile(string htmlContent, string fileName)
        {
            try
            {
                // Get the directory of the current executable  
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                // Navigate three directories up from the current executable directory  
                string projectDirectory = Path.GetFullPath(Path.Combine(directoryPath, @"..\..\.."));
                string filePath = Path.Combine(projectDirectory, fileName);

                // Write the HTML content to the file  
                File.WriteAllText(filePath, htmlContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
            }
        }
    }
}
