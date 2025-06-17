using CarPartsEShop.Seeders.Seeders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarPartsEShop.Repositories;

namespace CarPartsEShop.Seeders.DbSeeder
{
    public class DbSeeder
    {
        public async Task Seed(ApplicationDbContext context)
        {
            var seeders = new List<ISeeder>
            {
                new CategorySeeder(),
                new ProductSeeder(),
                new CustomerSeeder(),
            };

            foreach (var seeder in seeders)
            {
                await seeder.Seed(context);
            }

            context.SaveChanges();

        }
    }
}
