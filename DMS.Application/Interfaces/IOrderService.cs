using DMS.Application.DTOs.Common;
using DMS.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateDraftAsync(
            int dealerId,
            CreateOrderRequestDto request);

        Task<PagedResponseDto<OrderResponseDto>> GetMyOrdersAsync(
    int dealerId,
    OrderQueryDto query);

        Task<OrderResponseDto> GetMyOrderByIdAsync(
            int dealerId,
            int orderId);

        Task<OrderResponseDto> UpdateItemAsync(
            int dealerId,
            int orderId,
            int itemId,
            UpdateOrderItemRequestDto request);

        Task RemoveItemAsync(
            int dealerId,
            int orderId,
            int itemId);

        Task SubmitAsync(
            int dealerId,
            int orderId);

        Task CancelAsync(
            int dealerId,
            int orderId);

        Task<List<OrderResponseDto>> GetAllOrdersAsync();

        Task<OrderResponseDto> GetOrderByIdAsync(int orderId);

        Task ApproveAsync(
    int orderId,
    int userId);

        Task RejectAsync(
            int orderId,
            int userId,
            string reason);

        Task DispatchAsync(
            int orderId,
            int userId);

        Task DeliverAsync(
            int orderId,
            int userId);
    }
}
