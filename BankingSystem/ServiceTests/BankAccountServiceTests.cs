using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ServiceTests
{
    [TestClass]
    public class BankAccountServiceTests
    {
        private BankingDbContext _context;
        private IBankAccountService _service;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // cria um banco novo a cada teste
            .Options;

            _context = new BankingDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _service = new BankAccountService(_context);

            // Seed initial account
            _service.CreateAccount(new Bankaccount
            {
                Branch = "0001",
                Type = "CURRENT",
                HolderName = "João Silva",
                HolderEmail = "joao@email.com",
                HolderDocument = "12345678900",
                HolderType = "NATURAL"
            }).Wait();
        }

        [TestMethod]
        public async Task CreateAccount_ShouldGenerateNumberAndCreateBalance()
        {
            var account = new Bankaccount
            {
                Branch = "0002",
                Type = "PAYMENT",
                HolderName = "Maria Teste",
                HolderEmail = "maria@email.com",
                HolderDocument = "22222222222",
                HolderType = "NATURAL"
                // Status, Number e UpdatedAt serão preenchidos pelo serviço
            };

            var created = await _service.CreateAccount(account);

            Assert.IsNotNull(created);
            Assert.IsFalse(string.IsNullOrEmpty(created.Number));
            Assert.AreEqual("ACTIVE", created.Status);

            var balance = await _context.Balances.FindAsync(created.Id);
            Assert.IsNotNull(balance);
        }

        [TestMethod]
        public async Task GetByAccountNumber_ShouldReturnCorrectAccount()
        {
            var existing = await _context.Bankaccounts.FirstAsync();
            var result = await _service.GetByAccountNumber(existing.Number);

            Assert.IsNotNull(result);
            Assert.AreEqual(existing.Number, result.Number);
        }

        [TestMethod]
        public async Task GetByBranch_ShouldReturnAccounts()
        {
            var results = await _service.GetByBranch("0001");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public async Task GetByHolderDocument_ShouldReturnAccounts()
        {
            var results = await _service.GetByHolderDocument("12345678900");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public async Task UpdateEmail_ShouldChangeEmail()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var result = await _service.UpdateEmail(account.Id, "novo@email.com");

            var updated = await _context.Bankaccounts.FindAsync(account.Id);

            Assert.IsTrue(result);
            Assert.AreEqual("novo@email.com", updated.HolderEmail);
        }

        [TestMethod]
        public async Task UpdateStatus_ShouldChangeStatus()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var result = await _service.UpdateStatus(account.Id, "BLOCKED");

            var updated = await _context.Bankaccounts.FindAsync(account.Id);

            Assert.IsTrue(result);
            Assert.AreEqual("BLOCKED", updated.Status);
        }

        [TestMethod]
        public async Task CloseAccount_ShouldUpdateStatusToFinished()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var result = await _service.CloseAccount(account.Id);

            var updated = await _context.Bankaccounts.FindAsync(account.Id);

            Assert.IsTrue(result);
            Assert.AreEqual("FINISHED", updated.Status);
        }

        [TestMethod]
        public async Task GetBalance_ShouldReturnCorrectValues()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var balance = await _context.Balances.FindAsync(account.Id);
            balance.AvailableAmount = 500;
            balance.BlockedAmount = 100;
            await _context.SaveChangesAsync();

            var result = await _service.GetBalance(account.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.AvailableAmount);
            Assert.AreEqual(100, result.BlockedAmount);
        }

        [TestMethod]
        public async Task HoldAmount_ShouldCreateTransactionAndUpdateBalance()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var balance = await _context.Balances.FirstAsync();
            balance.AvailableAmount = 1000;
            _context.SaveChanges();

            var result = await _service.HoldAmount(account.Id, 200);

            var updatedBalance = await _context.Balances.FindAsync(account.Id);
            var transaction = _context.Transactions.FirstOrDefault(t => t.BankAccountId == account.Id && t.Type == "AMOUNT_HOLD");

            Assert.IsTrue(result);
            Assert.AreEqual(800, updatedBalance.AvailableAmount);
            Assert.AreEqual(200, updatedBalance.BlockedAmount);
            Assert.IsNotNull(transaction);
        }

        [TestMethod]
        public async Task ReleaseAmount_ShouldCreateTransactionAndUpdateBalance()
        {
            var account = await _context.Bankaccounts.FirstAsync();
            var balance = await _context.Balances.FirstAsync();
            balance.AvailableAmount = 800;
            balance.BlockedAmount = 200;
            _context.SaveChanges();

            var result = await _service.ReleaseAmount(account.Id, 150);

            var updatedBalance = await _context.Balances.FindAsync(account.Id);
            var transaction = _context.Transactions.FirstOrDefault(t => t.BankAccountId == account.Id && t.Type == "AMOUNT_RELEASE");

            Assert.IsTrue(result);
            Assert.AreEqual(950, updatedBalance.AvailableAmount);
            Assert.AreEqual(50, updatedBalance.BlockedAmount);
            Assert.IsNotNull(transaction);
        }

        [TestMethod]
        public async Task Transfer_ShouldCreateCreditAndDebitTransactions()
        {
            var toAccount = new Bankaccount
            {
                Branch = "0001",
                Type = "CURRENT",
                HolderName = "Maria Souza",
                HolderEmail = "maria@email.com",
                HolderDocument = "98765432100",
                HolderType = "NATURAL"
            };
            var created = await _service.CreateAccount(toAccount);

            var from = await _context.Bankaccounts.FirstAsync();
            var fromBalance = await _context.Balances.FindAsync(from.Id);
            fromBalance.AvailableAmount = 1000;
            _context.SaveChanges();

            var success = await _service.Transfer(from.Id, created.Number, 250);

            var fromUpdated = await _context.Balances.FindAsync(from.Id);
            var toUpdated = await _context.Balances.FindAsync(created.Id);

            var debit = _context.Transactions.FirstOrDefault(t => t.BankAccountId == from.Id && t.Type == "DEBIT");
            var credit = _context.Transactions.FirstOrDefault(t => t.BankAccountId == created.Id && t.Type == "CREDIT");

            Assert.IsTrue(success);
            Assert.AreEqual(750, fromUpdated.AvailableAmount);
            Assert.AreEqual(250, toUpdated.AvailableAmount);
            Assert.IsNotNull(debit);
            Assert.IsNotNull(credit);
        }
    }
}