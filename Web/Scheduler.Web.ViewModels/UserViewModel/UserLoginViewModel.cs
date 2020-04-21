namespace Scheduler.Web.ViewModels.UserViewModel
{
    using System.ComponentModel.DataAnnotations;

    public class UserLoginViewModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
