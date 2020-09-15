namespace Scheduler.Web.ViewModels.Comments
{
    using System.ComponentModel.DataAnnotations;

    public class InputCommentDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(90)]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string EventId { get; set; }

    }
}
