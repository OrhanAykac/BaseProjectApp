
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
            .HasColumnType("citext");

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasColumnType("citext")
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasColumnType("citext")
            .HasMaxLength(50);

        builder.BaseProperties();

        builder.HasIndex(i => new { i.IsDeleted, i.Email });
    }
}
