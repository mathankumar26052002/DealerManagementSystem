using DMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IDealerRepository
    {
        Task<List<Dealer>> GetAllAsync();

        Task<Dealer?> GetByIdAsync(int id);

        Task<Dealer?> GetByDealerCodeAsync(string dealerCode);

        Task<Dealer?> GetByEmailAsync(string email);

        Task AddAsync(Dealer dealer);

        void Update(Dealer dealer);

        void Delete(Dealer dealer);

        Task<bool> HasOrdersAsync(int dealerId);

        Task<bool> HasUsersAsync(int dealerId);

        Task SaveChangesAsync();
    }
}
