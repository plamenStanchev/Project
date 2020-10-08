namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System;

    public class EventUpdateViewModel
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
