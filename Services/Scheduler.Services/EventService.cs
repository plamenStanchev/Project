namespace Scheduler.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Services.Mapping;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Web.ViewModels.UserViewModel;
    using Z.EntityFramework.Plus;

    // to Add loger
    public class EventService : IEventService
    {
        private readonly UriBuilder uriBuilder;
        private readonly IDeletableEntityRepository<Event> efDeletableRepositiryEvent;
        private readonly IMapper mapper;
        private readonly IParticipantsService participantsService;

        public EventService(
            IDeletableEntityRepository<Event> efDeletableRepositiry,
            IMapper mapper,
            UriBuilder uriBuilder,
            IParticipantsService participantsService)
        {
            this.efDeletableRepositiryEvent = efDeletableRepositiry;
            this.mapper = mapper;
            this.uriBuilder = uriBuilder;
            this.participantsService = participantsService;
        }

        // todo RefactorNames

        public async Task<bool> CreateEvent(EventAddViewModel eventAdd)
        {
            Event @event = this.mapper.MapEvent(eventAdd);
            @event.Url = this.BuildUrlForEvent(@event.Id);
            var result = this.efDeletableRepositiryEvent
                .AddAsync(@event);
            if (result.IsCompletedSuccessfully)
            {
                await this.efDeletableRepositiryEvent.SaveChangesAsync();

                await this.participantsService.AddUserToEventAsync(@event.OwnerId, @event.Id);

                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Event>> GetAllEventsForUser(string userId)
        {
            var events = await this.efDeletableRepositiryEvent.All()
                .Where(e => e.AtendingUsers.Any(u => u.ApplicationUser.Id == userId))
                .ToListAsync();

            return events;
        }

        public async Task<Event> GetEvent(string eventId)
        {
            var @event = await this.efDeletableRepositiryEvent.All()
                .Where(e => e.Id == eventId
                && e.IsDeleted == false)
                .Select(e => new { @eventInner = e, owner = e.Owner })
                .FirstOrDefaultAsync();

            if (@event is null)
            {
                return await Task.FromResult<Event>(null);
            }

            return @event.eventInner;
        }

        public async Task<IEnumerable<Event>> GetEventsFromTo(string start, string end, string userId)
        {
            var startDate = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var endDate = DateTime.Parse(end, null, System.Globalization.DateTimeStyles.RoundtripKind);

            var eventsUserPartisipitsIn = await this.GetAllEventsForUser(userId);

            var events = eventsUserPartisipitsIn
                .Where(e => (e.Start.CompareTo(startDate) >= 0
                 || e.End.CompareTo(endDate) <= 0)
                 && e.IsDeleted == false)
                 .ToList();

            return events;
        }

        public async Task DeleteEvent(string eventId, string userId)
        {
            var @event = this.efDeletableRepositiryEvent.AllAsNoTracking()
                .Where(e => e.Id == eventId)
                .FirstOrDefault(e => e.OwnerId == userId && e.IsDeleted == false);

            if (@event != null)
            {
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
