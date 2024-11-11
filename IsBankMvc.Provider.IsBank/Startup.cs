using IsBankMvc.Abstraction.Contracts.Payments;
using IsBankMvc.Provider.IsBank.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace IsBankMvc.Provider.IsBank
{
    public static class Startup
    {
        public static IServiceCollection RegisterIsBank(this IServiceCollection services)
        {
            services.AddScoped<IIsBankPaymentProvider, IsBankPaymentProvider>();

            return services;
        }
    }
}
