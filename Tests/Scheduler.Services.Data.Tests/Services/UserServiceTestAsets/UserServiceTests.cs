namespace Scheduler.Services.Data.UserServiceTestAsets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using MockQueryable.Moq;
    using Moq;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Data.Tests.UserServiceTestAsets;
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
        private List<ApplicationUser> data;

        public UserServiceTests()
        {
            this.mapper = new Mapper();
            this.manager = null;
            this.userService = new UserService(
                                                this.mockUserRepository.Object,
                                                this.manager,
                                                this.mapper
                                                );
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

        [Fact]
        public async Task ShudRegisterUser()
        {
            var user = new UserRegisterViewModel()
            {
                FirstName = "Plamen",
                LastName = "some",
                Email = "SomeEmail@abv.bg",
                ConfirmPassword = "SomePass1_",
                Password = "SomePass1_",
            };

            var moCkStore = new Mock<IUserPasswordStore<ApplicationUser>>();
            var appUser = mapper.MapAppUser(user);

            moCkStore.As<IUserEmailStore<ApplicationUser>>();
            moCkStore
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            moCkStore
                .Setup(s => s.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(appUser);

            var userManeger = UserManagaerMock.CreateUserManager<ApplicationUser>(moCkStore);
            this.manager = userManeger;

            this.userService = new UserService(this.mockUserRepository.Object, userManeger, this.mapper);

            var result = await this.userService.Register(user);
            Assert.Equal("Plamen", result.FirstName);

        }

        [Theory]
        [InlineData(2,"SomeEmail@some.com", "SomeEmail1@some.com", "SomeEmail2@some.com")]
        [InlineData(1, "SomeEmail@some.com")]
        [InlineData(0,"SomeEmail2@some.com")]
        public async Task ShudGetUserIds(int expectedResult,params string[] emails)
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
        public async Task ShudGetAppUserFromEmail(string email,string expectedResult)
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
    }
}
