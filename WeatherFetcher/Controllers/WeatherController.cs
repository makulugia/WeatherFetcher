using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherFetcher;
using WeatherFetcher.Interfaces;

[ApiController]
[Route("weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherDataRepository _weatherDataRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory httpClientFactory;

    public WeatherController(IWeatherService weatherService, IWeatherDataRepository weatherDataRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _weatherService = weatherService;
        _weatherDataRepository = weatherDataRepository;
        _configuration = configuration;
        this.httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> FetchWeatherData(string city)
    {
        try
        {
            var weather = await _weatherService.FetchWeatherData(city);
            if (weather == null)
            {
                throw new Exception("Failed to fetch or parse weather data.");
            }
            return Ok(weather);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while fetching weather data. {ex.Message}";
            throw new Exception(errorMessage);
        }
    }
}
