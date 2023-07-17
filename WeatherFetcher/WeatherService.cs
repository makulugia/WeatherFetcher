using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
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
                        long dt = entry.dt;
                        float temp = entry.main.temp;
                        float temp_min = entry.main.temp_min;
                        float temp_max = entry.main.temp_max;

                        var weatherEntry = new WeatherEntry
                        {
                            dt_txt = dtTxt,
                            dt = dt,
                            main = new Main { temp = temp }
                        };

                        var weatherRecord = new WeatherRecord
                        {
                            dt_txt = dtTxt,
                            dt = dt,
                            temp = temp,
                            temp_min = temp_min,
                            temp_max = temp_max
                        };

                        weatherFinal.list.Add(weatherRecord);
                    }

                    //await _weatherDataRepository.SaveWeatherData(weatherFinal); // Save the weather data to the repository
                    await SaveWeatherDataToFile(weatherFinal); // Save the weather data to a text file

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
        private async Task SaveWeatherDataToFile(WeatherRecordList weatherData)
        {
            string filePath = "weatherData.txt";
            StringBuilder csvData = new StringBuilder();

            if (File.Exists(filePath))
            {
                // Read the existing data from the file
                string existingData = await File.ReadAllTextAsync(filePath);
                csvData.AppendLine(existingData);
            }

            // Filter out the weather records with dt values that are already present in the file
            var newWeatherData = weatherData.list.Where(record =>
                !csvData.ToString().Contains(record.dt.ToString())
            );

            int anyNew = 0;

            foreach (var weatherRecord in newWeatherData)
            {
                string csvLine = $"{weatherRecord.dt_txt},{weatherRecord.dt},{weatherRecord.temp},{weatherRecord.temp_min},{weatherRecord.temp_max}";
                csvData.AppendLine(csvLine);
                anyNew += 1;
            }

            Console.WriteLine($"{anyNew} new entrie(s) added!");

            await File.WriteAllTextAsync(filePath, csvData.ToString());
        }

    }
}
