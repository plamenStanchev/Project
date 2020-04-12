namespace Scheduler.Web.ViewModels.UserViewModel
{
    using Scheduler.Data.Models;
    using Scheduler.Services.Mapping;
    using System.ComponentModel.DataAnnotations;

    public class UserViewModel : IMapTo<ApplicationUser>
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
