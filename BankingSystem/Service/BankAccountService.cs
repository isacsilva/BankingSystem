using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Core;

namespace Service
{

    public class BankAccountService : IBankAccountService
    {
        private readonly BankingDbContext _context;

        public BankAccountService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Bankaccount> CreateAccount(Bankaccount account)
        {
            if (string.IsNullOrWhiteSpace(account.Branch) ||
                string.IsNullOrWhiteSpace(account.Type) ||
                string.IsNullOrWhiteSpace(account.HolderName) ||
                string.IsNullOrWhiteSpace(account.HolderEmail) ||
                string.IsNullOrWhiteSpace(account.HolderDocument) ||
                string.IsNullOrWhiteSpace(account.HolderType))
            {
                throw new ArgumentException("Campos obrigatórios estão faltando.");
            }


            account.Number = await GenerateUniqueAccountNumber();
            account.Status = "ACTIVE";
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;

            _context.Bankaccounts.Add(account);
            await _context.SaveChangesAsync();
            _context.Entry(account).State = EntityState.Detached;

            var balance = new Balance
            {
                BankAccountId = account.Id,
                AvailableAmount = 0m,
                BlockedAmount = 0m
            };

            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Bankaccount> GetByAccountNumber(string accountNumber)
        {
            return await _context.Bankaccounts
                .Include(b => b.Balance)
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Number == accountNumber);
        }

        public async Task<IEnumerable<Bankaccount>> GetByBranch(string branch)
        {
            return await _context.Bankaccounts
                .Where(b => b.Branch == branch)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bankaccount>> GetByHolderDocument(string document)
        {
            return await _context.Bankaccounts
                .Where(b => b.HolderDocument == document)
                .ToListAsync();
        }

        public async Task<bool> UpdateEmail(int accountId, string newEmail)
        {
            var account = await _context.Bankaccounts.FindAsync(accountId);
            if (account == null) return false;

            account.HolderEmail = newEmail;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatus(int accountId, string newStatus)
        {
            var account = await _context.Bankaccounts.FindAsync(accountId);
            if (account == null) return false;

            account.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseAccount(int accountId)
        {
            var account = await _context.Bankaccounts.FindAsync(accountId);
            if (account == null) return false;

            account.Status = "FINISHED";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Balance> GetBalance(int accountId)
        {
            return await _context.Balances.FirstOrDefaultAsync(b => b.BankAccountId == accountId);
        }

        public async Task<bool> HoldAmount(int accountId, decimal amount)
        {
            var balance = await _context.Balances.FirstOrDefaultAsync(b => b.BankAccountId == accountId);
            if (balance == null || balance.AvailableAmount < amount)
                return false;

            balance.AvailableAmount -= amount;
            balance.BlockedAmount += amount;

            _context.Transactions.Add(new Transaction
            {
                Type = "AMOUNT_HOLD",
                Amount = amount,
                BankAccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseAmount(int accountId, decimal amount)
        {
            var balance = await _context.Balances.FirstOrDefaultAsync(b => b.BankAccountId == accountId);
            if (balance == null || balance.BlockedAmount < amount)
                return false;

            balance.BlockedAmount -= amount;
            balance.AvailableAmount += amount;

            _context.Transactions.Add(new Transaction
            {
                Type = "AMOUNT_RELEASE",
                Amount = amount,
                BankAccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Transfer(int fromAccountId, string toAccountNumber, decimal amount)
        {
            var fromAccount = await _context.Bankaccounts.Include(a => a.Balance)
                .FirstOrDefaultAsync(a => a.Id == fromAccountId && a.Status == "ACTIVE");

            var toAccount = await _context.Bankaccounts.Include(a => a.Balance)
                .FirstOrDefaultAsync(a => a.Number == toAccountNumber && a.Status == "ACTIVE");

            if (fromAccount == null || toAccount == null)
                return false;

            if (fromAccount.Balance.AvailableAmount < amount)
                return false;

            
            fromAccount.Balance.AvailableAmount -= amount;
            toAccount.Balance.AvailableAmount += amount;

            
            _context.Transactions.Add(new Transaction
            {
                Type = "DEBIT",
                Amount = amount,
                BankAccountId = fromAccount.Id,
                CounterpartyBankCode = "270",
                CounterpartyBankName = "MVP Bank",
                CounterpartyBranch = toAccount.Branch,
                CounterpartyAccountNumber = toAccount.Number,
                CounterpartyAccountType = toAccount.Type,
                CounterpartyHolderName = toAccount.HolderName,
                CounterpartyHolderType = toAccount.HolderType,
                CounterpartyHolderDocument = toAccount.HolderDocument,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

          
            _context.Transactions.Add(new Transaction
            {
                Type = "CREDIT",
                Amount = amount,
                BankAccountId = toAccount.Id,
                CounterpartyBankCode = "270",
                CounterpartyBankName = "MVP Bank",
                CounterpartyBranch = fromAccount.Branch,
                CounterpartyAccountNumber = fromAccount.Number,
                CounterpartyAccountType = fromAccount.Type,
                CounterpartyHolderName = fromAccount.HolderName,
                CounterpartyHolderType = fromAccount.HolderType,
                CounterpartyHolderDocument = fromAccount.HolderDocument,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Bankaccount>> FilterAccounts(string? number, string? branch, string? document)
        {
            var query = _context.Bankaccounts
                .Include(b => b.Balance)
                .Include(b => b.Transactions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(number))
                query = query.Where(b => b.Number == number);

            if (!string.IsNullOrEmpty(branch))
                query = query.Where(b => b.Branch == branch);

            if (!string.IsNullOrEmpty(document))
                query = query.Where(b => b.HolderDocument == document);

            return await query.ToListAsync();
        }

        private async Task<string> GenerateUniqueAccountNumber()
        {
            string number;
            bool exists;

            do
            {
                number = new Random().Next(100000, 999999).ToString(); // 6 dígitos
                exists = await _context.Bankaccounts.AnyAsync(a => a.Number == number);
            }
            while (exists);

            return number;
        }
    }

}
