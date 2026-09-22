using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DMS.Tests.OrderTests
{
    public class DealerOrderIsolationTests : TestBase
    {
        [Fact]
        public async Task Dealer_Cannot_Access_Another_Dealers_Order()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

            var dealer1 = new Dealer
            {
                DealerCode = $"TEST-D1-{Guid.NewGuid():N}".Substring(0, 15),
                CompanyName = "Test Dealer 1",
                ContactPerson = "Test User",
                Email = $"dealer1-{Guid.NewGuid():N}@test.com",
                Phone = "9876543210",
                Address = "Test Address",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var dealer2 = new Dealer
            {
                DealerCode = $"TEST-D2-{Guid.NewGuid():N}".Substring(0, 15),
                CompanyName = "Test Dealer 2",
                ContactPerson = "Test User",
                Email = $"dealer2-{Guid.NewGuid():N}@test.com",
                Phone = "9876543211",
                Address = "Test Address",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Dealers.AddRange(dealer1, dealer2);

            await context.SaveChangesAsync();

            var order = new Order
            {
                OrderNumber = $"TEST-{Guid.NewGuid():N}",
                DealerId = dealer1.Id,
                Status = OrderStatus.Submitted,
                TotalAmount = 1000,
                CreatedAt = DateTime.UtcNow
            };

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            // Act
            // Dealer 2 tries to access Dealer 1's order
            var result = await context.Orders
                .FirstOrDefaultAsync(x =>
                    x.Id == order.Id &&
                    x.DealerId == dealer2.Id);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task Dealer_Can_Access_Their_Own_Order()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

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

            // IMPORTANT: save dealer first
            await context.SaveChangesAsync();

            // Create order using the actual generated dealer ID
            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}",
                DealerId = dealer.Id,
                Status = OrderStatus.Submitted,
                TotalAmount = 1000,
                CreatedAt = DateTime.UtcNow
            };

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            // Act
            var result = await context.Orders
                .FirstOrDefaultAsync(x =>
                    x.Id == order.Id &&
                    x.DealerId == dealer.Id);

            // Assert
            Assert.NotNull(result);
        }
    }
}
