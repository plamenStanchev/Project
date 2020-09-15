namespace Scheduler.Web.ViewModels.EventViewModel
{
    using Scheduler.Web.ViewModels.Comments;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EventDetailsViewModel
    {
        public EventDetailsViewModel()
        {
            this.Participants = new HashSet<string>();
        }

        [Required]
        public string Id { get; set; }

        public string OwnerName { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public IEnumerable<string> Participants { get; set; }

        public IEnumerable<OutputCommentDto> Comments { get; set; }
    }
}
