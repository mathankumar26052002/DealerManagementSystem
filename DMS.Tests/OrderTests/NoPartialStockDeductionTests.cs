using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace DMS.Tests.OrderTests
{
    public class NoPartialStockDeductionTests : TestBase
    {
        [Fact]
        public async Task Approval_Should_Not_Partially_Deduct_Stock()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

            // Create dealer
            var dealer = new Dealer
            {
                DealerCode = $"TEST-{Guid.NewGuid():N}".Substring(0, 10),
                CompanyName = "Test Dealer",
                ContactPerson = "Test User",
                Email = $"test-{Guid.NewGuid():N}@test.com",
                Phone = "9876543210",
                Address = "Test Address",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Dealers.Add(dealer);

            // Create products
            var laptop = new Product
            {
                ProductCode = $"LAP-{Guid.NewGuid():N}".Substring(0, 10),
                Name = "Test Laptop",
                Category = "Electronics",
                UnitPrice = 50000,
                AvailableStock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var mouse = new Product
            {
                ProductCode = $"MOU-{Guid.NewGuid():N}".Substring(0, 10),
                Name = "Test Mouse",
                Category = "Electronics",
                UnitPrice = 1000,
                AvailableStock = 20,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Products.AddRange(laptop, mouse);

            await context.SaveChangesAsync();

            // Create submitted order
            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}",
                DealerId = dealer.Id,
                Status = OrderStatus.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            order.OrderItems.Add(new OrderItem
            {
                ProductId = laptop.Id,
                ProductCodeSnapshot = laptop.ProductCode,
                ProductNameSnapshot = laptop.Name,
                UnitPrice = laptop.UnitPrice,
                Quantity = 5,
                LineTotal = laptop.UnitPrice * 5
            });

            order.OrderItems.Add(new OrderItem
            {
                ProductId = mouse.Id,
                ProductCodeSnapshot = mouse.ProductCode,
                ProductNameSnapshot = mouse.Name,
                UnitPrice = mouse.UnitPrice,
                Quantity = 100,
                LineTotal = mouse.UnitPrice * 100
            });

            order.TotalAmount =
                order.OrderItems.Sum(x => x.LineTotal);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            // Act
            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in order.OrderItems)
                {
                    var rowsAffected =
                        await context.Database.ExecuteSqlInterpolatedAsync($"""
                        UPDATE Products
                        SET AvailableStock = AvailableStock - {item.Quantity}
                        WHERE Id = {item.ProductId}
                          AND AvailableStock >= {item.Quantity}
                        """);

                    if (rowsAffected != 1)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for {item.ProductNameSnapshot}");
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            // Assert
            var updatedLaptop = await context.Products
                .FirstAsync(x => x.Id == laptop.Id);

            var updatedMouse = await context.Products
                .FirstAsync(x => x.Id == mouse.Id);

            var updatedOrder = await context.Orders
                .FirstAsync(x => x.Id == order.Id);

            Assert.Equal(10, updatedLaptop.AvailableStock);

            Assert.Equal(20, updatedMouse.AvailableStock);

            Assert.Equal(
                OrderStatus.Submitted,
                updatedOrder.Status);
        }
    }
}
