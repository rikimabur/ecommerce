using Application.Common;

namespace Application.Domain.Entities
{
    public class Product(int productId, string name, string description, double price) : AuditableEntity, IEntity<int>
    {
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public double Price { get; private set; } = price;
        public int Id { get; set; } = productId;
    }
}
