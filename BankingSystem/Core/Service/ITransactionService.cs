using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface ITransactionService
    {
        Task<Transaction> GetById(int transactionId);
        Task<IEnumerable<Transaction>> FilterTransactions(int? Id, int? accountId, DateTime? from, DateTime? to, string? type, string? counterpartyDocument);
        Task<IEnumerable<Transaction>> GetByCounterpartyDocument(string document);
        Task<Transaction> Credit(int accountId, decimal amount);
        Task<Transaction> Debit(int accountId, decimal amount);
    }
}
