namespace Scheduler.Services.Mapping
{
    using System;

    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.Comments;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Web.ViewModels.UserViewModel;

    public class Mapper : IMapper
    {
        public ApplicationUser MapAppUser(UserRegisterViewModel userRegisterViewModel)
        {
            return new ApplicationUser()
            {
                UserName = $"{userRegisterViewModel.FirstName} {userRegisterViewModel.LastName}",
                FirstName = userRegisterViewModel.FirstName,
                LastName = userRegisterViewModel.LastName,
                Email = userRegisterViewModel.Email,
                CreatedOn = DateTime.UtcNow,
            };
        }

        public Comment MapComment(CommentDto commentDto)
        {
            return new Comment()
            {
                Name = commentDto.Name,
                Content = commentDto.Content,
                AuthorId = commentDto.AuthorId,
                EventId = commentDto.EventId,
            };
        }

        public CommentDto MapCommentDto(Comment comment)
        {
            return new CommentDto()
            {
                Name = comment.Name,
                Content = comment.Content,
                AuthorId = comment.AuthorId,
                EventId = comment.EventId,
            };
        }

        public Event MapEvent(EventAddViewModel eventDto)
         {
            return new Event()
            {
                Name = eventDto.Name,
                Description = eventDto.Description,
                Start = eventDto.Start,
                End = eventDto.End,
                Date = eventDto.Date,
                AllDay = eventDto.AllDay,
                Url = eventDto.Url,
                OwnerId = eventDto.OwnerId,
            };
         }

        public EventAddViewModel MapEventDto(Event @event)
         {
            return new EventAddViewModel()
            {
                Name = @event.Name,
                AllDay = @event.AllDay,
                Description = @event.Description,
                Start = @event.Start,
                End = @event.End,
                Date = @event.Date,
                Url = @event.Url,
                OwnerId = @event.OwnerId,
            };
         }
    }
}
