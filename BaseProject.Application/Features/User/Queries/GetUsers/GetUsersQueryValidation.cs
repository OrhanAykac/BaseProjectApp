using FluentValidation;

namespace BaseProject.Application.Features.User.Queries.GetUsers;

public class GetUsersQueryValidation : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidation()
    {
        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageIndex)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Filter.UsePaginate);

        RuleFor(x => x.Filter.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .When(x => x.Filter.UsePaginate);

        RuleFor(x => x.Filter.OnlyActive)
            .NotNull();

        RuleFor(x => x.Filter.UsePaginate)
            .NotNull();
    }
}
