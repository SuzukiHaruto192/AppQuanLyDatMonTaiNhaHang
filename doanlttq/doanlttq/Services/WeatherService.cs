using Azure;
using doanlttq.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.Services
{

    public class WeatherService
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly string _apiKey = ConfigurationManager.AppSettings["OpenWeather_ApiKey"];
        private readonly string _baseUrl = ConfigurationManager.AppSettings["OpenWeather_BaseUrl"];
        private readonly string _lat = ConfigurationManager.AppSettings["Restaurant_Latitude"];
        private readonly string _lon = ConfigurationManager.AppSettings["Restaurant_Longitude"];

        public async Task<string> FetchRawWeatherDataAsync()
        {
            // 1. Xây dựng URL
            string requestUrl =
                $"{_baseUrl}?lat={_lat}&lon={_lon}&units=metric&appid={_apiKey}";

            // 2. Gọi API và xử lý lỗi...
            // ... (phần code được hướng dẫn ở các bước trước)
            try
            {
                // 2. Thực hiện yêu cầu HTTP GET
                HttpResponseMessage response = await _client.GetAsync(requestUrl);

                // 3. Kiểm tra mã lỗi (ví dụ: 404, 500)
                if (response.IsSuccessStatusCode)
                {
                    // Trả về chuỗi JSON thô
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Xử lý lỗi (API Key sai, lỗi server...)
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                // Xử lý lỗi mạng (mất kết nối Internet)
                Console.WriteLine($"Network error: {ex.Message}");
                return null;
            }
        }

        public string NormalizeWeatherContext(string jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
            {
                return "DEFAULT_CONTEXT";
            }

            var apiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(jsonResponse);
            float temperature = 0;
            if (apiResponse != null)
            {
                temperature = apiResponse.main.temp;
            }

            if (temperature < 20)
                return "Lạnh";
            else if (temperature < 25)
                return "Mát";
            else if (temperature <= 28)
                return "Dễ chịu";
            else if (temperature <= 32)
                return "Nóng";
            else
                return "Rất nóng";

        }
    }
}

