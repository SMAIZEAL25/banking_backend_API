using BankingApp.Application.Features.Transactions.Withdrawal;
using BankingApp.Application.Mappings;
using BankingApp.Infrastructure.Database;
using BankingApp.Infrastructure.IRateLimiter;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastructure.RateLimiter;
using BankingApp.Infrastructure.Repositories;
using BankingApp.Infrastruture.Integration;
using BankingApp.Infrastruture.Redis;
using BankingApp.Infrastruture.Repostries.IRepositories;
using BankingApp.Infrastruture.Repostries.Repository;
using BankingApp.Infrastruture.Services;
using BankingApp.Infrastruture.Services.Interfaces;
using BankingApp.Application.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using BankingApp.Infrastructure.UnitOfWork;

namespace banking_backend_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog (if configured via appsettings)
            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
            });

            // --- Controllers & API Explorer ---
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // --- Database Contexts ---
            builder.Services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

            builder.Services.AddDbContext<BankingAuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IRechargeAuthDB")));

            // --- Identity + Roles ---
            builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BankingAuthDbContext>()
                .AddDefaultTokenProviders();

            // --- JWT Authentication ---
            var jwtKey = builder.Configuration["JwtSettings:Key"];
            var issuer = builder.Configuration["JwtSettings:Issuer"];
            var audience = builder.Configuration["JwtSettings:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // --- Swagger + JWT Security ---
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "Banking API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            // --- Redis Cache (Singleton recommended for connection) ---
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "BankingApp_";
            });
            // If you have a wrapper service around Redis, register as singleton (stateless)
            builder.Services.AddSingleton<ICacheService, RedisCacheService>();

            // --- HttpClient for payment gateway ---
            builder.Services.AddHttpClient<IPaymentGateway, PaystackService>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            // --- Health Checks ---
            builder.Services.AddHealthChecks()
                .AddSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"), name: "main-db", healthQuery: "SELECT 1;", tags: new[] { "database" })
                .AddSqlServer(builder.Configuration.GetConnectionString("IRechargeAuthDB"), name: "auth-db", healthQuery: "SELECT 1;", tags: new[] { "database", "auth" });

            builder.Services.AddHealthChecksUI(options =>
            {
                options.AddHealthCheckEndpoint("API", "/health");
                options.AddHealthCheckEndpoint("Main DB", "/health/maindb");
                options.AddHealthCheckEndpoint("Auth DB", "/health/authdb");
                options.SetEvaluationTimeInSeconds(30);
            })
            .AddSqlServerStorage(builder.Configuration.GetConnectionString("HealthChecksUI"));

            // --- CORS ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // --- Repositories & Unit of Work & Services ---
            // Generic repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Specific repos
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountingHistoryRepository, AccountingHistoryRepository>();

            // Unit of Work uses the concrete IAccountRepository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services / application layer
            builder.Services.AddScoped<IAccountTransfer, AccountTransfer>();
            builder.Services.AddScoped<IBankingService, BankingService>();
            builder.Services.AddScoped<IViewAccountBalance, ViewAccountBalance>();
            

            // --- AuthManager ---
            builder.Services.AddScoped<IAuthManager, AuthManager>();

            // --- AutoMapper ---
            builder.Services.AddAutoMapper(typeof(MappinConfigs));

            // --- FluentValidation ---
            // Scans assembly that contains your validators. Replace with one of your validators' type if different.
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(typeof(BankingApp.Application.AssemblyReference).Assembly);

            // --- Rate Limiter ---
            builder.Services.AddSingleton<IRateLimiter>(provider =>
                new TokenBucketRateLimiter(maxRequests: 5, timeWindowInSeconds: 1));


            builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssembly(typeof(BankingApp.Application.AssemblyReference).Assembly));

            var app = builder.Build();

            // --- Middleware & pipeline ordering ---
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            // Rate limiter before auth to short-circuit abusive clients
            app.UseMiddleware<RateLimitingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            // --- Health Check endpoints ---
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/maindb", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("database"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/authdb", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("auth"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
                options.ApiPath = "/health-api";
            });

            app.MapControllers();
            app.Run();
        }
    }
}
