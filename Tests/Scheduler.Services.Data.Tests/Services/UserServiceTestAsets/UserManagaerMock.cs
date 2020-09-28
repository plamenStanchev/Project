namespace Scheduler.Services.Data.Tests.UserServiceTestAsets
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;

    public static class UserManagaerMock
    {

        public static UserManager<TUser> CreateUserManager<TUser>(Mock<IUserPasswordStore<TUser>> userPasswordStore)
            where TUser : class
        {
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();

            idOptions.Password.RequireDigit = true;
            idOptions.Lockout.AllowedForNewUsers = true;
            idOptions.Lockout.MaxFailedAccessAttempts = 50;
            //idOptions.User.RequireUniqueEmail = true;
            idOptions.User.AllowedUserNameCharacters += ' ';

            idOptions.SignIn.RequireConfirmedEmail = false;

            // Lockout settings.
            idOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            idOptions.Lockout.MaxFailedAccessAttempts = 50;
            idOptions.Lockout.AllowedForNewUsers = true;

            idOptions.ClaimsIdentity.UserNameClaimType = string.Empty;
            idOptions.ClaimsIdentity.UserIdClaimType = string.Empty;
            idOptions.ClaimsIdentity.SecurityStampClaimType = string.Empty;
            idOptions.ClaimsIdentity.RoleClaimType = string.Empty;
            idOptions.Tokens.AuthenticatorIssuer = string.Empty;

            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            UserValidator<TUser> validator = new UserValidator<TUser>();
            userValidators.Add(validator);

            var passValidator = new PasswordValidator<TUser>();
            var pwdValidators = new List<IPasswordValidator<TUser>>();
            pwdValidators.Add(passValidator);
            var userManager = new UserManager<TUser>(
                userPasswordStore.Object,
                options.Object,
                new PasswordHasher<TUser>(),
                null,
                pwdValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            return userManager;
        }
    }
}
