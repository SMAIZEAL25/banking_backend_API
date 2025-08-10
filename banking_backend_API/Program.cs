
using BankingApp.Application.Mappings;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Database;
using BankingApp.Infrastruture.Integration;
using BankingApp.Infrastruture.Redis;
using BankingApp.Infrastruture.Repostries.IRepositories;
using BankingApp.Infrastruture.Repostries.Repository;
using BankingApp.Infrastruture.Services;
using BankingApp.Infrastruture.Services.Interface;
using BankingApp.Infrastruture.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Http.Headers;

namespace banking_backend_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Sql Database
            builder.Services.AddDbContext<BankingDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });

            //Serilog Configuration
            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "BankingApp_"; // Key prefix
            });

            builder.Services.AddHttpClient<IPaymentGateway, PaystackService>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();



            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            // AtuMapper Config 
            builder.Services.AddAutoMapper(typeof(MappinConfigs));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
