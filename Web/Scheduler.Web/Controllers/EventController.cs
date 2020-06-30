namespace Scheduler.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.EventViewModel;

    //TODo  move validation to Validation Service
    public class EventController : BaseController
    {

        private IEventService eventService;
        private UserManager<ApplicationUser> userManager;

        public EventController(IEventService eventService, UserManager<ApplicationUser> userManager)
        {
            this.eventService = eventService;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventAddViewModel eventAddViewModel )
        {
            eventAddViewModel.OwnerId = this.userManager.GetUserId(this.User);
            var result = await this.eventService.CreateEvent(eventAddViewModel);
            if (result)
            {
                return this.Ok(result);
            }
            else
            {
                return this.Problem();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(string start, string end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrWhiteSpace(start)
               || string.IsNullOrEmpty(end) || string.IsNullOrWhiteSpace(end))
            {
                return this.BadRequest();
            }

            var events = await this.eventService.GetEventsFromTo(start, end);

            return this.Ok(events);
        }


        public IActionResult Edit()
        {
            return default;
        }
    }
}