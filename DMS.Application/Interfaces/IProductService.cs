using DMS.Application.DTOs.Common;
using DMS.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();

        Task<ProductResponseDto> GetByIdAsync(int id);

        Task<ProductResponseDto> CreateAsync(
            CreateProductRequestDto request);

        Task<ProductResponseDto> UpdateAsync(
            int id,
            UpdateProductRequestDto request);

        Task DeleteAsync(int id);

        Task UpdateStatusAsync(
            int id,
            UpdateProductStatusRequestDto request);

        Task<PagedResponseDto<ProductResponseDto>> GetCatalogAsync(
    string? search,
    int pageNumber,
    int pageSize);
    }
}
