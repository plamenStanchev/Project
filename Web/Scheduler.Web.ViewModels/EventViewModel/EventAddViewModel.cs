namespace Scheduler.Web.ViewModels.EventViewModel
{
    using System;

    using FluentValidation;
    using FluentValidation.Validators;

    public class EventAddViewModel
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public bool AllDay { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Url { get; set; }

        public string OwnerId { get; set; }
    }

    public class EventAddValidator : AbstractValidator<EventAddViewModel>
    {
        public EventAddValidator()
        {
            this.RuleFor(e => e.Name)
                .NotEmpty()
                .NotNull()
                .Length(3, 20);
            this.RuleFor(e => e.Description)
                .NotNull()
                .NotEmpty()
                .Length(1, 500);
            this.RuleFor(e => e.StartDate)
                .NotNull();
            this.RuleFor(e => e.StartTime)
                .NotNull();
            this.RuleFor(e => e.EndDate)
                .NotNull();
            this.RuleFor(e => e.EndTime)
                .NotNull();
            this.RuleFor(e => e.Url)
                .NotNull()
                .UrlChek()
                .WithMessage("Enter Valid Url");
            this.RuleFor(e => e.OwnerId)
                .NotNull()
                .NotEmpty();
        }
    }

    public static class EventPropertyValidator
    {
        public static IRuleBuilderOptions<T, string> UrlChek<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(@"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"));
        }
    }
}
