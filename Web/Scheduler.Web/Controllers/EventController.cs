namespace Scheduler.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.EventViewModel;

    //TODo  move validation to Validation Service 
    public class EventController : Controller
    {
        private const string homeUrl = "/";
        private string userId => this.userManager.GetUserId(this.User);

        private IEventService eventService;
        private UserManager<ApplicationUser> userManager;

        public EventController(IEventService eventService, UserManager<ApplicationUser> userManager)
        {
            this.eventService = eventService;
            this.userManager = userManager;
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
        public IActionResult UpdateEvent(EventAddViewModel eventAddViewModel)
        {
            return default;
        }

        [HttpPost]
        public IActionResult UpdateParticipants(EventAddParticipantsViewModel eventParticipants)
        {
            return default;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string eventId)
        {
            await this.eventService.DeleteEvent(eventId, this.userId);

            return this.Redirect(homeUrl);
        }
    }
}
