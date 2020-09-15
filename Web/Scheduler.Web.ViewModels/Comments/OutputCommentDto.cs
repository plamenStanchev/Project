namespace Scheduler.Web.ViewModels.Comments
{
    using System;

    public class OutputCommentDto
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string EventId { get; set; }

        public DateTime CreatedOn{ get; set; }
    }
}
