using BaseProject.Domain.Abstract;

namespace BaseProject.Infrastructure.Persistance.EntityConfigurations;

public static class BaseEntityConfiguration
{
    public static DateTime SeedDateTime => new(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc);
    public static Guid SeedUserId => Guid.Parse("ACA69BFD-3201-4D8C-BF98-05935B962571");
    public static Guid SeedTenantId => Guid.Parse("FF4EC41C-6D23-4A27-A96C-B5A23FBE85B4");

    public static void BaseProperties<TEntity>(this EntityTypeBuilder<TEntity> builder, bool hasTenantId)
    where TEntity : BaseEntity, IEntity
    {

        builder
            .Property<DateTime>("CreatedAt")
            .IsRequired();

        builder
            .Property<DateTime?>("UpdatedAt");

        builder
            .Property<Guid>("CreatedBy")
            .IsRequired();

        builder
            .Property<Guid?>("UpdatedBy");


        builder
            .Property<bool>("IsDeleted")
            .IsRequired();

        builder
            .Property<bool>("IsActive")
            .IsRequired();


        if (hasTenantId)
        {
            builder.Property(p => p.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.HasOne("Tenant")
                .WithMany()
                .HasForeignKey("TenantId");

            builder.HasIndex(p => new { p.IsDeleted, p.TenantId, p.IsActive });
        }
        else
        {
            //TenantId kullanmadığımız tablolarda ignore ediyoruz ama Tenant tablosu hariç
            if (typeof(TEntity) != typeof(Tenant))
                builder.Ignore(p => p.TenantId);

            builder.HasIndex(p => new { p.IsDeleted, p.IsActive });
        }



        builder.HasQueryFilter(x => EF.Property<bool>(x, "IsDeleted") == false);
    }
}
