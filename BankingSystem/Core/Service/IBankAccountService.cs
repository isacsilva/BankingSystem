using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IBankAccountService
    {
        Task<Bankaccount> CreateAccount(Bankaccount account);
        Task<Bankaccount> GetByAccountNumber(string accountNumber);
        Task<IEnumerable<Bankaccount>> GetByBranch(string branch);
        Task<IEnumerable<Bankaccount>> GetByHolderDocument(string document);
        Task<bool> UpdateEmail(int accountId, string newEmail);
        Task<bool> UpdateStatus(int accountId, string newStatus);
        Task<bool> CloseAccount(int accountId);
        Task<Balance> GetBalance(int accountId);
        Task<bool> HoldAmount(int accountId, decimal amount);
        Task<bool> ReleaseAmount(int accountId, decimal amount);
        Task<bool> Transfer(int fromAccountId, string toAccountNumber, decimal amount);
        Task<IEnumerable<Bankaccount>> FilterAccounts(string? number, string? branch, string? document);
    }
}
