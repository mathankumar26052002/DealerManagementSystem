using DMS.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardResponseDto> GetDashboardAsync();
    }
}
