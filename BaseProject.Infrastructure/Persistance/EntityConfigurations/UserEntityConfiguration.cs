
using BaseProject.Infrastructure.Utilities.Hashing;

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

        builder.HasKey(u => u.RefreshToken)
            .HasName("PK_users_refresh_token");

        builder.Property(u => u.RefreshTokenExpire)
            .HasColumnName("refresh_token_expire");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("password_hash");

        builder.Property(u => u.PasswordSalt)
            .IsRequired()
            .HasColumnName("password_salt");

        builder.BaseProperties(true);

        HashingHelper.CreatePasswordHash("dev123test*-", out byte[] passwordHash, out byte[] passwordSalt);

        builder.HasData(new User
        {
            UserId = BaseEntityConfiguration.SeedUserId,
            TenantId = BaseEntityConfiguration.SeedTenantId,
            FirstName = "Test",
            LastName = "Kullanıcı",
            Email = "orhanaykac@gmail.com",
            CreatedAt = BaseEntityConfiguration.SeedDateTime,
            CreatedBy = BaseEntityConfiguration.SeedTenantId,
            IsActive = true,
            IsDeleted = false,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RefreshToken = null,
            RefreshTokenExpire = null
        });

        builder.HasIndex(i => new { i.IsDeleted, i.Email });


        builder.HasOne(u => u.Tenant)
            .WithMany()
            .HasForeignKey(u => u.TenantId);
    }
}
