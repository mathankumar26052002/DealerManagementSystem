using DMS.Application.DTOs.Dashboard;
using DMS.Application.Interfaces;

namespace DMS.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _dashboardRepository;

        public AdminDashboardService(
            IAdminDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<AdminDashboardResponseDto> GetDashboardAsync()
        {
            return await _dashboardRepository.GetDashboardAsync();
        }
    }
}
