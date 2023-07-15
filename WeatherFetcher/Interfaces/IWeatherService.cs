namespace WeatherFetcher.Interfaces
{
    public interface IWeatherService
    {
       Task<WeatherRecordList> FetchWeatherData(string city);
    }
}
