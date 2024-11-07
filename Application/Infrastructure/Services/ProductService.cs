using Application.Domain.Entities;
using Application.Infrastructure.Repositories;

namespace Application.Infrastructure.Services
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<int> Create(Product product)
        {
            int result = await _productRepository.Add(product);
            return result;
        }

        public async Task<bool> Update(int id, Product product)
        {
            var productEntity = await GetById(id);
            int productsUpdated = await _productRepository.Update(product);
            return (productsUpdated > 0);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = await _productRepository.GetAll();
            return products;
        }

        public async Task<int> CountAll()
        {
            int count = await _productRepository.CountAll();
            return count;
        }

        public async Task<Product> GetById(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
                throw new Exception("Product record does not exist.");
            return product;
        }

        public async Task<bool> Delete(int id)
        {
            var product = await GetById(id);
            int result = await _productRepository.Delete(product);
            return (result > 0);
        }
    }
}
