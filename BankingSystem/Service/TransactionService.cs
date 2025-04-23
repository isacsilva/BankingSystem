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

    public class TransactionService : ITransactionService
    {
        private readonly BankingDbContext _context;

        public TransactionService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> GetById(int transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(int accountId, DateTime? from = null, DateTime? to = null, string type = null)
        {
            var query = _context.Transactions.AsQueryable();

            query = query.Where(t => t.BankAccountId == accountId);

            if (from.HasValue)
                query = query.Where(t => t.CreatedAt >= from);

            if (to.HasValue)
                query = query.Where(t => t.CreatedAt <= to);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(t => t.Type == type);

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByCounterpartyDocument(string document)
        {
            return await _context.Transactions
                .Where(t => t.CounterpartyHolderDocument == document)
                .ToListAsync();
        }

        public async Task<Transaction> Credit(int accountId, decimal amount)
        {
            var account = await _context.Bankaccounts.Include(b => b.Balance).FirstOrDefaultAsync(b => b.Id == accountId);
            if (account == null || account.Status != "ACTIVE") return null;
            if (amount <= 0) return null;

            account.Balance.AvailableAmount += amount;

            var transaction = new Transaction
            {
                BankAccountId = accountId,
                Type = "CREDIT",
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> Debit(int accountId, decimal amount)
        {
            var account = await _context.Bankaccounts.Include(b => b.Balance).FirstOrDefaultAsync(b => b.Id == accountId);
            if (account == null || account.Status != "ACTIVE") return null;
            if (amount <= 0) return null;
            if (account.Balance.AvailableAmount < amount) return null;

            account.Balance.AvailableAmount -= amount;

            var transaction = new Transaction
            {
                BankAccountId = accountId,
                Type = "DEBIT",
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
