﻿namespace Scheduler.Web.ConfigureServicesExtensins
{
    using System;
    using System.Reflection;

    using FluentValidation;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scheduler.Data;
    using Scheduler.Services;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Services.Messaging;
    using Scheduler.Web.ViewModels.Comments;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Web.ViewModels.UserViewModel;

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
           IConfiguration configuration,
           Assembly assembly)
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
            services.AddControllersWithViews().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssembly(assembly);
            });
            return services;
        }

        public static IServiceCollection AddValidators(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IValidator<InputCommentDto>, ImputComentValidator>()
                    .AddTransient<IValidator<EventAddViewModel>, EventAddValidator>()
                    .AddTransient<IValidator<UserLoginViewModel>, UserLoginValidator>()
                    .AddTransient<IValidator<UserRegisterViewModel>, UserRegisterValidator>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IUserService, UserService>()
                    .AddTransient<IEventService, EventService>()
                    .AddTransient<ICommentService, CommentService>()
                    .AddSingleton<IMapper, Mapper>()
                    .AddTransient<UriBuilder>()
                    .AddTransient<Services.Messaging.IEmailSender, NullMessageSender>();

            return services;
        }
    }
}
