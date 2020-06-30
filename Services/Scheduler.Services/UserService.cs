namespace Scheduler.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.UserViewModel;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> efDeletableRepositiry;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> efDeletableEntityRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.efDeletableRepositiry = efDeletableEntityRepository;
            this.mapper = mapper;
        }

        public async Task<ApplicationUser> GetAppUser(string id)
        {
            return await this.efDeletableRepositiry
                .All().Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> Register(UserRegisterViewModel userViewModel)
        {
            var appuser = this.mapper.MapAppUser(userViewModel);

            var result = await this.userManager.CreateAsync(appuser, userViewModel.Password);
            if (result.Succeeded)
            {
                return appuser;
            }
            else
            {
                return null;
            }
        }

        public async Task<ApplicationUser> GetAppUser(UserLoginViewModel userViewModel)
        {
            var user = await this.efDeletableRepositiry.All()
                .Where(u => u.Email == userViewModel.Email && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
