﻿namespace WeatherFetcher.Interfaces
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherData(WeatherRecordList weatherFinal);
    }
}
