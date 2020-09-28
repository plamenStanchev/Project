namespace Scheduler.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;

    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Freands()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string email)
        {
            var friendForAdd = await this.userService.GetAppUserFromEmail(email);
            var curentUserId = this.userManager.GetUserId(this.User);
            var curentUser = await this.userService.GetAppUser(curentUserId);

            if (friendForAdd == null)
            {
                return this.BadRequest();
            }

            return await this.Freands();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend()
        {
            return await this.Freands();
        }
    }
}
