using BankingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.StoreProcedures
{
    public class StoredProcedureScripts
    {
        // Store procedure to get account monthly transaction history (max 12 months)
        public const string GetMonthlyTransactionStatements = @"
CREATE OR ALTER PROCEDURE [dbo].[GetMonthlyTransactionStatements]
    @AccountNumber VARCHAR(50),
    @StartDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Return only non-sensitive transaction data
    SELECT 
        t.TransactionId,
        t.TransactionType,
        t.Amount,
        t.Description,
        t.TransactionDate,
        t.CurrentBalance,
        t.BeneficiaryAccountNumber,
        t.BeneficiaryName,
        t.BeneficiaryBank,
        t.TransactionStatus,
        -- User display name instead of ID
        u.FirstName + ' ' + u.LastName AS ProcessedBy,
        -- Account display name instead of ID
        a.AccountName
    FROM Transactions t WITH (NOLOCK)
    INNER JOIN Accounts a WITH (NOLOCK) 
        ON t.AccountId = a.AccountId
    LEFT JOIN Users u WITH (NOLOCK) 
        ON t.UserId = u.UserId
    WHERE a.AccountNumber = @AccountNumber
      AND t.TransactionDate >= @StartDate
    ORDER BY t.TransactionDate DESC
    OPTION (OPTIMIZE FOR UNKNOWN, MAXDOP 4);
END";



        //Stored procedure to get account transaction history for the last 5 weeks
    public const string GetWeeklyTransactionStatements = @"CREATE OR ALTER PROCEDURE [dbo].[GetWeeklyTransactionStatements]
    @AccountNumber VARCHAR(50),
    @StartDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return only non-sensitive transaction data
    SELECT 
        t.TransactionId,
        t.TransactionType,
        t.Amount,
        t.Description,
        t.TransactionDate,
        t.CurrentBalance,
        t.BeneficiaryAccountNumber,
        t.BeneficiaryName,
        t.BeneficiaryBank,
        t.TransactionStatus,
        -- User display name instead of ID
        u.FirstName + ' ' + u.LastName AS ProcessedBy,
        -- Account display name instead of ID
        a.AccountName
    FROM Transactions t WITH (NOLOCK)
    INNER JOIN Accounts a WITH (NOLOCK) 
        ON t.AccountId = a.AccountId
    LEFT JOIN Users u WITH (NOLOCK) 
        ON t.UserId = u.UserId
    WHERE a.AccountNumber = @AccountNumber
      AND t.TransactionDate >= @StartDate
    ORDER BY t.TransactionDate DESC
    OPTION (OPTIMIZE FOR UNKNOWN, MAXDOP 4);
END";
    }


}
