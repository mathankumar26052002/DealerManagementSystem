using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Infrastructure.Repositories;
using DMS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DMS.Tests.OrderTests
{
    public class ApproveOrderTests : TestBase
    {
        [Fact]
        public async Task ApproveOrder_Should_Rollback_When_Stock_Is_Insufficient()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

            // Create Admin user
            var admin = new User
            {
                Username = $"testadmin_{Guid.NewGuid():N}",
                PasswordHash = "test-password",
                Role = "Admin",
                IsActive = true
            };

            // Create Dealer
            var dealer = new Dealer
            {
                DealerCode = $"TEST-{Guid.NewGuid():N}".Substring(0, 10),
                CompanyName = "Test Dealer",
                ContactPerson = "Test User",
                Email = $"dealer-{Guid.NewGuid():N}@test.com",
                Phone = "9876543210",
                Address = "Test Address",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Create Product 1
            // Enough stock
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

            // Create Product 2
            // Insufficient stock
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

            context.Users.Add(admin);
            context.Dealers.Add(dealer);
            context.Products.AddRange(laptop, mouse);

            // Save first so IDs are generated
            await context.SaveChangesAsync();

            // Create submitted order
            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}",
                DealerId = dealer.Id,
                Status = OrderStatus.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            // Laptop quantity = 5
            // Stock = 10 → sufficient
            order.OrderItems.Add(new OrderItem
            {
                ProductId = laptop.Id,
                ProductCodeSnapshot = laptop.ProductCode,
                ProductNameSnapshot = laptop.Name,
                UnitPrice = laptop.UnitPrice,
                Quantity = 5,
                LineTotal = laptop.UnitPrice * 5
            });

            // Mouse quantity = 100
            // Stock = 20 → insufficient
            order.OrderItems.Add(new OrderItem
            {
                ProductId = mouse.Id,
                ProductCodeSnapshot = mouse.ProductCode,
                ProductNameSnapshot = mouse.Name,
                UnitPrice = mouse.UnitPrice,
                Quantity = 100,
                LineTotal = mouse.UnitPrice * 100
            });

            order.TotalAmount = order.OrderItems.Sum(x => x.LineTotal);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            // Save IDs before calling repository
            var orderId = order.Id;
            var laptopId = laptop.Id;
            var mouseId = mouse.Id;
            var adminId = admin.Id;

            // Create actual repository
            var repository = new OrderRepository(context);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                {
                    await repository.ApproveOrderAsync(
                        orderId,
                        adminId);
                });

            // Assert 1: Correct exception
            Assert.Contains(
                "Insufficient stock",
                exception.Message);

            // Reload products from database
            var updatedLaptop = await context.Products
                .AsNoTracking()
                .FirstAsync(x => x.Id == laptopId);

            var updatedMouse = await context.Products
                .AsNoTracking()
                .FirstAsync(x => x.Id == mouseId);

            // Reload order from database
            var updatedOrder = await context.Orders
                .AsNoTracking()
                .FirstAsync(x => x.Id == orderId);

            // Assert 2: Laptop stock must NOT be deducted
            Assert.Equal(
                10,
                updatedLaptop.AvailableStock);

            // Assert 3: Mouse stock must NOT be deducted
            Assert.Equal(
                20,
                updatedMouse.AvailableStock);

            // Assert 4: Order must remain Submitted
            Assert.Equal(
                OrderStatus.Submitted,
                updatedOrder.Status);
        }
    }
}
