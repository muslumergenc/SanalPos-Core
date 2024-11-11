using IsBankMvc.DataAccess.Contexts;
using IsBankMvc.DataAccess.Contracts;
using IsBankMvc.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IsBankMvc.DataAccess
{
    public static class Startup
    {
        public static IServiceCollection RegisterDataAccess(this IServiceCollection services)
        {
            //var server = "server";// EnvironmentHelper.Get("APP_DB_SERVER")!;
            //var database = "db"; //EnvironmentHelper.Get("APP_DB_NAME")!;
            //var username = "username";// EnvironmentHelper.Get("APP_DB_USER")!;
            //var password = "pass"; //EnvironmentHelper.Get("APP_DB_PASS")!;
            //var port = "port"; //EnvironmentHelper.Get("APP_DB_PORT")!;
            //var connectionString =$"Server={server}; Port={port};Database={database};User Id={username}; Password={password};";

            var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=AllPaymentDb;Trusted_Connection=True;MultipleActiveResultSets=true";


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseNpgsql(connectionString, builder =>
                //{
                //    builder.CommandTimeout(120);
                //    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                //});
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            });

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            return services;
        }
    }
}
