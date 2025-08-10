using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.StoreProcedures
{
    public class StoredProcedureScripts
    {
        public const string GetPagedTransactions = @"
        CREATE PROCEDURE GetPagedTransactions
            @StartIndex INT,
            @PageSize INT
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT * 
            FROM Transactions
            ORDER BY TransactionDate DESC
            OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY;
        END
    ";
    

   // Store procedure to get account monthly transaction 
    public const string GetRecentTransactions = @"
    CREATE OR ALTER PROCEDURE [dbo].[GetRecentTransactions]
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
        INNER JOIN Accounts a WITH (NOLOCK) ON t.AccountId = a.AccountId
        LEFT JOIN Users u WITH (NOLOCK) ON t.UserId = u.UserId
        WHERE a.AccountNumber = @AccountNumber
        AND t.TransactionDate >= @StartDate
        ORDER BY t.TransactionDate DESC
        OPTION (OPTIMIZE FOR UNKNOWN, MAXDOP 4);
    END";
    }
}

