namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Scheduler.Data.Models;

    public class EventAddViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        public bool AllDay { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Url]
        public string Url { get; set; }

        [Required]
        public string OwnerId { get; set; }
    }
}
