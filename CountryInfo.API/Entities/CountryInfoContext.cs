
using Microsoft.EntityFrameworkCore;

namespace CountryInfo.API.Entities
{
    public class CountryInfoContext : DbContext
    {
        public CountryInfoContext(DbContextOptions<CountryInfoContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<AreaPostalCode> AreaPostalCodes { get; set; }
    }
}
