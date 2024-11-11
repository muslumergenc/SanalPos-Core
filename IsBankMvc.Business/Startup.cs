using IsBankMvc.Abstraction.Interfaces.Payments;
using IsBankMvc.Business.Implementation;
using IsBankMvc.Provider.IsBank;
using Microsoft.Extensions.DependencyInjection;

namespace IsBankMvc.Business
{
    public static class Startup
    {
        public static IServiceCollection RegisterBusiness(this IServiceCollection services)
        {
            services.AddScoped<IPaymentService, PaymentService>();
            services.RegisterIsBank();   
            return services;
        }
    }
}
