namespace Scheduler.Services.Interfaces
{
    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.UserViewModel;
    using System.Threading.Tasks;

    public interface IUserService
    {
        public Task<ApplicationUser> Register(UserRegisterViewModel userViewModel);

        public UserRegisterViewModel GetUserModel(string id);

        public ApplicationUser GetAppUser(string id);

        public ApplicationUser GetAppUser(UserLoginViewModel userViewModel);

    }
}
