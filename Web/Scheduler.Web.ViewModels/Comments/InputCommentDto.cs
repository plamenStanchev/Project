namespace Scheduler.Web.ViewModels.Comments
{
    using FluentValidation;

    public class InputCommentDto
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }

        public string EventId { get; set; }
    }

    public class ImputComentValidator : AbstractValidator<InputCommentDto>
    {
        public ImputComentValidator()
        {
            this.RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull()
                .Length(3, 90);
            this.RuleFor(c => c.Content)
                .NotEmpty()
                .NotNull()
                .Length(3, 9);
            this.RuleFor(c => c.AuthorId)
                .NotNull()
                .NotEmpty();
            this.RuleFor(c => c.EventId)
                .NotNull()
                .NotEmpty();
        }
    }
}
