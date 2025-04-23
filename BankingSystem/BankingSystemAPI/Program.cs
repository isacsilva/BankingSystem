
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace BankingSystemAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("BankingConnection")
                ?? throw new InvalidOperationException("Connection string 'BankingConnection' not found.");

            // Configure the database context with Pomelo + AutoDetect
            builder.Services.AddDbContext<BankingDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString) // Auto detecta a versão do MySQL
                )
            );

            // Dependency injection of services

            builder.Services.AddScoped<IBankAccountService, BankAccountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}