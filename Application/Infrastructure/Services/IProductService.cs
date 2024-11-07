using Application.Domain.Entities;
using Application.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.Services
{
    public interface IProductService
    {
        Task<int> Create(Product product);
        Task<bool> Update(int id, Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<int> CountAll();
        Task<Product> GetById(int id);
        Task<bool> Delete(int id);
    }
}
