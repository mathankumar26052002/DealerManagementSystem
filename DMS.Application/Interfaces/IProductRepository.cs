using DMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByProductCodeAsync(string productCode);

        Task AddAsync(Product product);

        void Update(Product product);

        void Delete(Product product);

        Task<bool> HasOrderItemsAsync(int productId);

        Task SaveChangesAsync();

        Task<(List<Product> Products, int TotalCount)> GetActiveProductsAsync(
    string? search,
    int pageNumber,
    int pageSize);
    }
}
