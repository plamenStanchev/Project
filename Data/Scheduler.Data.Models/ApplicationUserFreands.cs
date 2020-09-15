namespace Scheduler.Data.Models
{
    public class ApplicationUserFreands
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string FriandId { get; set; }

        public ApplicationUser Friend { get; set; }
    }
}
