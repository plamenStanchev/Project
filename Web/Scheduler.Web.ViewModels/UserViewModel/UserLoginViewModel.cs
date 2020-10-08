namespace Scheduler.Web.ViewModels.UserViewModel
{
    using FluentValidation;

    public class UserLoginViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UserLoginValidator : AbstractValidator<UserLoginViewModel>
    {
        public UserLoginValidator()
        {
            this.RuleFor(u => u.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
            this.RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty();
        }
    }
}
