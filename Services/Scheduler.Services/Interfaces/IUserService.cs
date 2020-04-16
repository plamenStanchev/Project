namespace Scheduler.Services.Interfaces
{
    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.UserViewModel;
    using System.Threading.Tasks;

    public interface IUserService
    {
        public Task<ApplicationUser> Register(UserViewModel userViewModel);

        public UserViewModel GetUserModel(string id);

        public ApplicationUser GetAppUser(string id);


    }
}
