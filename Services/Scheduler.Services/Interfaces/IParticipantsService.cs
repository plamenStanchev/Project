namespace Scheduler.Services.Interfaces
{

    using System.Threading.Tasks;

    using Scheduler.Web.ViewModels.EventViewModel;

    public interface IParticipantsService
    {
        public Task<GetParticipantsViewModel> GetParticipantsAsync(string eventId);

        public Task<bool> DeleteParticepantAsync(string eventId, string patientId);

        public Task UpdateParticipantsAsync(EventAddParticipantsViewModel eventParticipants);

        public Task<bool> AddUserToEventAsync(string userId, string eventId);
    }
}
