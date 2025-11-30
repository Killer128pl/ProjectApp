using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly IPackageService _packageService;

        public DataSeeder(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public SeedResult Seed()
        {
            var p2 = Guid.Parse("2137");

            var paczka1 = _packageService.CreatePackage(Guid.NewGuid(), new DateTime(2025, 5, 5), 2.5f, "Big");
            var paczka2 = _packageService.CreatePackage(p2, new DateTime(2024, 7, 12), 2.5f, "Small");
            var paczka3 = _packageService.CreatePackage(Guid.NewGuid(), new DateTime(2023, 2, 9), 2.5f, "Very Big");

            return new SeedResult
            {
                pckg1 = paczka1,
                pckg2 = paczka2,
                pckg3 = paczka3
            };

        }
    }
}
