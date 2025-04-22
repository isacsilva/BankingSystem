using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface ITransactionService
    {
        Task<Transaction> GetByIdAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId, DateTime? from = null, DateTime? to = null, string type = null);
        Task<IEnumerable<Transaction>> GetByCounterpartyDocumentAsync(string document);
        Task<Transaction> CreditAsync(int accountId, decimal amount);
        Task<Transaction> DebitAsync(int accountId, decimal amount);
    }
}
