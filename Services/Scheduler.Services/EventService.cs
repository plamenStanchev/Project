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
        private readonly UriBuilder uriBuilder;
        private readonly IDeletableEntityRepository<Event> efDeletableRepositiryEvent;
        private readonly IMapper mapper;
        private readonly IRepository<ApplicationUserEvent> efRepositiryEventAppUser;

        public EventService(IDeletableEntityRepository<Event> efDeletableRepositiry, 
            IMapper mapper,
            IRepository<ApplicationUserEvent> repository,
            UriBuilder uriBuilder)
        {
            this.efDeletableRepositiryEvent = efDeletableRepositiry;
            this.mapper = mapper;
            this.efRepositiryEventAppUser = repository;
            this.uriBuilder = uriBuilder;
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
            @event.Url = this.BuildUrlForEvent(@event.Id);
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
                .Where(ae => ae.ApplicationUserId == userId
                && ae.Event.IsDeleted == false)
                .Select(ae => ae.Event)
                .ToListAsync();

            return events;
        }

        public async Task<Event> GetEvent(string eventId)
        {
            var @event = await this.efDeletableRepositiryEvent.All()
                .Where(e => e.Id == eventId
                && e.IsDeleted == false)
                 .FirstOrDefaultAsync();

            return @event;
        }

        public async Task<IEnumerable<Event>> GetEventsFromTo(string start, string end, string userId)
        {
            var startDate = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var endDate = DateTime.Parse(end, null, System.Globalization.DateTimeStyles.RoundtripKind);

            var eventsUserPartisipitsIn = await this.GetAllEventsForUser(userId);


            var events = eventsUserPartisipitsIn
                .Where(e => (e.Start.CompareTo(startDate) >= 0
                 || e.End.CompareTo(endDate) <= 0))
                 .ToList();

            return events;
        }

        public async Task DeleteEvent(string eventId, string userId)
        {
            var permission = this.efDeletableRepositiryEvent.All()
                .Where(e => e.Id == eventId)
                .FirstOrDefault(e => e.OwnerId == userId);

            if (permission != null)
            {
                var @event = new Event()
                {
                    Id = eventId,
                };
                this.efDeletableRepositiryEvent.Delete(@event);
                await this.efDeletableRepositiryEvent.SaveChangesAsync();
            }
        }

        public async Task UpdateEvent(EventAddViewModel eventAddViewModel)
        {
            var @event = this.mapper.MapEvent(eventAddViewModel);
            @event.Id = eventAddViewModel.Id;

            this.efDeletableRepositiryEvent.Update(@event);

            await this.efDeletableRepositiryEvent.SaveChangesAsync();
        }

        public Task UpdateParticipants(EventAddParticipantsViewModel eventParticipants)
        {
            throw new NotImplementedException();
        }

        private string BuildUrlForEvent(string paramId)
        {
            this.uriBuilder.Path = "Event/Details";
            this.uriBuilder.Query = $"eventId={paramId}";
            this.uriBuilder.Scheme = "https";
            this.uriBuilder.Port = 44319;
            return this.uriBuilder.ToString();
        }
    }
}
