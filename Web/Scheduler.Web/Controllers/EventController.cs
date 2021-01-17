namespace Scheduler.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Common;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.EventViewModel;

    // TODo  move validation to Validation Service
    public class EventController : BaseController
    {

        private readonly IEventService eventService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICommentService commentService;
        private readonly IMapper mapper;

        public EventController(
            IEventService eventService,
            UserManager<ApplicationUser> userManager,
            ICommentService commentService,
            IMapper mapper)
        {
            this.eventService = eventService;
            this.userManager = userManager;
            this.commentService = commentService;
            this.mapper = mapper;
        }

        private string userId => this.userManager.GetUserId(this.User);

        [HttpPost]
        public async Task<IActionResult> Create(EventAddViewModel eventAddViewModel)
        {
            eventAddViewModel.OwnerId = this.userManager.GetUserId(this.User);
            var result = await this.eventService.CreateEvent(eventAddViewModel);
            if (result)
            {
                return this.Redirect(GlobalConstants.HomeUrl);
            }
            else
            {
                return this.Problem();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string eventId)
        {
            var @event = await this.eventService.GetEvent(eventId);
            var eventDto = this.mapper.MapToEvetnDetailsDto(@event);

            eventDto.Participants = @event.AtendingUsers
                .Select(au => string.Concat(au.ApplicationUser.FirstName + " " + au.ApplicationUser.LastName)).ToList();
            eventDto.Participants = new string[] { "Some Name", "Some otherName", "OneMore TestName", "Last TestName" };
            eventDto.Comments = await this.commentService.GetCommentsForEvent(eventId);
            return this.View(eventDto);
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
        public async Task<IActionResult> UpdateEvent([FromBody]EventAddViewModel eventAddViewModel)
        {
           await this.eventService.UpdateEvent(eventAddViewModel);
           return this.Redirect(GlobalConstants.HomeUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string eventId, string userId)
        {
           await this.eventService.DeleteEvent(eventId, userId);
           return this.Redirect(GlobalConstants.HomeUrl);
        }
    }
}
