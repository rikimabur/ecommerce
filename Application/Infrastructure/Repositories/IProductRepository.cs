using Application.Domain.Entities;
using Application.Infrastructure.Persistence;

namespace Application.Infrastructure.Repositories
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
    }

    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
