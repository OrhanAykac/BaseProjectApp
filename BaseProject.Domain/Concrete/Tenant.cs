using BaseProject.Domain.Abstract;

namespace BaseProject.Domain.Concrete;

public class Tenant : BaseEntity, IEntity
{
    public string Name { get; set; } = null!;
}
