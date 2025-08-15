using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Fundo.Infrastructure.Data;
using Fundo.Infrastructure.Repositories;
using Fundo.Application.UseCases.Loans;
using FluentValidation;
using Fundo.Application.UseCases.Loans.Validators;
using Fundo.Domain.Repositories;
using Fundo.Domain.Services;
using Fundo.Domain.Factories;
using Serilog;
using Fundo.Applications.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) { _configuration = configuration; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            var jwtKey = _configuration["JWT:Key"] ?? "dev-secret-key-change-32-bytes-min-123456";
            var issuer = _configuration["JWT:Issuer"] ?? "loan-api";
            var audience = _configuration["JWT:Audience"] ?? "loan-ui";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = key
                    };
                });
            services.AddDbContext<LoanDbContext>(options =>
                options.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Application & Infrastructure DI
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ILoanHistoryRepository, LoanHistoryRepository>();
            services.AddScoped<CreateLoanHandler>();
            services.AddScoped<EditLoanHandler>();
            services.AddScoped<ILoanFactory, DefaultLoanFactory>();
            services.AddScoped<IPaymentService, Fundo.Application.Services.PaymentService>();
            services.AddScoped<IValidator<CreateLoanCommand>, CreateLoanCommandValidator>();
            services.AddScoped<IValidator<EditLoanCommand>, EditLoanCommandValidator>();
            services.AddScoped<IValidator<MakePaymentCommand>, MakePaymentCommandValidator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            // Ensure database is created and seeded
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LoanDbContext>();
                db.Database.EnsureCreated();
            }
        }
    }
}
