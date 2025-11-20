using BaseProject.Domain.Concrete;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Application.Data;

public interface IAppDbContextRO
{
    DbSet<User> Users { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken c = default);
}
