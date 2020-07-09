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

        public string Id { get; set; }

        public bool AllDay { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Url]
        public string Url { get; set; }

        [Required]
        public string OwnerId { get; set; }
    }
}
