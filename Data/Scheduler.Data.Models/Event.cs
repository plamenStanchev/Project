namespace Scheduler.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Scheduler.Data.Common.Models;

    public class Event : BaseDeletableModel<string>
    {
        public Event()
        {
            this.AtendingUsers = new HashSet<ApplicationUserEvent>();
            this.Id = Guid.NewGuid().ToString();
            this.Comments = new HashSet<Comment>();
        }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public ApplicationUser Owner { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<ApplicationUserEvent> AtendingUsers { get; set; }
    }
}
