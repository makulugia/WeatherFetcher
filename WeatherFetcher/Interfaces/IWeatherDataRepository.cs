namespace WeatherFetcher.Interfaces
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherData(WeatherData weatherData);
    }
}
