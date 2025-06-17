using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarPartsEShop.Models;
using CarPartsEShop.Repositories;

namespace CarPartsEShop.Seeders.Seeders
{
    public class CategorySeeder : ISeeder
    {
        public async Task Seed(ApplicationDbContext context)
        {
            if (context.Categories.Any()) return;

            context.Categories.AddRange(
                new Category { Name = "Hamulce" },
                new Category { Name = "Silniki" },
                new Category { Name = "Zawieszenie" },
                new Category { Name = "Oświetlenie" },
                new Category { Name = "Układy wydechowe" },
                new Category { Name = "Skrzynie biegów" },
                new Category { Name = "Karoseria" },
                new Category { Name = "Elektronika" }
            );

            await context.SaveChangesAsync();
        }
    }
}
