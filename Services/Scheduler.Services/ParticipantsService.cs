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
    using Scheduler.Web.ViewModels.EventViewModel;
    using Scheduler.Web.ViewModels.UserViewModel;

    public class ParticipantsService : IParticipantsService
    {
        private readonly IRepository<ApplicationUserEvent> efRepositiryApplicationUserEvent;
        private readonly IUserService userService;

        public ParticipantsService(
            IRepository<ApplicationUserEvent> efRepositiryApplicationUserEvent,
            IUserService userService)
        {
            this.efRepositiryApplicationUserEvent = efRepositiryApplicationUserEvent;
            this.userService = userService;
        }

        public async Task<bool> AddUserToEventAsync(string userId, string eventId)
        {
            var eventAppUser = new ApplicationUserEvent()
            {
                ApplicationUserId = userId,
                EventId = eventId,
            };

            var result = this.efRepositiryApplicationUserEvent
                .AddAsync(eventAppUser);

            if (result.IsCompletedSuccessfully)
            {
                await this.efRepositiryApplicationUserEvent.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteParticepantAsync(string eventId, string patientId)
        {
           this.efRepositiryApplicationUserEvent.Delete(new ApplicationUserEvent() { ApplicationUserId = patientId, EventId = eventId });
           var result = await this.efRepositiryApplicationUserEvent.SaveChangesAsync();
           if (result == 0)
           {
                return false;
           }

           return true;
        }

        public async Task<GetParticipantsViewModel> GetParticipantsAsync(string eventId)
        {
            var participantResult = await this.efRepositiryApplicationUserEvent
                .All().Where(ue => ue.EventId == eventId)
                .Select(au => new GetParticipantsViewModel()
                {
                    EventId = au.EventId,
                    EventName = au.Event.Name,
                    UserResponeViewModels = new List<UserResponeViewModel>(),
                }).FirstOrDefaultAsync();

            participantResult.UserResponeViewModels = await this.efRepositiryApplicationUserEvent
                .All().Where(ue => ue.EventId == eventId).Select(e => new UserResponeViewModel()
                {
                    FirstName = e.ApplicationUser.FirstName,
                    LastName = e.ApplicationUser.LastName,
                    UserId = e.ApplicationUser.Id,
                    Email = e.ApplicationUser.Email,
                }).ToListAsync();
            return participantResult;
        }

        public async Task UpdateParticipantsAsync(EventAddParticipantsViewModel eventParticipants)
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

            await this.efRepositiryApplicationUserEvent
                .All()
                .Where(predicate)
                .DeleteFromQueryAsync();

            await this.efRepositiryApplicationUserEvent.SaveChangesAsync();

            var usersInEvent = await this.efRepositiryApplicationUserEvent.All()
                .Where(ae => ae.EventId == eventParticipants.EventId)
                .Select(ae => ae.ApplicationUserId)
                .ToListAsync();

            foreach (var user in participantsInEvent)
            {
                if (!usersInEvent.Contains(user.ApplicationUserId))
                {
                    await this.efRepositiryApplicationUserEvent.AddAsync(new ApplicationUserEvent()
                    {
                        ApplicationUserId = user.ApplicationUserId,
                        EventId = user.EventId,
                    });
                }
            }

            await this.efRepositiryApplicationUserEvent.SaveChangesAsync();
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
