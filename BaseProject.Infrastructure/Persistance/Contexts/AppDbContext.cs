using BaseProject.Application.Data;

namespace BaseProject.Infrastructure.Persistance.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm ilişkiler için varsayılan silme davranışını Restrict olarak ayarlıyoruz
        //Child datalar silinmeden önce parent dataların silinmesini engelliyoruz
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.AddInterceptors(new AuditLogInterceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
