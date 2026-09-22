using DMS.Application.DTOs.Orders;
using DMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);

        Task<Order?> GetDealerOrderByIdAsync(
            int orderId,
            int dealerId);

        Task<(List<Order> Orders, int TotalCount)> GetDealerOrdersAsync(
       int dealerId,
       OrderQueryDto query);

        Task<List<Order>> GetAllOrdersAsync();


        Task AddAsync(Order order);

        void Update(Order order);

        void Delete(Order order);

        Task SaveChangesAsync();

        Task AddStatusHistoryAsync(OrderStatusHistory history);

        Task ApproveOrderAsync(
    int orderId,
    int userId);
    }
}
