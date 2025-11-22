
namespace BaseProject.Infrastructure.Persistance.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users")
            .HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id");

        builder.Property(p => p.Email)
            .IsRequired()
            .HasColumnName("email")
            .HasColumnType("citext")
            .HasMaxLength(75);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasColumnName("first_name")
            .HasColumnType("citext")
            .HasMaxLength(50);

        builder.Property(p => p.ApiSecret)
            .IsRequired()
            .HasColumnName("api_secret");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasColumnName("last_name")
            .HasColumnType("citext")
            .HasMaxLength(50);

        builder.BaseProperties();

        builder.HasIndex(i => new { i.IsDeleted, i.Email });
    }
}
