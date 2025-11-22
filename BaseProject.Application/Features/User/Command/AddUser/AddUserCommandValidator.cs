using FluentValidation;

namespace BaseProject.Application.Features.User.Command.AddUser;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(75);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 32);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 32);

        RuleFor(r => r.IsActive)
            .NotNull();
    }
}
