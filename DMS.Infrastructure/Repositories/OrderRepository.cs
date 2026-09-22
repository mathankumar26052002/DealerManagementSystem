using DMS.Application.DTOs.Orders;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DmsDbContext _context;

        public OrderRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(x => x.Dealer)
                .Include(x => x.OrderItems)
                .Include(x => x.StatusHistories)
                    .ThenInclude(x => x.ChangedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task AddStatusHistoryAsync(
    OrderStatusHistory history)
        {
            await _context.OrderStatusHistories.AddAsync(history);
        }

        public async Task ApproveOrderAsync(
    int orderId,
    int userId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(x => x.OrderItems)
                    .FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException(
                        "Order not found.");
                }

                if (order.Status != OrderStatus.Submitted)
                {
                    throw new InvalidOperationException(
                        "Only submitted orders can be approved.");
                }

                foreach (var item in order.OrderItems)
                {
                    var rowsAffected = await _context.Database
                        .ExecuteSqlInterpolatedAsync($"""
                    UPDATE Products
                    SET AvailableStock = AvailableStock - {item.Quantity}
                    WHERE Id = {item.ProductId}
                      AND AvailableStock >= {item.Quantity}
                    """);

                    if (rowsAffected != 1)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for product '{item.ProductNameSnapshot}'.");
                    }
                }

                var previousStatus = order.Status;

                order.Status = OrderStatus.Approved;
                order.ApprovedAt = DateTime.UtcNow;

                _context.OrderStatusHistories.Add(
                    new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        PreviousStatus = previousStatus,
                        NewStatus = OrderStatus.Approved,
                        ChangedByUserId = userId,
                        ChangedAt = DateTime.UtcNow,
                        Remarks = "Order approved."
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Dealer)
                .Include(x => x.OrderItems)
                .Include(x => x.StatusHistories)
                    .ThenInclude(x => x.ChangedByUser)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetDealerOrderByIdAsync(
      int orderId,
      int dealerId)
        {
            return await _context.Orders
                .Include(x => x.Dealer)
                .Include(x => x.OrderItems)
                .Include(x => x.StatusHistories)
                    .ThenInclude(x => x.ChangedByUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == orderId &&
                    x.DealerId == dealerId);
        }

        public async Task<(List<Order> Orders, int TotalCount)>
     GetDealerOrdersAsync(
         int dealerId,
         OrderQueryDto query)
        {
            var ordersQuery = _context.Orders
                .AsNoTracking()
                .Include(x => x.Dealer)
                .Include(x => x.OrderItems)
                .Where(x => x.DealerId == dealerId);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                ordersQuery = ordersQuery.Where(x =>
                    x.OrderNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<OrderStatus>(
                    query.Status,
                    true,
                    out var status))
            {
                ordersQuery = ordersQuery.Where(
                    x => x.Status == status);
            }

            var totalCount = await ordersQuery.CountAsync();

            var orders = await ordersQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
