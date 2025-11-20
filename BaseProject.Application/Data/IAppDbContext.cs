using BaseProject.Domain.Concrete;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Application.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken c = default);
}
