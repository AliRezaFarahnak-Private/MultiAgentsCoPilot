
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.Text.Json;
using System.Diagnostics;

namespace RealTimeDatabaseCoPilot.Plugin;

public abstract class BasePlugin
{
    protected async Task<string> DynamicQuery(string codeQuery)
    {
        try
        {
            // check id codeQuery doesnt end with ;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            var result = await CSharpScript.EvaluateAsync<dynamic>(
                codeQuery,
                ScriptOptions.Default
                    .WithReferences(assemblies)
                    .WithImports("System", "System.Linq", "System.Collections.Generic", $@"{nameof(RealTimeDatabaseCoPilot)}.Plugin", nameof(RealTimeDatabaseCoPilot))
            );

            var resultString = JsonSerializer.Serialize(result);

            Utils.WriteColored(resultString, ConsoleColor.DarkCyan);

            Utils.ConvertToEChartHtml(resultString);

            return resultString;
        }
        catch (Exception ex)
        {
            if(Debugger.IsAttached)
              Utils.WriteColored(ex.Message, ConsoleColor.Red);
            
            return ex.Message;
        }
    }
}
