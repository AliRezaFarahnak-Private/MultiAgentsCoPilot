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
Pie Chart Questions:
What proportion of participants are in the placebo group versus the Drug XYZ group?
What is the gender distribution among participants?
What percentage of participants experienced hypoglycemia?
What proportion of participants discontinued due to adverse events?
What percentage of participants experienced gastrointestinal issues?
What proportion of participants had serious adverse events?
What percentage of participants showed a quality of life improvement greater than 10?
Bar Chart Questions:
What is the average age of participants in the placebo group versus the Drug XYZ group?
What is the average BMI of male versus female participants?
How many participants experienced hypoglycemia in the placebo group versus the Drug XYZ group?
What is the average change in HbA1c for participants with HbA1c below 7 in the placebo group versus the Drug XYZ group?
How many participants discontinued due to adverse events in the placebo group versus the Drug XYZ group?
What is the average medication adherence for participants in the placebo group versus the Drug XYZ group?
How many participants experienced gastrointestinal issues in the placebo group versus the Drug XYZ group?
How many participants had serious adverse events in the placebo group versus the Drug XYZ group?
What is the distribution of body weight change among participants?
What is the distribution of fasting plasma glucose levels among participants?

*/
