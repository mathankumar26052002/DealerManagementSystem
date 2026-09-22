using DMS.Application.DTOs.Common;
using DMS.Application.DTOs.Orders;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.Enum;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDealerRepository _dealerRepository;

        private readonly ILogger<OrderService> _logger;


        public OrderService(
     IOrderRepository orderRepository,
     IProductRepository productRepository,
     IDealerRepository dealerRepository,
     ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _dealerRepository = dealerRepository;
            _logger = logger;
        }

        public async Task<DTOs.Orders.OrderResponseDto> CreateDraftAsync(
            int dealerId,
            DTOs.Orders.CreateOrderRequestDto request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new InvalidOperationException(
                    "Order must contain at least one item.");
            }

            var dealer = await _dealerRepository.GetByIdAsync(dealerId);

            if (dealer == null)
            {
                throw new KeyNotFoundException(
                    "Dealer not found.");
            }

            if (!dealer.IsActive)
            {
                throw new InvalidOperationException(
                    "Inactive dealer cannot create an order.");
            }

            // Prevent duplicate products in the same order.
            var duplicateProductIds = request.Items
                .GroupBy(x => x.ProductId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateProductIds.Count > 0)
            {
                throw new InvalidOperationException(
                    "The same product cannot be added more than once.");
            }

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                DealerId = dealerId,
                Status = OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var requestItem in request.Items)
            {
                if (requestItem.Quantity <= 0)
                {
                    throw new InvalidOperationException(
                        "Quantity must be greater than zero.");
                }

                var product = await _productRepository
                    .GetByIdAsync(requestItem.ProductId);

                if (product == null)
                {
                    throw new KeyNotFoundException(
                        $"Product {requestItem.ProductId} not found.");
                }

                if (!product.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product '{product.Name}' is inactive.");
                }

                var lineTotal =
                    product.UnitPrice * requestItem.Quantity;

                var item = new OrderItem
                {
                    ProductId = product.Id,
                    ProductCodeSnapshot = product.ProductCode,
                    ProductNameSnapshot = product.Name,

                    // Current price is stored for the draft.
                    // It will be refreshed again during submission.
                    UnitPrice = product.UnitPrice,

                    Quantity = requestItem.Quantity,
                    LineTotal = lineTotal
                };

                order.OrderItems.Add(item);

                total += lineTotal;
            }

            order.TotalAmount = total;

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<PagedResponseDto<OrderResponseDto>>
     GetMyOrdersAsync(
         int dealerId,
         OrderQueryDto query)
        {
            if (query.PageNumber < 1)
                query.PageNumber = 1;

            if (query.PageSize < 1)
                query.PageSize = 10;

            if (query.PageSize > 100)
                query.PageSize = 100;

            var result =
                await _orderRepository.GetDealerOrdersAsync(
                    dealerId,
                    query);

            return new PagedResponseDto<OrderResponseDto>
            {
                Items = result.Orders
                    .Select(MapToDto)
                    .ToList(),

                PageNumber = query.PageNumber,

                PageSize = query.PageSize,

                TotalCount = result.TotalCount
            };
        }

        public async Task<OrderResponseDto> GetMyOrderByIdAsync(
            int dealerId,
            int orderId)
        {
            var order =
                await _orderRepository.GetDealerOrderByIdAsync(
                    orderId,
                    dealerId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            return MapToDto(order);
        }


        public async Task DispatchAsync(
    int orderId,
    int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved orders can be dispatched.");
            }

            var previousStatus = order.Status;

            order.Status = OrderStatus.Dispatched;

            _orderRepository.Update(order);

            await _orderRepository.AddStatusHistoryAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Dispatched,
                    ChangedByUserId = userId,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = "Order Dispatched"
                });

            await _orderRepository.SaveChangesAsync();
        }

        public async Task ApproveAsync(
     int orderId,
     int userId)
        {
            _logger.LogInformation(
                "Approving order {OrderId} by user {UserId}",
                orderId,
                userId);

            try
            {
                await _orderRepository.ApproveOrderAsync(
                    orderId,
                    userId);

                _logger.LogInformation(
                    "Order {OrderId} approved successfully by user {UserId}",
                    orderId,
                    userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to approve order {OrderId} by user {UserId}",
                    orderId,
                    userId);

                throw;
            }
        }

        public async Task DeliverAsync(
    int orderId,
    int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Dispatched)
            {
                throw new InvalidOperationException(
                    "Only dispatched orders can be delivered.");
            }

            var previousStatus = order.Status;

            order.Status = OrderStatus.Delivered;

            _orderRepository.Update(order);

            await _orderRepository.AddStatusHistoryAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Delivered,
                    ChangedByUserId = userId,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = "Order Delivered"
                });

            await _orderRepository.SaveChangesAsync();
        }


        public async Task<DTOs.Orders.OrderResponseDto> UpdateItemAsync(
            int dealerId,
            int orderId,
            int itemId,
            DTOs.Orders.UpdateOrderItemRequestDto request)
        {
            if (request.Quantity <= 0)
            {
                throw new InvalidOperationException(
                    "Quantity must be greater than zero.");
            }

            var order =
                await _orderRepository.GetDealerOrderByIdAsync(
                    orderId,
                    dealerId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft orders can be edited.");
            }

            var item = order.OrderItems
                .FirstOrDefault(x => x.Id == itemId);

            if (item == null)
            {
                throw new KeyNotFoundException(
                    "Order item not found.");
            }

            var product =
                await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    "Product not found.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    "Inactive product cannot be ordered.");
            }

            item.Quantity = request.Quantity;

            item.LineTotal =
                item.UnitPrice * item.Quantity;

            order.TotalAmount =
                order.OrderItems.Sum(x => x.LineTotal);

            _orderRepository.Update(order);

            await _orderRepository.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task RemoveItemAsync(
            int dealerId,
            int orderId,
            int itemId)
        {
            var order =
                await _orderRepository.GetDealerOrderByIdAsync(
                    orderId,
                    dealerId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft orders can be edited.");
            }

            var item = order.OrderItems
                .FirstOrDefault(x => x.Id == itemId);

            if (item == null)
            {
                throw new KeyNotFoundException(
                    "Order item not found.");
            }

            order.OrderItems.Remove(item);

            order.TotalAmount =
                order.OrderItems.Sum(x => x.LineTotal);

            if (order.OrderItems.Count == 0)
            {
                throw new InvalidOperationException(
                    "Order must contain at least one item.");
            }

            _orderRepository.Update(order);

            await _orderRepository.SaveChangesAsync();
        }

        public async Task SubmitAsync(
            int dealerId,
            int orderId)
        {
            _logger.LogInformation(
    "Submitting order {OrderId} by dealer {DealerId}",
    orderId,
    dealerId);

            var order =
                await _orderRepository.GetDealerOrderByIdAsync(
                    orderId,
                    dealerId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft orders can be submitted.");
            }

            if (order.OrderItems.Count == 0)
            {
                throw new InvalidOperationException(
                    "Order must contain at least one item.");
            }

            decimal total = 0;

            foreach (var item in order.OrderItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException(
                        "Quantity must be greater than zero.");
                }

                var product =
                    await _productRepository.GetByIdAsync(
                        item.ProductId);

                if (product == null)
                {
                    throw new InvalidOperationException(
                        $"Product '{item.ProductNameSnapshot}' no longer exists.");
                }

                if (!product.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product '{product.Name}' is inactive.");
                }

                // IMPORTANT:
                // Refresh price at submission time.
                item.ProductCodeSnapshot = product.ProductCode;
                item.ProductNameSnapshot = product.Name;
                item.UnitPrice = product.UnitPrice;
                item.LineTotal =
                    product.UnitPrice * item.Quantity;

                total += item.LineTotal;
            }

            order.TotalAmount = total;

            var previousStatus = order.Status;

            order.Status = OrderStatus.Submitted;
            order.SubmittedAt = DateTime.UtcNow;

            _orderRepository.Update(order);

            await _orderRepository.AddStatusHistoryAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Submitted,
                    ChangedByUserId = dealerId,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = "Order submitted by dealer."
                });

            await _orderRepository.SaveChangesAsync();
            _logger.LogInformation(
    "Order {OrderId} submitted successfully by dealer {DealerId}",
    orderId,
    dealerId);
        }

        public async Task CancelAsync(
            int dealerId,
            int orderId)
        {
            var order =
                await _orderRepository.GetDealerOrderByIdAsync(
                    orderId,
                    dealerId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Draft &&
                order.Status != OrderStatus.Submitted)
            {
                throw new InvalidOperationException(
                    "Only Draft or Submitted orders can be cancelled.");
            }

            var previousStatus = order.Status;

            order.Status = OrderStatus.Cancelled;

            _orderRepository.Update(order);

            await _orderRepository.AddStatusHistoryAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Cancelled,
                    ChangedByUserId = dealerId,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = "Order cancelled by dealer."
                });

            await _orderRepository.SaveChangesAsync();
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            return orders
                .Select(MapToDto)
                .ToList();
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(
    int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            return MapToDto(order);
        }

        public async Task RejectAsync(
    int orderId,
    int userId,
    string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new InvalidOperationException(
                    "Rejection reason is required.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(
                    "Order not found.");
            }

            if (order.Status != OrderStatus.Submitted)
            {
                throw new InvalidOperationException(
                    "Only submitted orders can be rejected.");
            }

            var previousStatus = order.Status;

            order.Status = OrderStatus.Rejected;

            _orderRepository.Update(order);

            await _orderRepository.AddStatusHistoryAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Rejected,
                    ChangedByUserId = userId,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = reason.Trim()
                });

            await _orderRepository.SaveChangesAsync();
        }





        private static OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                DealerId = order.DealerId,
                DealerCode = order.Dealer?.DealerCode ?? string.Empty,
                CompanyName = order.Dealer?.CompanyName ?? string.Empty,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                SubmittedAt = order.SubmittedAt,
                ApprovedAt = order.ApprovedAt,

                Items = order.OrderItems
                    .Select(item => new OrderItemResponseDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCodeSnapshot,
                        ProductName = item.ProductNameSnapshot,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal
                    })
                    .ToList(),

                StatusHistory = order.StatusHistories
                    .OrderBy(x => x.ChangedAt)
                    .Select(history => new OrderStatusHistoryResponseDto
                    {
                        Id = history.Id,
                        PreviousStatus = history.PreviousStatus == null
                            ? null
                            : history.PreviousStatus.ToString(),
                        NewStatus = history.NewStatus.ToString(),
                        ChangedByUserId = history.ChangedByUserId,
                        ChangedByUsername =
                            history.ChangedByUser?.Username ?? string.Empty,
                        ChangedAt = history.ChangedAt,
                        Remarks = history.Remarks
                    })
                    .ToList()
            };
        }
    }
}
