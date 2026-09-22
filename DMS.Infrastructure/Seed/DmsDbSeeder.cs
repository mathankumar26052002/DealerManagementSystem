using DMS.Domain.Entities;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Seed
{
    public static class DmsDbSeeder
    {
        public static async Task SeedAsync(DmsDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Dealers.AnyAsync())
            {
                var dealers = new List<Dealer>
            {
                new Dealer
                {
                    DealerCode = "D001",
                    CompanyName = "ABC Traders",
                    ContactPerson = "Arun Kumar",
                    Email = "arun@abctraders.com",
                    Phone = "9876543210",
                    Address = "Chennai, Tamil Nadu",
                    IsActive = true
                },
                new Dealer
                {
                    DealerCode = "D002",
                    CompanyName = "XYZ Enterprises",
                    ContactPerson = "Rahul Kumar",
                    Email = "rahul@xyzent.com",
                    Phone = "9876543211",
                    Address = "Thanjavur, Tamil Nadu",
                    IsActive = true
                }
            };

                await context.Dealers.AddRangeAsync(dealers);
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                var dealer1 = await context.Dealers
                    .FirstAsync(x => x.DealerCode == "D001");

                var dealer2 = await context.Dealers
                    .FirstAsync(x => x.DealerCode == "D002");

                var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                        "Admin@123"),
                    Role = "Admin",
                    IsActive = true
                },
                new User
                {
                    Username = "Mathan",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                        "Admin@123"),
                    Role = "Admin",
                    IsActive = true
                },

                new User
                {
                    Username = "dealer01",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                        "Dealer@123"),
                    Role = "Dealer",
                    DealerId = dealer1.Id,
                    IsActive = true
                },

                new User
                {
                    Username = "dealer02",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                        "Dealer@123"),
                    Role = "Dealer",
                    DealerId = dealer2.Id,
                    IsActive = true
                }
            };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            if (!await context.Products.AnyAsync())
            {
                var products = new List<Product>
            {
                new Product
                {
                    ProductCode = "P001",
                    Name = "Laptop",
                    Category = "Electronics",
                    UnitPrice = 50000,
                    AvailableStock = 20,
                    IsActive = true
                },

                new Product
                {
                    ProductCode = "P002",
                    Name = "Wireless Mouse",
                    Category = "Accessories",
                    UnitPrice = 1000,
                    AvailableStock = 50,
                    IsActive = true
                },

                new Product
                {
                    ProductCode = "P003",
                    Name = "Keyboard",
                    Category = "Accessories",
                    UnitPrice = 1500,
                    AvailableStock = 40,
                    IsActive = true
                },

                new Product
                {
                    ProductCode = "P004",
                    Name = "Monitor",
                    Category = "Electronics",
                    UnitPrice = 15000,
                    AvailableStock = 15,
                    IsActive = true
                }
            };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
