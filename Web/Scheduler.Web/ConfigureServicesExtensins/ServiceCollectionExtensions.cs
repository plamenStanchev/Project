namespace Scheduler.Web.ConfigureServicesExtensins
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scheduler.Data;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCookiPolicy(
            this IServiceCollection services,
            IConfiguration configuration)
            => services.Configure<CookiePolicyOptions>(
            options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

        public static IServiceCollection AddIdentetyOptions(
            this IServiceCollection services,
            IConfiguration configuration)
            => services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 50;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            });

        public static IServiceCollection AddDataBase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services.AddDbContext<ApplicationDbContext>(
                 options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static IServiceCollection AddControllersWithViewsExtension(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddControllersWithViews(
                  options =>
                  {
                      options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
                  });

            services.AddControllersWithViews().AddJsonOptions(
               options =>
               {
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
               });
            return services;
        }
    }
}
