namespace Scheduler.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.UserViewModel;

    public class AccountController : BaseController
    {
        private const string homeUrl = "/";
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

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            if (this.signInManager.IsSignedIn(this.User))
            {
                await this.signInManager.SignOutAsync();
            }

            return this.Redirect("/");
        }
    }
}
