namespace Scheduler.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Scheduler.Data.Models;
    using Scheduler.Services.Dtos;
    using Scheduler.Web.ViewModels.UserViewModel;
    

    public interface IUserService
    {
        public Task<List<UserIdEmailsDto>> GetUserIds(IEnumerable<string> userEmails);

        public Task<ApplicationUser> Register(UserRegisterViewModel userViewModel);

        public Task<ApplicationUser> GetAppUser(string id);

        public Task<ApplicationUser> GetAppUser(UserLoginViewModel userViewModel);

    }
}
