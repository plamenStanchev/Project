namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EventUpdateViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
