using System;
using System.Threading.Tasks;
using WeatherFetcher.Interfaces;

namespace WeatherFetcher
{
    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly WeatherContext _context;

        public WeatherDataRepository(WeatherContext context)
        {
            _context = context;
        }

        public async Task SaveWeatherData(WeatherData weatherData)
        {
            await _context.WeatherData.AddAsync(weatherData);
            await _context.SaveChangesAsync();
        }
    }
}
