using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiAgentsDatabaseCoPilot
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

        public static void MakeNextLineInConsole()
        {
            Console.WriteLine(Environment.NewLine);
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
