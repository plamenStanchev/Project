namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System.Collections.Generic;

    using Scheduler.Web.ViewModels.UserViewModel;

    public class GetParticipantsViewModel
    {
        public string EventId { get; set; }

        public string EventName { get; set; }
        public IEnumerable<UserResponeViewModel> UserResponeViewModels { get; set; }
    }
}
