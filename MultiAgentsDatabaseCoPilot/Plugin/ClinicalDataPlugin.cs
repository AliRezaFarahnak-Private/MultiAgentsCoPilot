using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MultiAgentsDatabaseCoPilot.Plugin;

namespace MultiAgentsDatabaseCoPilot
{
    public class ClinicalDataPlugin : BasePlugin
    {
        [KernelFunction, Description($@"Answers queries on the BaselineData, EfficacyData, SafetyData, and AdditionalData tables.   

public static class DBContext
{{
    public static List<BaselineCharacteristics> BaselineData;
    public static List<EfficacyEndpoint> EfficacyData;
    public static List<SafetyEndpoint> SafetyData;
    public static List<AdditionalObservation> AdditionalData;
    
    BaselineCharacteristics(int ParticipantId,int Group,  // 0 for Placebo, 1 for Drug XYZ int Age, string Gender,  // ""M"" or ""F"" double BMI,double HbA1c );
    EfficacyEndpoint(int ParticipantId,double ChangeInHbA1c,double FastingPlasmaGlucose,double BodyWeightChange,bool HbA1cBelow7);
    SafetyEndpoint(int ParticipantId, bool Hypoglycemia,bool GastrointestinalIssues,bool SeriousAdverseEvents);
    AdditionalObservation(int ParticipantId,double MedicationAdherence,double QualityOfLifeImprovement,bool DiscontinuedDueToAdverseEvents);
}}
")]
        public async Task<string> GetClinicalDataQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
        {
            Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
            string result = await DynamicQuery(CodeQuery);
            return result;
        }
    }
}

// Example queries you might use with this new context:  
/*  
*/
