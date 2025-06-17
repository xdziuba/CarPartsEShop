using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using System;

namespace CarPartsEShop.Seeders.Seeders
{
    public class ProductSeeder : ISeeder
    {
        public async Task Seed(ApplicationDbContext context)
        {
            if (context.Products.Any()) return;

            context.Products.AddRange(
                new Product
                {
                    Name = "Klocki hamulcowe Bosch",
                    Description = "Wysokiej jakości klocki hamulcowe firmy Bosch.",
                    Ean = "1234567890123",
                    Price = 149.99m,
                    Stock = 50,
                    CategoryId = 1
                },
                new Product
                {
                    Name = "Silnik 2.0 TDI",
                    Description = "Używany silnik wysokoprężny 2.0 TDI.",
                    Ean = "9876543210987",
                    Price = 4500.00m,
                    Stock = 5,
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Amortyzator przedni",
                    Description = "Amortyzator pasujący do modeli Audi A4.",
                    Ean = "1122334455667",
                    Price = 299.00m,
                    Stock = 20,
                    CategoryId = 3
                },
                new Product
                {
                    Name = "Reflektor LED Lewy",
                    Description = "Nowoczesny reflektor LED pasujący do VW Golf 7.",
                    Ean = "2233445566778",
                    Price = 799.99m,
                    Stock = 10,
                    CategoryId = 4
                },
                new Product
                {
                    Name = "Tłumik końcowy Bosal",
                    Description = "Tłumik wydechu renomowanej firmy Bosal.",
                    Ean = "3344556677889",
                    Price = 359.49m,
                    Stock = 15,
                    CategoryId = 5
                },
                new Product
                {
                    Name = "Skrzynia biegów manualna 6-biegowa",
                    Description = "Regenerowana skrzynia biegów do Opla Astry.",
                    Ean = "4455667788990",
                    Price = 2999.00m,
                    Stock = 3,
                    CategoryId = 6
                },
                new Product
                {
                    Name = "Zderzak przedni",
                    Description = "Nowy zderzak do Ford Focus MK3.",
                    Ean = "5566778899001",
                    Price = 499.00m,
                    Stock = 7,
                    CategoryId = 7
                },
                new Product
                {
                    Name = "Sterownik silnika ECU",
                    Description = "Oryginalny sterownik do silnika BMW 3 E90.",
                    Ean = "6677889900112",
                    Price = 649.99m,
                    Stock = 12,
                    CategoryId = 8
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
