using DMS.Application.Interfaces;

namespace DMS.API.Extensions
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetDealerId()
        {
            var dealerId = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("dealerId")
                ?.Value;

            if (!int.TryParse(dealerId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Dealer information is missing from token.");
            }

            return id;
        }

        public int GetUserId()
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            if (!int.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "User information is missing from token.");
            }

            return id;
        }
    }
}
