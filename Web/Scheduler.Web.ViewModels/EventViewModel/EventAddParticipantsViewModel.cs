namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EventAddParticipantsViewModel
    {
        public EventAddParticipantsViewModel()
        {
            this.UersEmail = new HashSet<string>();
        }

        public ICollection<string> UersEmail { get; set; }

        [Required]
        public string EventId { get; set; }
    }
}
