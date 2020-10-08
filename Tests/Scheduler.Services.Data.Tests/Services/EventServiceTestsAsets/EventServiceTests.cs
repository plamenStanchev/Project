namespace Scheduler.Services.Data.Tests.Services.EventServiceTestsAsets
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Xunit;

    public class EventServiceTests
    {
        public Mock<IDeletableEntityRepository<Event>> MockeEventRepository;
        public IMapper Mapper;
        public Mock<IRepository<ApplicationUserEvent>> MockApplicationUserEventRepository;
        public Mock<IUserService> MockUserService;
        public UriBuilder UriBuilder;
        public IEventService eventService;

        public EventServiceTests()
        {
            this.MockeEventRepository = new Mock<IDeletableEntityRepository<Event>>();
            this.Mapper = new Mapper();
            this.UriBuilder = new UriBuilder();
            this.MockApplicationUserEventRepository = new Mock<IRepository<ApplicationUserEvent>>();
            this.MockUserService = new Mock<IUserService>();
            this.eventService = new EventService(
                                                 this.MockeEventRepository.Object,
                                                 this.Mapper,
                                                 this.MockApplicationUserEventRepository.Object,
                                                 this.UriBuilder,
                                                 this.MockUserService.Object);
        }

        [Theory]
        [InlineData("SomeDescription", "someOwnerId", "Name", "10/10/2000", "18-18", "09/09/2100", "19-19")]
        public async Task ShudCreateEvent(
            string descriptionm,
            string ownerId,
            string name,
            string startDate,
            string startTime,
            string endDate,
            string endTime)
        {
            var model = new EventAddViewModel()
            {
                Description = descriptionm,
                StartDate = DateTime.Parse(startDate),
                StartTime = DateTime.ParseExact(startTime, "mm-ss", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse(endDate),
                EndTime = DateTime.ParseExact(endTime, "mm-ss", CultureInfo.InvariantCulture),
                Name = name,
                OwnerId = ownerId,
            };

            this.MockeEventRepository.Setup(a => a.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);
            this.MockApplicationUserEventRepository.Setup(a => a.AddAsync(It.IsAny<ApplicationUserEvent>()))
                .Returns(Task.CompletedTask);

            var result = await this.eventService.CreateEvent(model);

            Assert.True(result);
        }

        [Theory]
        [InlineData("1234", "12345")]
        [InlineData("SomeId", "SomeOther")]
        public async Task ShudAddUserToEvent(string userId, string eventId)
        {
            var userEvent = new ApplicationUserEvent()
            {
                ApplicationUserId = userId,
                EventId = eventId,
            };

            this.MockApplicationUserEventRepository.Setup(a => a.AddAsync(It.IsAny<ApplicationUserEvent>()))
                .Returns(Task.CompletedTask);

            var result = await this.eventService.AddUserToEvent(userId, eventId);
            Assert.True(result);
        }

        [Theory]
        [InlineData("1234", 1)]
        [InlineData("12345", 2)]
        [InlineData("SomeInvalidIncurentCase", 0)]
        public async Task ShudGetAllEventsForUser(string appUserId, int expectedResult)
        {
            var data = new List<ApplicationUserEvent>()
            {
                new ApplicationUserEvent()
                {
                    ApplicationUserId = "1234",
                    EventId = "1234",
                    Event = new Event()
                    {
                        Id = "1234",
                        IsDeleted = false,
                    },
                },
                new ApplicationUserEvent()
                {
                    ApplicationUserId = "12345",
                    EventId = "12345",
                    Event = new Event()
                    {
                        Id = "12345",
                        IsDeleted = false,
                    },
                },
                new ApplicationUserEvent()
                {
                    ApplicationUserId = "12345",
                    EventId = "123456",
                    Event = new Event()
                    {
                        Id = "123456",
                        IsDeleted = false,
                    },
                },
                new ApplicationUserEvent()
                {
                    ApplicationUserId = "12345",
                    EventId = "1234567",
                    Event = new Event()
                    {
                        Id = "1234567",
                        IsDeleted = true,
                    },
                },
            };

            var mockDbset = data.AsQueryable().BuildMockDbSet();
            this.MockApplicationUserEventRepository.Setup(a => a.All())
                .Returns(mockDbset.Object);

            var result = await this.eventService.GetAllEventsForUser(appUserId);

            Assert.Equal(expectedResult, result.Count());
        }

        [Theory]
        [InlineData("12345", "SomeName1")]
        [InlineData("1234", "SomeName2")]
        [InlineData("123456", null)]
        public async Task ShudGetEvent(string eventId, string expectedResult)
        {
            var data = new List<Event>()
            {
                new Event()
                {
                    Id = "12345",
                    Name = "SomeName1",
                    IsDeleted = false,
                    Owner = new ApplicationUser(),
                },
                new Event()
                {
                    Id = "1234",
                    Name = "SomeName2",
                    IsDeleted = false,
                    Owner = new ApplicationUser(),
                },
                new Event()
                {
                    Id = "123456",
                    Name = "SomeName3",
                    IsDeleted = true,
                    Owner = new ApplicationUser(),
                },
            };

            var mockDbset = data.AsQueryable().BuildMockDbSet();
            this.MockeEventRepository.Setup(e => e.All())
                .Returns(mockDbset.Object);

            Event result = await this.eventService.GetEvent(eventId);

            if (string.IsNullOrEmpty(expectedResult))
            {
                Assert.Null(result);
            }
            else
            {
                Assert.Equal(expectedResult, result.Name);
            }
        }
    }
}
