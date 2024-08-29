using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using static MultiAgentsDatabaseCoPilot.DBContext;

namespace MultiAgentsDatabaseCoPilot
{
    public static class DBContext
    {
        public static List<BaselineCharacteristics> BaselineData;
        public static List<EfficacyEndpoint> EfficacyData;
        public static List<SafetyEndpoint> SafetyData;
        public static List<AdditionalObservation> AdditionalData;

        public readonly record struct BaselineCharacteristics(
            int ParticipantId,
            int Group,  // 0 for Placebo, 1 for Drug XYZ  
            int Age,
            string Gender,  // "M" or "F"  
            double BMI,
            double HbA1c
        );

        public readonly record struct EfficacyEndpoint(
            int ParticipantId,
            double ChangeInHbA1c,
            double FastingPlasmaGlucose,
            double BodyWeightChange,
            bool HbA1cBelow7
        );

        public readonly record struct SafetyEndpoint(
            int ParticipantId,
            bool Hypoglycemia,
            bool GastrointestinalIssues,
            bool SeriousAdverseEvents
        );

        public readonly record struct AdditionalObservation(
            int ParticipantId,
            double MedicationAdherence,
            double QualityOfLifeImprovement,
            bool DiscontinuedDueToAdverseEvents
        );

        public static void LoadData()
        {
            BaselineData = MockDataGenerator.GenerateBaselineCharacteristics(20000);
            EfficacyData = MockDataGenerator.GenerateEfficacyEndpoints(20000, BaselineData);
            SafetyData = MockDataGenerator.GenerateSafetyEndpoints(20000, BaselineData);
            AdditionalData = MockDataGenerator.GenerateAdditionalObservations(20000, BaselineData);
        }

        private static IEnumerable<T> ReadCsvFile<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }

        private static void WriteCsvFile<T>(string filePath, IEnumerable<T> records)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(records);
        }
    }

    public static class MockDataGenerator
    {
        private static Random random = new Random();

        public static List<BaselineCharacteristics> GenerateBaselineCharacteristics(int count)
        {
            var list = new List<BaselineCharacteristics>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(new BaselineCharacteristics(
                    ParticipantId: i,
                    Group: random.Next(0, 2),  // 0 for Placebo, 1 for Drug XYZ  
                    Age: random.Next(30, 66),
                    Gender: random.Next(0, 2) == 0 ? "M" : "F",
                    BMI: Math.Round(25 + random.NextDouble() * 10, 1),
                    HbA1c: Math.Round(6 + random.NextDouble() * 4, 1)
                ));
            }
            return list;
        }

        public static List<EfficacyEndpoint> GenerateEfficacyEndpoints(int count, List<BaselineCharacteristics> baselineData)
        {
            var list = new List<EfficacyEndpoint>();
            foreach (var baseline in baselineData)
            {
                double changeInHbA1c = baseline.Group == 1 ? Math.Round(-2 + random.NextDouble() * 2, 1) : Math.Round(-1 + random.NextDouble() * 2, 1);
                list.Add(new EfficacyEndpoint(
                    ParticipantId: baseline.ParticipantId,
                    ChangeInHbA1c: changeInHbA1c,
                    FastingPlasmaGlucose: Math.Round(-50 + random.NextDouble() * 100, 1),
                    BodyWeightChange: Math.Round(baseline.Group == 1 ? -5 + random.NextDouble() * 5 : -2.5 + random.NextDouble() * 5, 1),
                    HbA1cBelow7: changeInHbA1c < -1.5
                ));
            }
            return list;
        }

        public static List<SafetyEndpoint> GenerateSafetyEndpoints(int count, List<BaselineCharacteristics> baselineData)
        {
            var list = new List<SafetyEndpoint>();
            foreach (var baseline in baselineData)
            {
                list.Add(new SafetyEndpoint(
                    ParticipantId: baseline.ParticipantId,
                    Hypoglycemia: baseline.Group == 1 ? random.Next(0, 100) < 15 : random.Next(0, 100) < 10,
                    GastrointestinalIssues: baseline.Group == 1 ? random.Next(0, 100) < 30 : random.Next(0, 100) < 20,
                    SeriousAdverseEvents: baseline.Group == 1 ? random.Next(0, 100) < 7 : random.Next(0, 100) < 3
                ));
            }
            return list;
        }

        public static List<AdditionalObservation> GenerateAdditionalObservations(int count, List<BaselineCharacteristics> baselineData)
        {
            var list = new List<AdditionalObservation>();
            foreach (var baseline in baselineData)
            {
                list.Add(new AdditionalObservation(
                    ParticipantId: baseline.ParticipantId,
                    MedicationAdherence: Math.Round(90 + random.NextDouble() * 10, 1),
                    QualityOfLifeImprovement: Math.Round(baseline.Group == 1 ? 10 + random.NextDouble() * 20 : 5 + random.NextDouble() * 15, 1),
                    DiscontinuedDueToAdverseEvents: baseline.Group == 1 ? random.Next(0, 100) < 10 : random.Next(0, 100) < 5
                ));
            }
            return list;
        }
    }
}
