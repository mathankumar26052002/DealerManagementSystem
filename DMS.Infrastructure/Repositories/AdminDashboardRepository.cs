using DMS.Application.DTOs.Dashboard;
using DMS.Application.Interfaces;
using DMS.Domain.Enum;
using DMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly DmsDbContext _context;

        public AdminDashboardRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardResponseDto> GetDashboardAsync()
        {
            var totalDealers = await _context.Dealers.CountAsync();

            var activeDealers = await _context.Dealers
                .CountAsync(x => x.IsActive);

            var totalProducts = await _context.Products.CountAsync();

            var activeProducts = await _context.Products
                .CountAsync(x => x.IsActive);

            var submittedOrders = await _context.Orders
                .CountAsync(x => x.Status == OrderStatus.Submitted);

            var approvedOrders = await _context.Orders
                .CountAsync(x => x.Status == OrderStatus.Approved);

            return new AdminDashboardResponseDto
            {
                TotalDealers = totalDealers,
                ActiveDealers = activeDealers,
                TotalProducts = totalProducts,
                ActiveProducts = activeProducts,
                SubmittedOrders = submittedOrders,
                ApprovedOrders = approvedOrders
            };
        }
    }
}
