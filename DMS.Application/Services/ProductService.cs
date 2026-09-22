using DMS.Application.DTOs.Common;
using DMS.Application.DTOs.Products;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;

namespace DMS.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            return MapToDto(product);
        }

        public async Task<ProductResponseDto> CreateAsync(
            CreateProductRequestDto request)
        {
            if (request.UnitPrice <= 0)
                throw new InvalidOperationException(
                    "Unit price must be greater than zero.");

            if (request.AvailableStock < 0)
                throw new InvalidOperationException(
                    "Available stock cannot be negative.");

            var existingProduct =
                await _productRepository.GetByProductCodeAsync(
                    request.ProductCode);

            if (existingProduct != null)
                throw new InvalidOperationException(
                    "Product code already exists.");

            var product = new Product
            {
                ProductCode = request.ProductCode.Trim(),
                Name = request.Name.Trim(),
                Category = request.Category.Trim(),
                UnitPrice = request.UnitPrice,
                AvailableStock = request.AvailableStock,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductResponseDto> UpdateAsync(
            int id,
            UpdateProductRequestDto request)
        {
            if (request.UnitPrice <= 0)
                throw new InvalidOperationException(
                    "Unit price must be greater than zero.");

            if (request.AvailableStock < 0)
                throw new InvalidOperationException(
                    "Available stock cannot be negative.");

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            product.Name = request.Name.Trim();
            product.Category = request.Category.Trim();
            product.UnitPrice = request.UnitPrice;
            product.AvailableStock = request.AvailableStock;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);

            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            var hasOrderItems =
                await _productRepository.HasOrderItemsAsync(id);

            if (hasOrderItems)
            {
                throw new InvalidOperationException(
                    "Product cannot be deleted because it is used in an order.");
            }

            _productRepository.Delete(product);

            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(
            int id,
            UpdateProductStatusRequestDto request)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);

            await _productRepository.SaveChangesAsync();
        }

        private static ProductResponseDto MapToDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Category = product.Category,
                UnitPrice = product.UnitPrice,
                AvailableStock = product.AvailableStock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<PagedResponseDto<ProductResponseDto>> GetCatalogAsync(
    string? search,
    int pageNumber,
    int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0.");

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.");

            var result = await _productRepository.GetActiveProductsAsync(
                search,
                pageNumber,
                pageSize);

            var products = result.Products
                .Select(product => new ProductResponseDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    Name = product.Name,
                    Category = product.Category,
                    UnitPrice = product.UnitPrice,
                    AvailableStock = product.AvailableStock,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                })
                .ToList();

            return new PagedResponseDto<ProductResponseDto>
            {
                Items = products,
                TotalCount = result.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize)
            };
        }
    }
}
