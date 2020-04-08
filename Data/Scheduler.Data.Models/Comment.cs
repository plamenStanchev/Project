namespace Scheduler.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Scheduler.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public ApplicationUser Author { get; set; }

        [Required]
        public string EventId { get; set; }

        public Event Event { get; set; }
    }
}
