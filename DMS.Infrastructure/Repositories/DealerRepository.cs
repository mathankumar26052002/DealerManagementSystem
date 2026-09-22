using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly DmsDbContext _context;

        public DealerRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dealer>> GetAllAsync()
        {
            return await _context.Dealers
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Dealer?> GetByIdAsync(int id)
        {
            return await _context.Dealers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Dealer?> GetByDealerCodeAsync(string dealerCode)
        {
            return await _context.Dealers
                .FirstOrDefaultAsync(x => x.DealerCode == dealerCode);
        }

        public async Task<Dealer?> GetByEmailAsync(string email)
        {
            return await _context.Dealers
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAsync(Dealer dealer)
        {
            await _context.Dealers.AddAsync(dealer);
        }

        public void Update(Dealer dealer)
        {
            _context.Dealers.Update(dealer);
        }

        public void Delete(Dealer dealer)
        {
            _context.Dealers.Remove(dealer);
        }

        public async Task<bool> HasOrdersAsync(int dealerId)
        {
            return await _context.Orders
                .AnyAsync(x => x.DealerId == dealerId);
        }

        public async Task<bool> HasUsersAsync(int dealerId)
        {
            return await _context.Users
                .AnyAsync(x => x.DealerId == dealerId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
