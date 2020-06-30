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

        public Task<IEnumerable<Event>> GetEventsFromTo(string start, string end);

        public Task<bool> CreateEvent(EventAddViewModel eventAdd);

        public Task<bool> AddUserToEvent(string userId, string eventId);
    }
}
