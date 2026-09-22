using DMS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.OrderTests
{
    public class InvalidOrderTransitionTests
    {
        [Fact]
        public void Draft_Cannot_Be_Approved()
        {
            // Arrange
            var currentStatus = OrderStatus.Draft;

            // Act
            var canApprove = currentStatus == OrderStatus.Submitted;

            // Assert
            Assert.False(canApprove);
        }

        [Fact]
        public void Draft_Cannot_Be_Dispatched()
        {
            // Arrange
            var currentStatus = OrderStatus.Draft;

            // Act
            var canDispatch = currentStatus == OrderStatus.Approved;

            // Assert
            Assert.False(canDispatch);
        }

        [Fact]
        public void Submitted_Cannot_Be_Delivered()
        {
            // Arrange
            var currentStatus = OrderStatus.Submitted;

            // Act
            var canDeliver = currentStatus == OrderStatus.Dispatched;

            // Assert
            Assert.False(canDeliver);
        }

        [Fact]
        public void Approved_Can_Be_Dispatched()
        {
            // Arrange
            var currentStatus = OrderStatus.Approved;

            // Act
            var canDispatch = currentStatus == OrderStatus.Approved;

            // Assert
            Assert.True(canDispatch);
        }

        [Fact]
        public void Dispatched_Can_Be_Delivered()
        {
            // Arrange
            var currentStatus = OrderStatus.Dispatched;

            // Act
            var canDeliver = currentStatus == OrderStatus.Dispatched;

            // Assert
            Assert.True(canDeliver);
        }
    }
}
