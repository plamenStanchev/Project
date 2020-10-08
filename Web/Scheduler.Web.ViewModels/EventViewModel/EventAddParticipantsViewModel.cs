namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System.Collections.Generic;

    public class EventAddParticipantsViewModel
    {
        public EventAddParticipantsViewModel()
        {
            this.UersEmail = new HashSet<string>();
        }

        public ICollection<string> UersEmail { get; set; }

        public string EventId { get; set; }
    }
}
