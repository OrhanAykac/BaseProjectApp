namespace BaseProject.Application.Features.User.Queries.GetUsers;

public class GetUsersQueryValidation : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidation()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .When(x => x.UsePaginate);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .When(x => x.UsePaginate);

        RuleFor(x => x.OnlyActive)
            .NotNull();

        RuleFor(x => x.UsePaginate)
            .NotNull();
    }
}
