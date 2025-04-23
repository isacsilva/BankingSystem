using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class BankingDbContext : DbContext
{
    public BankingDbContext()
    {
    }

    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Bankaccount> Bankaccounts { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=123456;database=bankingdb", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.25-mysql"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(e => e.BankAccountId).HasName("PRIMARY");

            entity.ToTable("balance");

            modelBuilder.Entity<Balance>()
                .HasKey(b => b.BankAccountId);

            modelBuilder.Entity<Balance>()
                .HasOne(b => b.BankAccount)
                .WithOne(a => a.Balance)
                .HasForeignKey<Balance>(b => b.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.BankAccountId)
                .ValueGeneratedNever()
                .HasColumnName("bankAccountId");
            entity.Property(e => e.AvailableAmount)
                .HasPrecision(18, 2)
                .HasColumnName("availableAmount");
            entity.Property(e => e.BlockedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("blockedAmount");

            entity.HasOne(d => d.BankAccount).WithOne(p => p.Balance)
                .HasForeignKey<Balance>(d => d.BankAccountId)
                .HasConstraintName("balance_ibfk_1");
        });

        modelBuilder.Entity<Bankaccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bankaccount");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => e.Number, "Number").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Branch)
                .HasMaxLength(5)
                .HasColumnName("branch");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.HolderDocument)
                .HasMaxLength(50)
                .HasColumnName("holderDocument");
            entity.Property(e => e.HolderEmail)
                .HasMaxLength(200)
                .HasColumnName("holderEmail");
            entity.Property(e => e.HolderName)
                .HasMaxLength(200)
                .HasColumnName("holderName");
            entity.Property(e => e.HolderType)
                .HasColumnType("enum('NATURAL','LEGAL')")
                .HasColumnName("holderType");
            entity.Property(e => e.Number)
                .HasMaxLength(10)
                .HasColumnName("number");
            entity.Property(e => e.Status)
                .HasColumnType("enum('ACTIVE','BLOCKED','FINISHED')")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasColumnType("enum('PAYMENT','CURRENT')")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.BankAccountId, "bankAccountId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BankAccountId).HasColumnName("bankAccountId");
            entity.Property(e => e.CounterpartyAccountNumber)
                .HasMaxLength(20)
                .HasColumnName("counterpartyAccountNumber");
            entity.Property(e => e.CounterpartyAccountType)
                .HasColumnType("enum('PAYMENT','CURRENT')")
                .HasColumnName("counterpartyAccountType");
            entity.Property(e => e.CounterpartyBankCode)
                .HasMaxLength(10)
                .HasColumnName("counterpartyBankCode");
            entity.Property(e => e.CounterpartyBankName)
                .HasMaxLength(100)
                .HasColumnName("counterpartyBankName");
            entity.Property(e => e.CounterpartyBranch)
                .HasMaxLength(10)
                .HasColumnName("counterpartyBranch");
            entity.Property(e => e.CounterpartyHolderDocument)
                .HasMaxLength(50)
                .HasColumnName("counterpartyHolderDocument");
            entity.Property(e => e.CounterpartyHolderName)
                .HasMaxLength(200)
                .HasColumnName("counterpartyHolderName");
            entity.Property(e => e.CounterpartyHolderType)
                .HasColumnType("enum('NATURAL','LEGAL')")
                .HasColumnName("counterpartyHolderType");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Type)
                .HasColumnType("enum('CREDIT','DEBIT','AMOUNT_HOLD','AMOUNT_RELEASE')")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BankAccountId)
                .HasConstraintName("transaction_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
