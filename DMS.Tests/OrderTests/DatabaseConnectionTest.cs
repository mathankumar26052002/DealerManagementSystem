using DMS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.OrderTests
{
    public class DatabaseConnectionTest : TestBase
    {
        [Fact]
        public async Task Test_Database_Connection()
        {
            // Arrange
            await InitializeDatabaseAsync();

            await using var context = TestDbContextFactory.Create();

            // Act
            var canConnect = await context.Database.CanConnectAsync();

            // Assert
            Assert.True(canConnect);
        }
    }
}
