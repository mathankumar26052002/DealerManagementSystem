using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DmsDbContext _context;

        public UserRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(x => x.Dealer)
                .FirstOrDefaultAsync(x =>
                    x.Username == username);
        }

        public async Task<List<User>> GetByDealerIdAsync(int dealerId)
        {
            return await _context.Users
                .Where(x => x.DealerId == dealerId)
                .ToListAsync();
        }
    }
}
