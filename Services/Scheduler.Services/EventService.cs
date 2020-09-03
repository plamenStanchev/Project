namespace Scheduler.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Scheduler.Data.Common.Repositories;
    using Scheduler.Data.Models;
    using Scheduler.Services.Interfaces;
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Z.EntityFramework.Plus;
    using System.Linq.Expressions;

    // to Add loger
    public class EventService : IEventService
    {
        private readonly UriBuilder uriBuilder;
        private readonly IDeletableEntityRepository<Event> efDeletableRepositiryEvent;
        private readonly IMapper mapper;
        private readonly IRepository<ApplicationUserEvent> efRepositiryEventAppUser;
        private readonly IUserService userService;

        public EventService(IDeletableEntityRepository<Event> efDeletableRepositiry, 
            IMapper mapper,
            IRepository<ApplicationUserEvent> repository,
            UriBuilder uriBuilder,
            IUserService userService)
        {
            this.efDeletableRepositiryEvent = efDeletableRepositiry;
            this.mapper = mapper;
            this.efRepositiryEventAppUser = repository;
            this.uriBuilder = uriBuilder;
            this.userService = userService;
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
                 || e.End.CompareTo(endDate) <= 0)
                 && e.IsDeleted == false)
                 .ToList();

            return events;
        }

        public async Task DeleteEvent(string eventId, string userId)
        {
            var permission = this.efDeletableRepositiryEvent.All()
                .Where(e => e.Id == eventId)
                .FirstOrDefault(e => e.OwnerId == userId && e.IsDeleted == false);

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

        public async Task UpdateParticipants(EventAddParticipantsViewModel eventParticipants)
        {
            var participantsDetails = await this.userService.GetUserIds(eventParticipants.UersEmail);
            var participantsInEvent = new List<ApplicationUserEvent>();
            foreach (var pd in participantsDetails)
            {
                participantsInEvent.Add(new ApplicationUserEvent()
                {
                    ApplicationUserId = pd.Id,
                    EventId = eventParticipants.EventId,
                });
            }

            var predicate = this.BuildPredicate(participantsInEvent);

            await this.efRepositiryEventAppUser
                .All()
                .Where(predicate)
                .DeleteAsync();

            await this.efRepositiryEventAppUser.SaveChangesAsync();

            var usersInEvent = await this.efRepositiryEventAppUser.All()
                .Where(ae => ae.EventId == eventParticipants.EventId)
                .Select(ae => ae.ApplicationUserId)
                .ToListAsync();

            foreach (var user in participantsInEvent)
            {
                if (!usersInEvent.Contains(user.ApplicationUserId))
                {
                    await this.efRepositiryEventAppUser.AddAsync(new ApplicationUserEvent()
                    {
                        ApplicationUserId = user.ApplicationUserId,
                        EventId = user.EventId,
                    });
                }
            }

            await this.efRepositiryEventAppUser.SaveChangesAsync();

        }

        private string BuildUrlForEvent(string paramId)
        {
            this.uriBuilder.Path = "Event/Details";
            this.uriBuilder.Query = $"eventId={paramId}";
            this.uriBuilder.Scheme = "https";
            this.uriBuilder.Port = 44319;
            return this.uriBuilder.ToString();
        }

        private Expression<Func<ApplicationUserEvent, bool>> BuildPredicate(IEnumerable<ApplicationUserEvent> updateCollection)
        {
            var parameter = Expression.Parameter(typeof(ApplicationUserEvent));
            var body = updateCollection
                .Select(ap => Expression.AndAlso(
                    Expression.NotEqual(Expression.Property(parameter, nameof(ap.ApplicationUserId)), Expression.Constant(ap.ApplicationUserId)),
                    Expression.Equal(Expression.Property(parameter, nameof(ap.EventId)), Expression.Constant(ap.EventId))))
                .Aggregate(Expression.And);

            var predicate = Expression.Lambda<Func<ApplicationUserEvent, bool>>(body, parameter);

            return predicate;

        }
    }
}
