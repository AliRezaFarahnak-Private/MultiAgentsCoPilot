using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MultiAgentsDatabaseCoPilot
{
    public static class DBContext
    {
        public static List<SalesFact> Transactions { get; private set; }

        public static void LoadData()
        {
            Transactions = GenerateMockSalesData(20000);
        }

        private static IEnumerable<T> ReadCsvFile<T>(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        private static void WriteCsvFile<T>(string filePath, IEnumerable<T> records)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        private static List<SalesFact> GenerateMockSalesData(int rowCount)
        {
            var random = new Random();
            var salesData = new List<SalesFact>();

            var productCategories = new[]
            {
                "Electronics", "Clothing", "Home Appliances", "Furniture", "Toys",
                "Books", "Sports Equipment", "Groceries", "Beauty Products", "Automotive"
            };

            var customerGenders = new[] { "Male", "Female", "Non-Binary" };

            var customerCountries = new[]
            {
                "USA", "Canada", "UK", "Germany", "France", "Australia", "Japan", "China", "India", "Brazil",
                "Mexico", "South Korea", "Italy", "Spain", "Russia", "Netherlands", "Switzerland", "Turkey",
                "Sweden", "Poland"
            };

            var startDate = new DateTime(2019, 1, 1);
            var endDate = new DateTime(2024, 7, 31);
            var dateRange = (endDate - startDate).Days;

            for (int i = 0; i < rowCount; i++)
            {
                salesData.Add(new SalesFact
                {
                    Date = startDate.AddDays(random.Next(dateRange + 1)),
                    ProductID = random.Next(100, 110),
                    ProductCategory = productCategories[random.Next(productCategories.Length)],
                    SalesQuantity = random.Next(1, 20),
                    SalesAmount = random.Next(100, 5000),
                    CustomerGender = customerGenders[random.Next(customerGenders.Length)],
                    CustomerCountry = customerCountries[random.Next(customerCountries.Length)],
                    CustomerAge = random.Next(18, 70)
                });
            }

            return salesData;
        }
    }
}
