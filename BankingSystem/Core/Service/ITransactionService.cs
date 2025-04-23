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
        Task<IEnumerable<Transaction>> GetByAccountId(int accountId, DateTime? from = null, DateTime? to = null, string? type = null);
        Task<IEnumerable<Transaction>> GetByCounterpartyDocument(string document);
        Task<Transaction> Credit(int accountId, decimal amount);
        Task<Transaction> Debit(int accountId, decimal amount);
    }
}
