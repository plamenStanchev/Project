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

        public Comment MapComment(InputCommentDto commentDto)
        {
            return new Comment()
            {
                Name = commentDto.Name,
                Content = commentDto.Content,
                AuthorId = commentDto.AuthorId,
                EventId = commentDto.EventId,
            };
        }

        public InputCommentDto MapCommentDto(Comment comment)
        {
            return new InputCommentDto()
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

                Start = new DateTime(
                year: eventDto.StartDate.Year,
                month: eventDto.StartDate.Month,
                day: eventDto.StartDate.Day,
                hour: eventDto.StartTime.Hour,
                minute: eventDto.StartTime.Minute,
                second: 0),

                End = new DateTime(
                year: eventDto.EndDate.Year,
                month: eventDto.EndDate.Month,
                day: eventDto.EndDate.Day,
                hour: eventDto.EndDate.Hour,
                minute: eventDto.EndDate.Minute,
                second: 0),

                AllDay = eventDto.AllDay,
                Url = eventDto.Url,
                OwnerId = eventDto.OwnerId,
            };
        }

        public EventDetailsViewModel MapToEvetnDetailsDto(Event @event)
        {
            return new EventDetailsViewModel
            {
                Id = @event.Id,
                Name = @event.Name,
                Date = @event.Date,
                Description = @event.Description,
                Start = @event.Start,
                End = @event.End,
                OwnerName = @event.Owner.FirstName + " " + @event.Owner.LastName,
            };
        }

        public OutputCommentDto MapToOutputCommentDto(Comment comment)
        {
            return new OutputCommentDto
            {
                Name = comment.Name,
                Content = comment.Content,
                EventId = comment.EventId,
                AuthorName = comment.Author.FirstName + " " + comment.Author.LastName,
                CreatedOn = comment.CreatedOn,
            };
        }
    }
}
