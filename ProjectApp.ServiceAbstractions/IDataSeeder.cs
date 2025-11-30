using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.ServiceAbstractions
{
    public interface IDataSeeder
    {
        SeedResult Seed();
    }
    public sealed class SeedResult
    {
        public Guid pckg1 { get; init; }
        public Guid pckg2 { get; init; }
        public Guid pckg3 { get; init; }
    }
}
