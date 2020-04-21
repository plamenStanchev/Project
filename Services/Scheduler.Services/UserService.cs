namespace Scheduler.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.UserViewModel;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> efDeletableRepositiry;
        private UserManager<ApplicationUser> userManager;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> efDeletableEntityRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.efDeletableRepositiry = efDeletableEntityRepository;
        }

        public ApplicationUser GetAppUser(string id)
        {
            return this.efDeletableRepositiry
                .All().Where(u => u.Id == id)
                .FirstOrDefault();
        }

        public UserRegisterViewModel GetUserModel(string id)
        {
            throw new System.NotImplementedException();
        }


        public async Task<ApplicationUser> Register(UserRegisterViewModel userViewModel)
        {
            var appuser = new ApplicationUser()
            {
                UserName = $"{userViewModel.FirstName} {userViewModel.LastName}",
                FistName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Email = userViewModel.Email,
                CreatedOn = DateTime.UtcNow,
            };
            //var passHash = this.userManager.PasswordHasher.HashPassword(appuser, userViewModel.Password);
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

        public ApplicationUser GetAppUser(UserLoginViewModel userViewModel)
        {
            var user = this.efDeletableRepositiry.All()
                .Where(u => u.Email == userViewModel.Email && u.IsDeleted == false)
                .FirstOrDefault();
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
