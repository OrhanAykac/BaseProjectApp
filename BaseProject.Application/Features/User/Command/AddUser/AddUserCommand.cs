using BaseProject.Application.Common.Hashing;

namespace BaseProject.Application.Features.User.Command.AddUser;

public record AddUserCommand(string Email, string Password, string FirstName, string LastName, bool IsActive, Guid ProcessedBy) : IRequest<Result>;

internal sealed class AddUserCommandHandler(IAppDbContext appDbContext, ITokenService tokenService) : IRequestHandler<AddUserCommand, Result>
{
    public async ValueTask<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = request.Adapt<Domain.Concrete.User>();

        userEntity.ApiSecret = tokenService.GenerateSecretKey();

        HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        userEntity.PasswordHash = passwordHash;
        userEntity.PasswordSalt = passwordSalt;
        userEntity.IsActive = request.IsActive;
        userEntity.IsDeleted = false;

        appDbContext.Users.Add(userEntity);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
