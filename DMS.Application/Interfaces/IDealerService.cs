using DMS.Application.DTOs.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IDealerService
    {
        Task<List<DealerResponseDto>> GetAllAsync();

        Task<DealerResponseDto> GetByIdAsync(int id);

        Task<DealerResponseDto> CreateAsync(CreateDealerRequestDto request);

        Task<DealerResponseDto> UpdateAsync(
            int id,
            UpdateDealerRequestDto request);

        Task DeleteAsync(int id);

        Task UpdateStatusAsync(
            int id,
            UpdateDealerStatusRequestDto request);
    }
}
