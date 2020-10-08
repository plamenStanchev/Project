namespace Scheduler.Services.Tests.Services.UserServiceTestAsets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Identity;
    using MockQueryable.Moq;
    using Moq;
    using Scheduler.Common;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Data.Tests.Services.UserServiceTestAsets;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.UserViewModel;
    using Xunit;

    public class UserServiceTests
    {
        public Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepository
            = new Mock<IDeletableEntityRepository<ApplicationUser>>();

        public IMapper mapper;
        public IUserService userService;
        public UserManager<ApplicationUser> manager;
        public RoleManager<ApplicationRole> roleManager;
        public IValidator<UserRegisterViewModel> validator;
        private List<ApplicationUser> data;

        public UserServiceTests()
        {
            this.mapper = new Mapper();
            this.manager = null;
            this.roleManager = null;
            this.validator = new UserRegisterValidator();
            this.userService = new UserService(
                                                this.mockUserRepository.Object,
                                                this.manager,
                                                this.mapper,
                                                this.roleManager,
                                                this.validator);
            this.PopulateData();
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234")]
        public async Task ShudGetUserId(string id)
        {
            var dbUsers = data.AsQueryable().BuildMockDbSet();
            this.mockUserRepository.Setup(a => a.All()).Returns(dbUsers.Object);

            var user = await this.userService.GetAppUser(id);

            if (id == "123")
            {
                Assert.Equal("Plamen", user.FirstName);
            }

            if (id == "1234")
            {
                Assert.Null(user);
            }
        }

        [Theory]
        [InlineData("Plamen", "some", "SomeEmail@abv.bg", "SomePass1_", "SomePass1_", "Plamen")]
        [InlineData("Plamen1", "", "SomeEmail2@abv.bg", "SomePass1_", "SomePass1_", null)]
        [InlineData("Plamen1", "ss", "SOmeThinsmaad@abv.bg", null, "SomePass1_", null)]
        public async Task ShudRegisterUser(
            string firstName,
            string lastName,
            string email,
            string confirmPassword,
            string password,
            string expectedResult)
        {
            var user = new UserRegisterViewModel()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                ConfirmPassword = confirmPassword,
                Password = password,
            };

            var mockStore = new Mock<IUserPasswordStore<ApplicationUser>>();
            var appUser = this.mapper.MapAppUser(user);

            mockStore.As<IUserEmailStore<ApplicationUser>>();
            mockStore
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            mockStore
                .Setup(s => s.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(appUser);

            var mockManager = MockHelpers.MockUserManager<ApplicationUser>(mockStore.Object);
            mockManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var mockRoleManager = MockHelpers.MockRoleManager<ApplicationRole>();
            this.SetUpMockRoleManagerFindByName(mockRoleManager);

            this.userService = new UserService(this.mockUserRepository.Object, mockManager.Object, this.mapper, mockRoleManager.Object, new UserRegisterValidator());

            var result = await this.userService.Register(user);

            if (expectedResult == null)
            {
                Assert.Null(result);
            }
            else
            {
                Assert.Equal(expectedResult, result.FirstName);
            }
        }

        [Theory]
        [InlineData(2, "SomeEmail@some.com", "SomeEmail1@some.com", "SomeEmail2@some.com")]
        [InlineData(1, "SomeEmail@some.com")]
        [InlineData(0, "SomeEmail2@some.com")]
        public async Task ShudGetUserIds(int expectedResult, params string[] emails)
        {
            var mockSet = this.data.AsQueryable().BuildMockDbSet();
            this.mockUserRepository.Setup(u => u.All())
                .Returns(mockSet.Object);

            var result = await this.userService.GetUserIds(emails);
            Assert.Equal(expectedResult, result.Count);
        }

        [Theory]
        [InlineData("SomeEmail@some.com", "123")]
        [InlineData("SomeEmail1@some.com", "SomeOtherId")]
        [InlineData("SomeEmail2@some.com", null)]
        public async Task ShudGetAppUserFromEmail(string email, string expectedResult)
        {
            this.SetUpAllMethodInRepo();
            var result = await this.userService.GetAppUserFromEmail(email);

            if (email == "SomeEmail2@some.com")
            {
                Assert.Null(result);
            }
            else
            {
                Assert.Equal(result.Id, expectedResult);
            }
        }

        [Fact]
        public async Task ShudChangeUserRole()
        {

            var roleAdmin = new ApplicationRole()
            {
                Id = "5",
                Name = GlobalConstants.AdministratorRoleName,
            };
            var roleMember = new ApplicationRole()
            {
                Id = "6",
                Name = GlobalConstants.MemberRoleName,
            };

            var appUser = new ApplicationUser()
            {
                Id = "some",
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@something.com",
                Roles = new List<IdentityUserRole<string>>(),
            };

            var mockRoleManager = MockHelpers.MockRoleManager<ApplicationRole>();
            this.SetUpMockRoleManagerFindByName(mockRoleManager);

            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockManagaer = MockHelpers.MockUserManager<ApplicationUser>(mockStore.Object);
            mockManagaer
                .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();

            this.userService = new UserService(this.mockUserRepository.Object, mockManagaer.Object, this.mapper, mockRoleManager.Object, this.validator);
            var resutl = await this.userService.AddRole(appUser, roleAdmin.Name);
            Assert.True(resutl);
        }

        private void SetUpMockRoleManagerFindByName(Mock<RoleManager<ApplicationRole>> mockRoleManager)
        {
            mockRoleManager.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync<string, RoleManager<ApplicationRole>, ApplicationRole>((value) =>
                {
                    var returnResult = this.RoleFindNameInMemorySetup(value);
                    return returnResult;
                });

        }

        private void SetUpAllMethodInRepo()
        {
            this.PopulateData();
            var mockset = this.data.AsQueryable().BuildMockDbSet();
            this.mockUserRepository.Setup(u => u.All())
                .Returns(mockset.Object);
        }

        private void PopulateData()
        {
            this.data = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "123",
                    FirstName = "Plamen",
                    IsDeleted = false,
                    Email = "SomeEmail@some.com",
                },
                new ApplicationUser()
                {
                    Id = "SomeOtherId",
                    FirstName = "ShudentreturnMe",
                    IsDeleted = false,
                    Email = "SomeEmail1@some.com",
                },
                new ApplicationUser()
                {
                    Id = "1234",
                    FirstName = "ShudentreturnMe",
                    IsDeleted = true,
                    Email = "SomeEmail2@some.com",
                },
                new ApplicationUser()
                {
                    Id = "1234567",
                    FirstName = "SomeDumy",
                    IsDeleted = false,
                    Email = "SomeEmail3@some.com",
                },
            };
        }

        private ApplicationRole RoleFindNameInMemorySetup(string rolename)
        {
            ApplicationRole returnResult = null;
            if (rolename == GlobalConstants.AdministratorRoleName)
            {
                returnResult = new ApplicationRole(GlobalConstants.AdministratorRoleName);
            }
            else if (rolename == GlobalConstants.MemberRoleName)
            {
                returnResult = new ApplicationRole(GlobalConstants.MemberRoleName);
            }

            var someStoper = new ApplicationRole();
            return returnResult;

        }
    }
}
