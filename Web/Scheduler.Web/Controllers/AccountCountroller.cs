namespace Scheduler.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.UserViewModel;
    using System.Threading.Tasks;

    public class AccountCountroller : BaseController
    {
        private const string Url = "/";
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountCountroller(IUserService userService, SignInManager<ApplicationUser> signInManager)
        {
            this.userService = userService;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            var appuser = this.userService.Register(userViewModel).Result;

            if (appuser != null)
            {
                await this.signInManager.SignInAsync(appuser, isPersistent: false);
                return this.Redirect(Url);
            }
            else
            {
                return this.Redirect("/");
            }
        }

  
    }
}
