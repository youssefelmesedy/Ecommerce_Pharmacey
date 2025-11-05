using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Pharmacy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [Authorize]
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            return Ok(new
            {
                User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value
            });
        }


        [HttpGet("long-operation")]
        public async Task<IActionResult> LongOperation(CancellationToken cancellationToken)
        {
            Console.WriteLine("🟢 Started Program");

            var items = Enumerable.Range(1, 10).ToList();
            var finishedSteps = new List<int>();
            int currentStep = 0;

            try
            {
                foreach (var step in items)
                {
                    currentStep = step; // نحفظ الخطوة الحالية قبل الـ delay

                    await Task.Delay(1000, cancellationToken);

                    finishedSteps.Add(step);
                    Console.WriteLine($"⏳ Step num: {step}");
                }

                Console.WriteLine("✅ Successfully Completed Program");
                return Ok($"Finished steps: {string.Join(", ", finishedSteps)}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("🛑 Canceled Program");
                return StatusCode(499, $"Canceled by user at step: {currentStep}. Finished steps: {string.Join(", ", finishedSteps)}");
            }
        }

    }
}
