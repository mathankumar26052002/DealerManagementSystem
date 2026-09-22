using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Infrastructure.Repositories;
using DMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.OrderTests
{
    public class ConcurrentApprovalTests : TestBase
    {
        [Fact]
        public async Task Concurrent_Approvals_Should_Not_Oversell_Stock()
        {
            // Arrange
            await InitializeDatabaseAsync();

            // --------------------------------------------------
            // SETUP DATABASE
            // --------------------------------------------------

            await using (var setupContext = TestDbContextFactory.Create())
            {
                var admin = new User
                {
                    Username = $"admin_{Guid.NewGuid():N}",
                    PasswordHash = "test-password",
                    Role = "Admin",
                    IsActive = true
                };

                var dealer1 = new Dealer
                {
                    DealerCode = $"D1-{Guid.NewGuid():N}".Substring(0, 10),
                    CompanyName = "Test Dealer 1",
                    ContactPerson = "Dealer 1",
                    Email = $"dealer1-{Guid.NewGuid():N}@test.com",
                    Phone = "9876543210",
                    Address = "Test Address",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var dealer2 = new Dealer
                {
                    DealerCode = $"D2-{Guid.NewGuid():N}".Substring(0, 10),
                    CompanyName = "Test Dealer 2",
                    ContactPerson = "Dealer 2",
                    Email = $"dealer2-{Guid.NewGuid():N}@test.com",
                    Phone = "9876543211",
                    Address = "Test Address",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var product = new Product
                {
                    ProductCode = $"PROD-{Guid.NewGuid():N}".Substring(0, 10),
                    Name = "Test Product",
                    Category = "Test",
                    UnitPrice = 1000,
                    AvailableStock = 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                setupContext.Users.Add(admin);

                setupContext.Dealers.AddRange(
                    dealer1,
                    dealer2);

                setupContext.Products.Add(product);

                await setupContext.SaveChangesAsync();

                // Order 1
                var order1 = new Order
                {
                    OrderNumber = $"ORD-{Guid.NewGuid():N}",
                    DealerId = dealer1.Id,
                    Status = OrderStatus.Submitted,
                    CreatedAt = DateTime.UtcNow
                };

                order1.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductCodeSnapshot = product.ProductCode,
                    ProductNameSnapshot = product.Name,
                    UnitPrice = product.UnitPrice,
                    Quantity = 5,
                    LineTotal = product.UnitPrice * 5
                });

                order1.TotalAmount =
                    order1.OrderItems.Sum(x => x.LineTotal);

                // Order 2
                var order2 = new Order
                {
                    OrderNumber = $"ORD-{Guid.NewGuid():N}",
                    DealerId = dealer2.Id,
                    Status = OrderStatus.Submitted,
                    CreatedAt = DateTime.UtcNow
                };

                order2.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductCodeSnapshot = product.ProductCode,
                    ProductNameSnapshot = product.Name,
                    UnitPrice = product.UnitPrice,
                    Quantity = 5,
                    LineTotal = product.UnitPrice * 5
                });

                order2.TotalAmount =
                    order2.OrderItems.Sum(x => x.LineTotal);

                setupContext.Orders.AddRange(
                    order1,
                    order2);

                await setupContext.SaveChangesAsync();

                // Store generated IDs
                var order1Id = order1.Id;
                var order2Id = order2.Id;
                var adminId = admin.Id;
                var productId = product.Id;

                // --------------------------------------------------
                // CONCURRENT APPROVAL
                // --------------------------------------------------

                var task1 = ApproveOrderAsync(
                    order1Id,
                    adminId);

                var task2 = ApproveOrderAsync(
                    order2Id,
                    adminId);

                var results = await Task.WhenAll(
                    CaptureResultAsync(task1),
                    CaptureResultAsync(task2));

                // --------------------------------------------------
                // ASSERT
                // --------------------------------------------------

                var successCount =
                    results.Count(x => x.Success);

                var failureCount =
                    results.Count(x => !x.Success);

                // Exactly one approval must succeed
                Assert.Equal(1, successCount);

                // Exactly one approval must fail
                Assert.Equal(1, failureCount);

                // Verify final stock
                await using var verifyContext =
                    TestDbContextFactory.Create();

                var finalProduct = await verifyContext.Products
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == productId);

                Assert.Equal(
                    0,
                    finalProduct.AvailableStock);

                // Verify order statuses
                var finalOrder1 = await verifyContext.Orders
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == order1Id);

                var finalOrder2 = await verifyContext.Orders
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == order2Id);

                var approvedCount = new[]
                {
                finalOrder1.Status,
                finalOrder2.Status
            }
                .Count(x => x == OrderStatus.Approved);

                Assert.Equal(
                    1,
                    approvedCount);
            }
        }


        private static async Task ApproveOrderAsync(
            int orderId,
            int adminId)
        {
            await using var context =
                TestDbContextFactory.Create();

            var repository =
                new OrderRepository(context);

            await repository.ApproveOrderAsync(
                orderId,
                adminId);
        }


        private static async Task<TestResult> CaptureResultAsync(
            Task task)
        {
            try
            {
                await task;

                return new TestResult
                {
                    Success = true
                };
            }
            catch
            {
                return new TestResult
                {
                    Success = false
                };
            }
        }


        private class TestResult
        {
            public bool Success { get; set; }
        }
    }
}
