using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.Helpers
{
    public static class TestDatabaseSetup
    {
        public static async Task InitializeAsync()
        {
            await using var context = TestDbContextFactory.Create();

            await context.Database.MigrateAsync();
        }
    }
}
