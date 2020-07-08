namespace Scheduler.Services.Mapping
{
    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.Comments;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Web.ViewModels.UserViewModel;

    public interface IMapper
    {
        public ApplicationUser MapAppUser(UserRegisterViewModel userRegisterViewModel);

        public Event MapEvent(EventAddViewModel eventDto);

        public Comment MapComment(CommentDto commentDto);

        public CommentDto MapCommentDto(Comment comment);
    }
}
