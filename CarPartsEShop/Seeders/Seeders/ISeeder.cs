using CarPartsEShop.Repositories;

namespace CarPartsEShop.Seeders.Seeders
{
    public interface ISeeder
    {
        Task Seed(ApplicationDbContext context);
    }
}
