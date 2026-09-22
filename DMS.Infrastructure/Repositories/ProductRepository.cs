using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace DMS.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DmsDbContext _context;

        public ProductRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Product?> GetByProductCodeAsync(string productCode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.ProductCode == productCode);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<bool> HasOrderItemsAsync(int productId)
        {
            return await _context.OrderItems
                .AnyAsync(x => x.ProductId == productId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Product> Products, int TotalCount)> GetActiveProductsAsync(
    string? search,
    int pageNumber,
    int pageSize)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.ProductCode.Contains(search) ||
                    x.Name.Contains(search) ||
                    x.Category.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}
