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
            services.AddDbContext<LoanDbContext>(options =>
                options.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Application & Infrastructure DI
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<CreateLoanHandler>();
            services.AddScoped<ILoanFactory, DefaultLoanFactory>();
            services.AddScoped<IPaymentService, Fundo.Application.Services.PaymentService>();
            services.AddScoped<IValidator<CreateLoanCommand>, CreateLoanCommandValidator>();
            services.AddScoped<IValidator<MakePaymentCommand>, MakePaymentCommandValidator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors("AllowAll");
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
