using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WeatherFetcher;
using WeatherFetcher.Interfaces;

namespace WeatherFetcher
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherService(IConfiguration configuration, IWeatherDataRepository weatherDataRepository, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _weatherDataRepository = weatherDataRepository;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<WeatherRecordList> FetchWeatherData(string city)
        {
            try
            {
                //var apiKey = Environment.GetEnvironmentVariable("WeatherAPIKey");
                //if (string.IsNullOrEmpty(apiKey))
                //{
                //    // Handle missing API key
                //    return null;
                //}

                //if (string.IsNullOrEmpty(apiKey))
                //{
                //    Console.WriteLine("WeatherAPIKey is not set.");
                //}
                //else
                //{
                //    Console.WriteLine("WeatherAPIKey is set: " + apiKey);
                //}

                var apiKey = _configuration["WeatherAPIKey"]; // Retrieve the API key from configuration
                var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<WeatherData>(json);

                    var weatherFinal = new WeatherRecordList
                    {
                        list = new List<WeatherRecord>()
                    };

                    foreach (var entry in result.list)
                    {
                        string dtTxt = entry.dt_txt;
                        float temp = entry.main.temp;

                        var weatherEntry = new WeatherEntry
                        {
                            dt_txt = dtTxt,
                            main = new Main { temp = temp }
                        };

                        var weatherRecord = new WeatherRecord
                        {
                            dt_txt = dtTxt,
                            temp = temp
                        };

                        weatherFinal.list.Add(weatherRecord);
                    }

                    return weatherFinal;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorMessage = $"Failed to fetch weather data. Status Code: {response.StatusCode}. Response: {errorResponse}";

                    // Handle error response

                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Failed to fetch weather data. {ex.Message}";

                // Handle HTTP request exception

                return null;
            }
            catch (JsonException ex)
            {
                var errorMessage = $"Failed to parse weather data. {ex.Message}";

                // Handle JSON deserialization exception

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while fetching weather data. {ex.Message}";

                // Handle other exceptions

                return null;
            }
        }
    }
}
