namespace Scheduler.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Services.Mapping;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    // to Add loger
    public class EventService : IEventService
    {
        private readonly IDeletableEntityRepository<Event> efDeletableRepositiryEvent;
        private readonly IMapper mapper;
        private readonly IRepository<ApplicationUserEvent> efRepositiryEventAppUser;

        public EventService(IDeletableEntityRepository<Event> efDeletableRepositiry, 
            IMapper mapper,
            IRepository<ApplicationUserEvent> repository
            )
        {
            this.efDeletableRepositiryEvent = efDeletableRepositiry;
            this.mapper = mapper;
            this.efRepositiryEventAppUser = repository;
        }

        // todo RefactorNames
        public async Task<bool> AddUserToEvent(string userId, string eventId)
        {
            var eventAppUser = new ApplicationUserEvent()
            {
                ApplicationUserId = userId,
                EventId = eventId,
            };

            var result = this.efRepositiryEventAppUser
                .AddAsync(eventAppUser);

            if (result.IsCompletedSuccessfully)
            {
                await this.efRepositiryEventAppUser.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CreateEvent(EventAddViewModel eventAdd)
        {
            Event @event = this.mapper.MapEvent(eventAdd);
            var result = this.efDeletableRepositiryEvent
                .AddAsync(@event);
            if (result.IsCompletedSuccessfully)
            {
                await this.efDeletableRepositiryEvent.SaveChangesAsync();

                await this.AddUserToEvent(@event.OwnerId, @event.Id);

                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Event>> GetAllEventsForUser(string userId)
        {
            var events = await this.efRepositiryEventAppUser.All()
                .Where(ae => ae.ApplicationUserId == userId)
                .Select(ae => ae.Event)
                .ToListAsync();

            return events;
        }

        public async Task<Event> GetEvent(string eventId)
        {
            var @event = await this.efDeletableRepositiryEvent.All()
                .Where(e => e.Id == eventId)
                 .FirstOrDefaultAsync();

            return @event;
        }

        public async Task<IEnumerable<Event>> GetEventsFromTo(string start, string end)
        {
            var startDate = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var endDate = DateTime.Parse(end, null, System.Globalization.DateTimeStyles.RoundtripKind);

            var events = await this.efDeletableRepositiryEvent.All()
                .Where(e => e.Start.CompareTo(startDate) >= 0
                 || e.End.CompareTo(endDate) <= 0)
                .ToListAsync();

            return events;
        }
    }
}
