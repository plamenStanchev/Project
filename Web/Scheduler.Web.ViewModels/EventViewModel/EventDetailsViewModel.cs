namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System;
    using System.Collections.Generic;

    using Scheduler.Web.ViewModels.Comments;

    public class EventDetailsViewModel
    {
        public EventDetailsViewModel()
        {
            this.Participants = new HashSet<string>();
        }

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
