namespace Scheduler.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Scheduler.Web.ViewModels.Comments;

    public interface ICommentService
    {
        public Task<bool> AddComment(InputCommentDto comment);

        public Task DeleteComment(int commentId);

        public OutputCommentDto GetComment(int commentId);

        public Task EditComment(InputCommentDto comment, int commentId);

        public Task<IEnumerable<OutputCommentDto>> GetCommentsForEvent(string eventId);
    }
}
