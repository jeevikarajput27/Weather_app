using Microsoft.AspNetCore.Mvc;
     using Microsoft.Extensions.Configuration;
     using System.Net.Http;
     using System.Threading.Tasks;
     using WeatherForecastApp.Models;
     using Newtonsoft.Json.Linq;

     namespace WeatherForecastApp.Controllers
     {
         public class WeatherController : Controller
         {
             private readonly IConfiguration _configuration;
             private readonly IHttpClientFactory _httpClientFactory;

             public WeatherController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
             {
                 _configuration = configuration;
                 _httpClientFactory = httpClientFactory;
             }

             public IActionResult Index()
             {
                 return View();
             }

             [HttpPost]
             public async Task<IActionResult> GetWeather(string location)
             {
                 var baseUrl = _configuration["WeatherApi:BaseUrl"];
                 var apiKey = _configuration["WeatherApi:ApiKey"];

                 var client = _httpClientFactory.CreateClient();
                 var response = await client.GetAsync($"{baseUrl}?q={location}&appid={apiKey}&units=metric");

                 if (!response.IsSuccessStatusCode)
                 {
                     ViewBag.Error = "Could not fetch weather data. Please try again.";
                     return View("Index");
                 }

                 var data = await response.Content.ReadAsStringAsync();
                 var weatherData = JObject.Parse(data);

                 var weather = new WeatherModel
                 {
                     Location = weatherData["name"].ToString(),
                     Temperature = weatherData["main"]["temp"].ToString() + " °C",
                     WeatherCondition = weatherData["weather"][0]["description"].ToString(),
                     Icon = $"https://openweathermap.org/img/wn/{weatherData["weather"][0]["icon"]}@2x.png"
                 };

                 return View("WeatherResult", weather);
             }
         }
     }