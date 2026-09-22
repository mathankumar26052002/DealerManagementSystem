using DMS.Domain.Entities;
using DMS.Domain.Enum;
using DMS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace DMS.Tests.OrderTests
{
    public class OrderPriceSnapshotTests : TestBase
    {
        [Fact]
        public async Task Order_Should_Keep_Submission_Price_When_Product_Price_Changes()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

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
                Name = "Test Laptop",
                Category = "Electronics",
                UnitPrice = 50000,
                AvailableStock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Dealers.Add(dealer);
            context.Products.Add(product);

            await context.SaveChangesAsync();

            // --------------------------------------------------
            // CREATE ORDER USING CURRENT PRODUCT PRICE
            // --------------------------------------------------

            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}",
                DealerId = dealer.Id,
                Status = OrderStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };

            var quantity = 2;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,

                // Snapshot values
                ProductCodeSnapshot = product.ProductCode,
                ProductNameSnapshot = product.Name,

                // Current price at submission
                UnitPrice = product.UnitPrice,

                Quantity = quantity,

                LineTotal = product.UnitPrice * quantity
            };

            order.OrderItems.Add(orderItem);

            order.TotalAmount =
                order.OrderItems.Sum(x => x.LineTotal);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            // Store the original order price
            var originalOrderPrice = orderItem.UnitPrice;
            var originalLineTotal = orderItem.LineTotal;

            // --------------------------------------------------
            // ADMIN CHANGES PRODUCT PRICE
            // --------------------------------------------------

            product.UnitPrice = 60000;

            await context.SaveChangesAsync();

            // --------------------------------------------------
            // RELOAD ORDER FROM DATABASE
            // --------------------------------------------------

            var savedOrder = await context.Orders
                .AsNoTracking()
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            var savedOrderItem = savedOrder.OrderItems
                .First();

            // --------------------------------------------------
            // ASSERT
            // --------------------------------------------------

            // Product price should now be 60,000
            var updatedProduct = await context.Products
                .AsNoTracking()
                .FirstAsync(x => x.Id == product.Id);

            Assert.Equal(
                60000,
                updatedProduct.UnitPrice);

            // But order price must remain 50,000
            Assert.Equal(
                50000,
                savedOrderItem.UnitPrice);

            // Quantity remains 2
            Assert.Equal(
                2,
                savedOrderItem.Quantity);

            // Line total remains 100,000
            Assert.Equal(
                100000,
                savedOrderItem.LineTotal);

            // Order total remains 100,000
            Assert.Equal(
                100000,
                savedOrder.TotalAmount);

            // Verify it matches the original snapshot
            Assert.Equal(
                originalOrderPrice,
                savedOrderItem.UnitPrice);

            Assert.Equal(
                originalLineTotal,
                savedOrderItem.LineTotal);
        }
    }
}
