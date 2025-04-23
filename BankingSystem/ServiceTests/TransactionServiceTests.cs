using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private BankingDbContext _context;
        private ITransactionService _service;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new BankingDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _service = new TransactionService(_context);

            // Seed account with balance
            var account = new Bankaccount
            {
                Branch = "0001",
                Type = "CURRENT",
                HolderName = "João Silva",
                HolderEmail = "joao@email.com",
                HolderDocument = "12345678900",
                HolderType = "NATURAL",
                Status = "ACTIVE",
                Number = "100001"
            };

            _context.Bankaccounts.Add(account);
            _context.SaveChanges();

            _context.Balances.Add(new Balance
            {
                BankAccountId = account.Id,
                AvailableAmount = 1000,
                BlockedAmount = 0
            });
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task Credit_ShouldIncreaseBalanceAndRegisterTransaction()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var result = await _service.Credit(account.Id, 200);

            var balance = await _context.Balances.FindAsync(account.Id);
            Assert.AreEqual(1200, balance.AvailableAmount);
            Assert.AreEqual("CREDIT", result.Type);
        }

        [TestMethod]
        public async Task Debit_ShouldDecreaseBalanceAndRegisterTransaction()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var result = await _service.Debit(account.Id, 300);

            var balance = await _context.Balances.FindAsync(account.Id);
            Assert.AreEqual(700, balance.AvailableAmount);
            Assert.AreEqual("DEBIT", result.Type);
        }

        [TestMethod]
        public async Task GetByAccountId_ShouldReturnFilteredTransactions()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            await _service.Credit(account.Id, 100);
            await _service.Debit(account.Id, 50);

            var transactions = await _service.GetByAccountId(account.Id);
            Assert.AreEqual(2, transactions.Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectTransaction()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var credit = await _service.Credit(account.Id, 75);

            var result = await _service.GetById(credit.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("CREDIT", result.Type);
        }

        [TestMethod]
        public async Task GetByCounterpartyDocument_ShouldReturnMatchingTransactions()
        {
            var account = await _context.Bankaccounts.FirstAsync();

            _context.Transactions.Add(new Transaction
            {
                BankAccountId = account.Id,
                Type = "DEBIT",
                Amount = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CounterpartyHolderDocument = "99999999999"
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetByCounterpartyDocument("99999999999");
            Assert.AreEqual(1, result.Count());
        }
    }
}