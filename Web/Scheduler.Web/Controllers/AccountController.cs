namespace Scheduler.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Facebook;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.Infrastructure.Oidc;
    using Scheduler.Web.ViewModels.UserViewModel;

    public class AccountController : BaseController
    {
        private const string homeUrl = "/";
        private const string externalAuthSchema = "Identity.External";
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IUserService userService, SignInManager<ApplicationUser> signInManager)
        {
            this.userService = userService;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterViewModel userViewModel)
        {
            var modelState = this.TryValidateModel(userViewModel);
            if (modelState == false)
            {
                return this.Redirect(homeUrl);
            }

            var appuser = this.userService.Register(userViewModel).Result;

            if (appuser != null)
            {
                await this.signInManager.SignInAsync(appuser, isPersistent: false);
                return this.Redirect(homeUrl);
            }
            else
            {
                return this.Redirect(homeUrl);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel userViewModel)
        {
            var modelState = this.TryValidateModel(userViewModel);
            if (modelState == false)
            {
                return this.Redirect(homeUrl);
            }

            var appUser = await this.userService.GetAppUser(userViewModel);
            if (appUser != null)
            {
                var result = await this.signInManager.CheckPasswordSignInAsync(appUser, userViewModel.Password, false);
                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(appUser, false);
                    return this.Redirect(homeUrl);
                }
            }
            else
            {
                return this.Redirect("/");
            }

            return default;
        }

        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            string authenticationScheme = string.Empty;

            authenticationScheme = provider switch
            {
               OidcProviderType.Facebook => FacebookDefaults.AuthenticationScheme,
               OidcProviderType.Google => GoogleDefaults.AuthenticationScheme,
               _=> throw new InvalidOperationException(),
            };
            var auth = new AuthenticationProperties
            {
                RedirectUri = this.Url.Action(nameof(this.LoginCallback), new { provider, returnUrl }),
            };

            return new ChallengeResult(authenticationScheme, auth);
        }

        public async Task<IActionResult> LoginCallback(string provider, string returnUrl = homeUrl)
        {
            AuthenticateResult authenticateResult = await this.HttpContext.AuthenticateAsync(externalAuthSchema);

            if (!authenticateResult.Succeeded)
            {
                return this.BadRequest();
            }

            var id = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name);
            var user = await this.userService.GetAppUserFromEmail(email.Value);

            if (user == null)
            {
                var registerModel = new UserRegisterViewModel()
                {
                    ConfirmPassword = null,
                    Email = email.Value,
                    Password = null,
                    FirstName = name.Value.Split(" ").ToArray()[0],
                    LastName = name.Value.Split(" ").ToArray()[1],
                };

                var loginInfo = new IdentityUserLogin<string>()
                {
                    LoginProvider = provider,
                    ProviderKey = id.Value,
                    ProviderDisplayName = name.Value,
                };
                var appUser = await this.userService.RegisterExternal(registerModel, loginInfo);
                await this.LoginExternal(appUser, provider);
            }
            else
            {
                await this.LoginExternal(user, provider);
            }

            return this.Redirect(homeUrl);
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            if (this.signInManager.IsSignedIn(this.User))
            {
                await this.signInManager.SignOutAsync();
            }

            return this.Redirect(homeUrl);
        }

        private async Task LoginExternal(ApplicationUser appUser, string provider)
        {
            var authenticationProperties = this.signInManager
                .ConfigureExternalAuthenticationProperties(provider, homeUrl, appUser.Id);
            await this.signInManager.SignInAsync(appUser, authenticationProperties);
        }
    }
}
