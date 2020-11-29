namespace Scheduler.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.Comments;

    public class CommentService : ICommentService
    {
        private readonly IDeletableEntityRepository<Comment> efDeletableEntityRepository;
        private readonly IMapper mapper;

        public CommentService(
            IDeletableEntityRepository<Comment> efDeletableEntityRepository,
            IMapper mapper)
        {
            this.efDeletableEntityRepository = efDeletableEntityRepository;
            this.mapper = mapper;
        }

        public async Task<bool> AddComment(InputCommentDto commentDto)
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

        public async Task EditComment(InputCommentDto commentDto, int comentId)
        {
            var comment = this.mapper.MapComment(commentDto);
            comment.Id = comentId;

            this.efDeletableEntityRepository.Update(comment);
            await this.efDeletableEntityRepository.SaveChangesAsync();
        }

        public async Task<OutputCommentDto> GetComment(int commentId)
        {
            var comment = await this.efDeletableEntityRepository.All()
                .Where(c => c.Id == commentId
                 && c.IsDeleted == false)
                .Select(c => new { commnet = c, authro = c.Author })
                .FirstOrDefaultAsync();
            if (comment == null)
            {
                return null;
            }

            return this.mapper.MapToOutputCommentDto(comment.commnet);
        }

        public async Task<IEnumerable<OutputCommentDto>> GetCommentsForEvent(string eventId)
        {
           var comments = await this.efDeletableEntityRepository.All()
                .Where(c => c.EventId == eventId
                && c.IsDeleted == false)
                .ToListAsync();

           return comments.Select(c => this.mapper.MapToOutputCommentDto(c)).ToList();
        }
    }
}
