namespace Scheduler.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Scheduler.Common;
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
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IValidator<UserRegisterViewModel> validator;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> efDeletableEntityRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager,
            IValidator<UserRegisterViewModel> validator)
        {
            this.userManager = userManager;
            this.efDeletableRepositiry = efDeletableEntityRepository;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.validator = validator;
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
            var validateReslt = this.validator.Validate(userViewModel);
            if (!validateReslt.IsValid)
            {
                return null;
            }

            var appuser = this.mapper.MapAppUser(userViewModel);

            var resultUser = await this.userManager.CreateAsync(appuser, userViewModel.Password);
            var resultRole = await this.AddRole(appuser, GlobalConstants.MemberRoleName);
            if (resultUser.Succeeded)
            {
                if (resultRole == false)
                {
                    return null;
                }

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
            var validateReslt = this.validator.Validate(userViewModel);
            if (!validateReslt.IsValid)
            {
                return null;
            }

            var applicationUser = this.mapper.MapAppUser(userViewModel);

            applicationUser.Logins.Add(userLogin);
            await this.userManager.CreateAsync(applicationUser);

            return applicationUser;
        }

        public async Task<bool> AddRole(ApplicationUser applicationUser, string newRoleName)
        {
            if (applicationUser == null || newRoleName == null)
            {
                return false;
            }

            var role = await this.roleManager.FindByNameAsync(newRoleName);
            if (role != null)
            {
                var result = await this.userManager.AddToRoleAsync(applicationUser, role.Name);

                if (result.Succeeded == false)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Log(string methodHitName,string mesage)
        {

        }
    }
}
