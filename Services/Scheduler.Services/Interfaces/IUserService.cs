namespace Scheduler.Services.Interfaces
{
    using System.Threading.Tasks;

    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.UserViewModel;
    

    public interface IUserService
    {
        public Task<ApplicationUser> Register(UserRegisterViewModel userViewModel);

        public Task<ApplicationUser> GetAppUser(string id);

        public Task<ApplicationUser> GetAppUser(UserLoginViewModel userViewModel);

    }
}
