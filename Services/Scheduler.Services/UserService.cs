namespace Scheduler.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Dtos;
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
                .All().Where(u => u.Id == id
                && u.IsDeleted == false)
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

        public async Task<List<UserIdEmailsDto>> GetUserIds(IEnumerable<string> userEmails)
        {
            var userIdsAndEmail = await this.efDeletableRepositiry.All()
                .Where(u => userEmails.Contains(u.Email)
                && u.IsDeleted == false)
                .Select(e => new UserIdEmailsDto()
                {
                    Email = e.Email,
                    Id = e.Id,
                }).ToListAsync();

            return userIdsAndEmail;
        }

        public async Task<ApplicationUser> GetAppUserFromEmail(string email)
        {
            var applicationUser = await this.efDeletableRepositiry.All()
                .FirstOrDefaultAsync(ap => ap.Email == email
                && ap.IsDeleted == false);
            return applicationUser;
        }

        public async Task<ApplicationUser> RegisterExternal(UserRegisterViewModel userViewModel, IdentityUserLogin<string> userLogin)
        {
            var applicationUser = this.mapper.MapAppUser(userViewModel);

            applicationUser.Logins.Add(userLogin);
            await this.userManager.CreateAsync(applicationUser);

            return applicationUser;
        }
    }
}
