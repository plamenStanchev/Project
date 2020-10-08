namespace Scheduler.Web.ViewModels.UserViewModel
{
    using FluentValidation;

    public class UserRegisterViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class UserRegisterValidator : AbstractValidator<UserRegisterViewModel>
    {
        public UserRegisterValidator()
        {
            this.RuleFor(u => u.FirstName)
                .NotEmpty()
                .NotNull()
                .Length(3, 50);
            this.RuleFor(u => u.LastName)
                .NotEmpty()
                .NotNull()
                .Length(3, 50);
            this.RuleFor(u => u.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
            this.RuleFor(u => u.Password)
                .NotEmpty()
                .NotNull()
                .Length(6, 100);
            this.RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .NotNull();
        }
    }
}
