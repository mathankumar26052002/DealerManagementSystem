using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        private const string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=DealerManagementDb_Test;Trusted_Connection=True;TrustServerCertificate=True";

        public static DmsDbContext Create()
        {
            var options = new DbContextOptionsBuilder<DmsDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new DmsDbContext(options);
        }
    }
}
