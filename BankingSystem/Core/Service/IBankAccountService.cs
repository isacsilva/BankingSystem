using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IBankAccountService
    {
        Task<Bankaccount> CreateAccountAsync(Bankaccount account);
        Task<Bankaccount> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<Bankaccount>> GetByBranchAsync(string branch);
        Task<IEnumerable<Bankaccount>> GetByHolderDocumentAsync(string document);
        Task<bool> UpdateEmailAsync(int accountId, string newEmail);
        Task<bool> UpdateStatusAsync(int accountId, string newStatus);
        Task<bool> CloseAccountAsync(int accountId);
        Task<Balance> GetBalanceAsync(int accountId);
        Task<bool> HoldAmountAsync(int accountId, decimal amount);
        Task<bool> ReleaseAmountAsync(int accountId, decimal amount);
        Task<bool> TransferAsync(int fromAccountId, string toAccountNumber, decimal amount);
    }
}
