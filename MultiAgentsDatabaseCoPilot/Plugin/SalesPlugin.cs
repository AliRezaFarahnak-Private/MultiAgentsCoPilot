using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RealTimeDatabaseCoPilot.Plugin;

public class SalesPlugin : BasePlugin
{
    [KernelFunction, Description(@"
Answers queries on the SalesFact table. 

SalesFacts are accessible like this:  
DBContext.Transactions<List<SalesFact>> Transactions = DBContext.Transactions;  
SalesFact(DateTime Date, int ProductID, string ProductCategory, int SalesQuantity, decimal SalesAmount, string CustomerGender, string CustomerCountry, int CustomerAge)")]
    public async Task<string> GetSalesFactQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        Utils.WriteColored(CodeQuery, ConsoleColor.DarkYellow);
        string result = await DynamicQuery(CodeQuery);
        return result;
    }
}

/*
How many rows of data do you have?
From which years do you have data?
What is the total sales amount per month for Electronics in 2023?
What is the total sales amount per month for Electronics from 2020 to 2023?
Which 5 products had the highest sales quantity in 2022?
What is the total sales amount for each customer country in 2021?
List the top 3 months with the highest sales amount for Furniture in 2023.
What is the average sales amount per transaction for Clothing in 2022?
Which customer age group had the highest total sales amount in 2021?
What is the total number of sales transactions for Beauty Products in 2020?
Which 5 days had the highest sales amount in 2023?
What is the total sales quantity per product category in 2022?
List the top 10 customers by total sales amount in 2023.
*/
