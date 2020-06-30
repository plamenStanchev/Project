namespace Scheduler.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.Comments;

    public class CommentService : ICommentService
    {
        private readonly IDeletableEntityRepository<Comment> efDeletableEntityRepository;
        private readonly IMapper mapper;

        public CommentService(IDeletableEntityRepository<Comment> efDeletableEntityRepository,
            IMapper mapper)
        {
            this.efDeletableEntityRepository = efDeletableEntityRepository;
            this.mapper = mapper;
        }

        public async Task<bool> AddComment(CommentDto commentDto)
        {
            var comment = this.mapper.MapComment(commentDto);
            var result = this.efDeletableEntityRepository.AddAsync(comment);
            if (result.IsCompletedSuccessfully)
            {
                await this.efDeletableEntityRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task DeleteComment(int commentId)
        {
            this.efDeletableEntityRepository.Delete(new Comment()
            { Id = commentId });
            await this.efDeletableEntityRepository.SaveChangesAsync();
        }

        public async Task EditComment(CommentDto commentDto)
        {
            var comment = this.mapper.MapComment(commentDto);
            this.efDeletableEntityRepository.Update(comment);
            await this.efDeletableEntityRepository.SaveChangesAsync();
        }

        public CommentDto GetComment(int commentId)
        {
            var comment = this.efDeletableEntityRepository.All()
                 .FirstOrDefault(c => c.Id == commentId);

            return this.mapper.MapCommentDto(comment);
        }
    }
}
