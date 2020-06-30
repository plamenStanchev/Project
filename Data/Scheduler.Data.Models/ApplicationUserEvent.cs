namespace Scheduler.Data.Models
{
    public class ApplicationUserEvent
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string EventId { get; set; }

        public Event Event { get; set; }

    }
}
