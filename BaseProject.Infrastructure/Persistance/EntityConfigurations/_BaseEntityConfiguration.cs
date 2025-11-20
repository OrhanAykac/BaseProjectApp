using BaseProject.Domain.Abstract;

namespace BaseProject.Infrastructure.Persistance.EntityConfigurations;

public static class BaseEntityConfiguration
{
    public static void BaseProperties<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
    where TEntity : class, IEntity, new()
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

        builder.HasQueryFilter(x => EF.Property<bool>(x, "IsDeleted") == false);
    }
}
