namespace Scheduler.Services.Data.Tests.Services.CommentServiceTestAsets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.Comments;
    using Xunit;

    public class CommentServiceTests
    {
        public Mock<IDeletableEntityRepository<Comment>> MockCommentRepository
            = new Mock<IDeletableEntityRepository<Comment>>();

        public IMapper mapper;
        public ICommentService CommentService;

        public CommentServiceTests()
        {
            this.mapper = new Mapper();
            this.CommentService = new CommentService(this.MockCommentRepository.Object, this.mapper);
        }

        [Theory]
        [InlineData("someTest", "AndAnother", "SomethingElse", "Contennt")]
        [InlineData("SomeTest2", "SomeAouthr", "Event", "content")]
        public async Task ShudAddComment(string name, string authorId, string eventId, string content)
        {
            this.MockCommentRepository.Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);
            var coment = new InputCommentDto()
            {
                AuthorId = authorId,
                Content = content,
                EventId = eventId,
                Name = name,
            };
            var result = await this.CommentService.AddComment(coment);

            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task ShudDeleteComment(int id)
        {
            var data = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    IsDeleted = false,
                },
                new Comment()
                {
                    Id = 2,
                    IsDeleted = false,
                },
                new Comment()
                {
                    Id = 3,
                    IsDeleted = false,
                },
            };
            var mockSet = data.AsQueryable().BuildMockDbSet();
            this.MockCommentRepository.Setup(r => r.All())
                .Returns(mockSet.Object);
            this.MockCommentRepository.Setup(s => s.Delete(It.IsAny<Comment>()))
                .Verifiable();
            await this.CommentService.DeleteComment(id);
        }

        [Theory]
        [InlineData(1, "SomeName1")]
        [InlineData(2, "SomeName2")]
        [InlineData(5, null)]
        public async Task ShudGetComment(int id, string expectedResult)
        {
            var data = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    IsDeleted = false,
                    Name = "SomeName1",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                },
                new Comment()
                {
                    Id = 2,
                    IsDeleted = false,
                    Name = "SomeName2",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                },
                new Comment()
                {
                    Id = 3,
                    IsDeleted = false,
                    Name = "SomeName3",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                },
            };
            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            this.MockCommentRepository.Setup(r => r.All())
                .Returns(mockDbSet.Object);
            var result = await this.CommentService.GetComment(id);

            if (id == 5)
            {
                Assert.Null(result);
            }
            else
            {
                Assert.Equal(expectedResult, result.Name);
            }

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ShudEitComment(int id)
        {
            var comment = new InputCommentDto()
            {
                AuthorId = "Some",
                Content = "dadasdsaddaas",
                EventId = "dadsadad",
                Name = "dadea",
            };
            this.MockCommentRepository.Setup(r => r.Update(It.IsAny<Comment>()))
                .Verifiable();
            await this.CommentService.EditComment(comment, id);
        }
        [Theory]
        [InlineData("some", 2)]
        [InlineData("some1", 1)]
        [InlineData("SomeEroorID", 0)]
        public async Task GetCommentsForEvent(string eventId, int expectedResult)
        {
            var data = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    IsDeleted = false,
                    Name = "SomeName1",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                    EventId = "some",
                },
                new Comment()
                {
                    Id = 2,
                    IsDeleted = false,
                    Name = "SomeName2",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                    EventId = "some",
                },
                new Comment()
                {
                    Id = 3,
                    IsDeleted = false,
                    Name = "SomeName3",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                    EventId = "some1",
                },
                new Comment()
                {
                    Id = 4,
                    IsDeleted = true,
                    Name = "SomeName4",
                    Author = new ApplicationUser()
                    {
                        FirstName = "some",
                        LastName = "Alsosome",
                    },
                    EventId = "Some",
                },
            };
            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            this.MockCommentRepository.Setup(r => r.All())
                .Returns(mockDbSet.Object);
            var result = await this.CommentService.GetCommentsForEvent(eventId);
            Assert.Equal(expectedResult, result.Count());
        }
    }
}
