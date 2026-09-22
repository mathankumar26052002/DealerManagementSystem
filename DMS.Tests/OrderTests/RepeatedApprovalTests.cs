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
    public class RepeatedApprovalTests : TestBase
    {
        [Fact]
        public async Task Repeated_Approval_Should_Not_Deduct_Stock_Twice()
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

            // Create Product
            var product = new Product
            {
                ProductCode = $"PROD-{Guid.NewGuid():N}".Substring(0, 10),
                Name = "Test Product",
                Category = "Test",
                UnitPrice = 1000,
                AvailableStock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            context.Dealers.Add(dealer);
            context.Products.Add(product);

            await context.SaveChangesAsync();

            // Create Submitted Order
            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}",
                DealerId = dealer.Id,
                Status = OrderStatus.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductCodeSnapshot = product.ProductCode,
                ProductNameSnapshot = product.Name,
                UnitPrice = product.UnitPrice,
                Quantity = 5,
                LineTotal = product.UnitPrice * 5
            });

            order.TotalAmount =
                order.OrderItems.Sum(x => x.LineTotal);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var orderId = order.Id;
            var productId = product.Id;
            var adminId = admin.Id;

            // Create actual repository
            var repository = new OrderRepository(context);

            // --------------------------------------------------
            // FIRST APPROVAL
            // --------------------------------------------------

            await repository.ApproveOrderAsync(
                orderId,
                adminId);

            // Verify first approval
            var afterFirstApproval = await context.Products
                .AsNoTracking()
                .FirstAsync(x => x.Id == productId);

            Assert.Equal(
                5,
                afterFirstApproval.AvailableStock);

            var orderAfterFirstApproval = await context.Orders
                .AsNoTracking()
                .FirstAsync(x => x.Id == orderId);

            Assert.Equal(
                OrderStatus.Approved,
                orderAfterFirstApproval.Status);

            // --------------------------------------------------
            // SECOND APPROVAL
            // --------------------------------------------------

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                {
                    await repository.ApproveOrderAsync(
                        orderId,
                        adminId);
                });

            // Assert exception
            Assert.Contains(
                "Only submitted orders can be approved",
                exception.Message);

            // --------------------------------------------------
            // VERIFY STOCK WAS NOT DEDUCTED AGAIN
            // --------------------------------------------------

            var afterSecondApproval = await context.Products
                .AsNoTracking()
                .FirstAsync(x => x.Id == productId);

            Assert.Equal(
                5,
                afterSecondApproval.AvailableStock);

            // Verify order is still Approved
            var finalOrder = await context.Orders
                .AsNoTracking()
                .FirstAsync(x => x.Id == orderId);

            Assert.Equal(
                OrderStatus.Approved,
                finalOrder.Status);
        }
    }
}
