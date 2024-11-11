using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace IsBankMvc.Abstraction
{
    public static class Startup
    {
        public static IServiceCollection RegisterAbstraction(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, ConsoleLogger>();
            services.AddSingleton<IJsonService, JsonService>();
            services.AddSingleton<IHttpService, HttpService>();
            return services;
        }
    }
}
