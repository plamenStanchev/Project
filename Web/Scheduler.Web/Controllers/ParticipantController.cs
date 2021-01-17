namespace Scheduler.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.EventViewModel;

    public class ParticipantController : BaseController
    {
        private readonly IParticipantsService participantsService;
        private readonly IUserService userService;

        public ParticipantController(
            IParticipantsService participantsService)
        {
            this.participantsService = participantsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string eventId)
        {
            var participants = await this.participantsService.GetParticipantsAsync(eventId);
            return this.View(participants);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EventAddParticipantsViewModel eventParticipants)
        {
            await this.participantsService.UpdateParticipantsAsync(eventParticipants);
            return this.CreateRedirecToGet( new { eventId = eventParticipants.EventId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string eventId, string participantId)
        {
           await this.participantsService.DeleteParticepantAsync(eventId, participantId);
           return this.CreateRedirecToGet(new { eventId });
        }

        [HttpGet]
        public async Task<IActionResult> AddParticipant(string eventId, string participantId)
        {
            await this.participantsService.AddUserToEventAsync(participantId, eventId);
            return this.CreateRedirecToGet(eventId);
        }

        private IActionResult CreateRedirecToGet(object routeValues)
        {
            return this.RedirectToAction(
                actionName: nameof(this.Get),
                routeValues: routeValues);
        }
    }
}
