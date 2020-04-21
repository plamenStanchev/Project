namespace Scheduler.Web.ViewModels.UserViewModel
{
    using System.ComponentModel.DataAnnotations;

    using Scheduler.Data.Models;
    using Scheduler.Services.Mapping;
 

    public class UserRegisterViewModel : IMapTo<ApplicationUser>
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
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
