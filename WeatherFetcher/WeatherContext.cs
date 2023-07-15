using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WeatherFetcher
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherRecordList> WeatherRecordList { get; set; }
    }
}
