namespace Scheduler.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Scheduler.Data.Models;
    using Scheduler.Web.ViewModels.EventViewModel;

    public interface IEventService
    {
        public Task<Event> GetEvent(string eventId);

        public Task<IEnumerable<Event>> GetAllEventsForUser(string userId);

        public Task<IEnumerable<Event>> GetEventsFromTo(string start, string end, string userId);

        public Task<bool> CreateEvent(EventAddViewModel eventAdd);

        public Task<bool> AddUserToEvent(string userId, string eventId);

        public Task DeleteEvent(string eventId, string userId);

        public Task UpdateEvent(EventAddViewModel eventAddViewModel);

        public Task UpdateParticipants(EventAddParticipantsViewModel eventParticipants);
    }
}
