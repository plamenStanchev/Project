namespace Scheduler.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.Comments;
    using Scheduler.Web.ViewModels.EventViewModel;

    //TODo  move validation to Validation Service 
    public class EventController : BaseController
    {
        private const string homeUrl = "/";

        private string userId => this.userManager.GetUserId(this.User);

        private readonly IEventService eventService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICommentService commentService;

        public EventController(
            IEventService eventService,
            UserManager<ApplicationUser> userManager,
            ICommentService commentService)
        {
            this.eventService = eventService;
            this.userManager = userManager;
            this.commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventAddViewModel eventAddViewModel)
        {
            eventAddViewModel.OwnerId = this.userManager.GetUserId(this.User);
            var result = await this.eventService.CreateEvent(eventAddViewModel);
            if (result)
            {
                return this.Redirect(homeUrl);
            }
            else
            {
                return this.Problem();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string evnetId)
        {
            var @event = await this.eventService.GetEvent(evnetId);

            return this.Redirect(homeUrl);
            //return this.View(@event);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(string start, string end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrWhiteSpace(start)
               || string.IsNullOrEmpty(end) || string.IsNullOrWhiteSpace(end))
            {
                return this.BadRequest();
            }

            var events = await this.eventService.GetEventsFromTo(start, end, this.userId);

            return this.Ok(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsForUser()
        {
            var evetns = await this.eventService.GetAllEventsForUser(this.userId);
            return this.Ok(evetns);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEvent(EventAddViewModel eventAddViewModel)
        {
           await this.eventService.UpdateEvent(eventAddViewModel);
           return this.Redirect(homeUrl);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateParticipants(EventAddParticipantsViewModel eventParticipants)
        {
           await this.eventService.UpdateParticipants(eventParticipants);
           return this.Redirect(homeUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string eventId)
        {
            await this.eventService.DeleteEvent(eventId, this.userId);

            return this.Redirect(homeUrl);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentDto commentViewModel)
        {
            var modelState = this.TryValidateModel(commentViewModel);
            if (modelState == false)
            {
                return this.Redirect(homeUrl);
            }

            var result = await this.commentService.AddComment(commentViewModel);

            if (result == false)
            {
                return this.Redirect(homeUrl);
            }

            return await this.Details(commentViewModel.EventId);
        }

        public async Task<IActionResult> DeleteComment(int comentId,string eventId)
        {
           await this.commentService.DeleteComment(comentId);

           return await this.Details(eventId);
        }

        public async Task<IActionResult> EditComent(CommentDto commentViewModel,int commentId,string eventId)
        {
            var modelState = this.TryValidateModel(commentViewModel);
            if (modelState == false)
            {
                return this.Redirect(homeUrl);
            }

            await this.commentService.EditComment(commentViewModel, commentId);

            return await this.Details(eventId);
        }
    }
}
