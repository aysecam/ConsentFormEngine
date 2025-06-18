using Microsoft.Extensions.DependencyInjection;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.Concrete;
using ConsentFormEngine.Business.Mailing;
using ConsentFormEngine.Business.Security.JWT;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Mailing;
using ConsentFormEngine.Core.Services;
using ConsentFormEngine.DataAccess.Repositories;

namespace ConsentFormEngine.Business.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddScoped<ICountryService, CountryManager>();
            services.AddScoped<ICityService, CityManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            //services.AddScoped<IUserService, UserManager>();
            //services.AddScoped<IFormEntryService, FormEntryManager>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IMailService, MailManager>();
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<ITokenHelper, JwtHelper>();

            return services;
        }
    }
}
