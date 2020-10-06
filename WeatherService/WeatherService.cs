using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Net;

namespace WeatherService
{
    public static class WeatherService
    {
        [FunctionName(nameof(WeatherService.GetForecasts))]
        [OpenApiOperation(operationId: "getForecasts",
            tags: new[] { "forecasts" },
            Summary = "Gets the weather forecasts",
            Description = "This method returns an array of weather forecasts.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(WeatherForecast[]),
            Summary = "Weather forecasts",
            Description = "The array of weather forecasts")]
        public static async Task<IActionResult> GetForecasts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var rng = new Random();
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return await Task.FromResult(
                new OkObjectResult(forecasts)).ConfigureAwait(false);
        }

        [FunctionName(nameof(WeatherService.CreateForecast))]
        [OpenApiOperation(operationId: "createForecast",
            tags: new[] { "forecasts" },
            Summary = "Creates a new weather forecast",
            Description = "This creates a new weather forecast.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json",
            bodyType: typeof(WeatherForecast),
            Required = true,
            Description = "Weather forecast that needs to be created")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(WeatherForecast),
            Summary = "Weather forecasts",
            Description = "The new weather forecast that was just sent as a request.")]
        public static async Task<IActionResult> CreateForecast(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var requestedForecast = JsonConvert.DeserializeObject<WeatherForecast>(body);
            return await Task.FromResult(
                new OkObjectResult(requestedForecast)).ConfigureAwait(false);
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    }
}
