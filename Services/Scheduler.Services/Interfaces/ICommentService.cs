namespace Scheduler.Services.Interfaces
{
    using System.Threading.Tasks;

    using Scheduler.Web.ViewModels.Comments;

    public interface ICommentService
    {
        public Task<bool> AddComment(CommentDto comment);

        public Task DeleteComment(int commentId);

        public CommentDto GetComment(int commentId);

        public Task EditComment(CommentDto comment);
    }
}
