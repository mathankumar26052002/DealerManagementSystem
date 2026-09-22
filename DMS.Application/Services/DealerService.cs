using DMS.Application.DTOs.Dealers;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;


namespace DMS.Application.Services
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IUserRepository _userRepository;



        public DealerService(IDealerRepository dealerRepository,IUserRepository userRepository)
        {
            _dealerRepository = dealerRepository;
            _userRepository = userRepository;
        }

        public async Task<List<DealerResponseDto>> GetAllAsync()
        {
            var dealers = await _dealerRepository.GetAllAsync();

            return dealers
                .Select(MapToDto)
                .ToList();
        }

        public async Task<DealerResponseDto> GetByIdAsync(int id)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);

            if (dealer == null)
                throw new KeyNotFoundException("Dealer not found.");

            return MapToDto(dealer);
        }

        public async Task<DealerResponseDto> CreateAsync(
            CreateDealerRequestDto request)
        {
            var existingCode =
                await _dealerRepository.GetByDealerCodeAsync(request.DealerCode);

            if (existingCode != null)
                throw new InvalidOperationException(
                    "Dealer code already exists.");

            var existingEmail =
                await _dealerRepository.GetByEmailAsync(request.Email);

            if (existingEmail != null)
                throw new InvalidOperationException(
                    "Dealer email already exists.");

            var dealer = new Dealer
            {
                DealerCode = request.DealerCode.Trim(),
                CompanyName = request.CompanyName.Trim(),
                ContactPerson = request.ContactPerson.Trim(),
                Email = request.Email.Trim(),
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _dealerRepository.AddAsync(dealer);
            await _dealerRepository.SaveChangesAsync();

            return MapToDto(dealer);
        }

        public async Task<DealerResponseDto> UpdateAsync(
            int id,
            UpdateDealerRequestDto request)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);

            if (dealer == null)
                throw new KeyNotFoundException("Dealer not found.");

            var existingEmail =
                await _dealerRepository.GetByEmailAsync(request.Email);

            if (existingEmail != null && existingEmail.Id != id)
                throw new InvalidOperationException(
                    "Dealer email already exists.");

            dealer.CompanyName = request.CompanyName.Trim();
            dealer.ContactPerson = request.ContactPerson.Trim();
            dealer.Email = request.Email.Trim();
            dealer.Phone = request.Phone.Trim();
            dealer.Address = request.Address.Trim();
            dealer.UpdatedAt = DateTime.UtcNow;

            _dealerRepository.Update(dealer);

            await _dealerRepository.SaveChangesAsync();

            return MapToDto(dealer);
        }

        public async Task DeleteAsync(int id)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);

            if (dealer == null)
                throw new KeyNotFoundException("Dealer not found.");

            var hasOrders = await _dealerRepository.HasOrdersAsync(id);

            if (hasOrders)
            {
                throw new InvalidOperationException(
                    "Dealer cannot be deleted because orders exist.");
            }

            var hasUsers = await _dealerRepository.HasUsersAsync(id);

            if (hasUsers)
            {
                throw new InvalidOperationException(
                    "Dealer cannot be deleted because users exist.");
            }

            _dealerRepository.Delete(dealer);

            await _dealerRepository.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(
      int id,
      UpdateDealerStatusRequestDto request)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);

            if (dealer == null)
                throw new KeyNotFoundException("Dealer not found.");

            dealer.IsActive = request.IsActive;
            dealer.UpdatedAt = DateTime.UtcNow;

            _dealerRepository.Update(dealer);


          
            var users = await _userRepository.GetByDealerIdAsync(id);

            foreach (var user in users)
            {
                user.IsActive = request.IsActive;
            }


            await _dealerRepository.SaveChangesAsync();
        }

        private static DealerResponseDto MapToDto(Dealer dealer)
        {
            return new DealerResponseDto
            {
                Id = dealer.Id,
                DealerCode = dealer.DealerCode,
                CompanyName = dealer.CompanyName,
                ContactPerson = dealer.ContactPerson,
                Email = dealer.Email,
                Phone = dealer.Phone,
                Address = dealer.Address,
                IsActive = dealer.IsActive,
                CreatedAt = dealer.CreatedAt,
                UpdatedAt = dealer.UpdatedAt
            };
        }
    }
}
