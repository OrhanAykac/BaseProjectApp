namespace BaseProject.Infrastructure.Persistance.EntityConfigurations;

public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants")
            .HasKey(t => t.TenantId);

        builder.Property(t => t.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasColumnType("citext")
            .HasMaxLength(100);

        builder.BaseProperties(false);

        builder.HasData(new Tenant
        {
            TenantId = BaseEntityConfiguration.SeedTenantId,
            Name = "Default Tenant",
            CreatedAt = BaseEntityConfiguration.SeedDateTime,
            CreatedBy = BaseEntityConfiguration.SeedUserId,
            IsActive = true,
            IsDeleted = false
        });
    }
}
