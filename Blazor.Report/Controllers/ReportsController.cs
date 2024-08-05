using AspNetCore.Reporting;
using Blazor.Report.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Report.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public ReportsController(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [Route("GenerateReport/{fileName}")]
        public IActionResult GenerateReport(string fileName)
        {
            // Get report file name from wwwroot
            var reportFile = Path.Combine(hostEnvironment.WebRootPath, "Reports",
                "WeatherReport.rdlc");

            // Initialize local report
            LocalReport report = new LocalReport(reportFile);

            // Get weather forcast list
            var weathers = GetWeathers();

            // add local report data source
            report.AddDataSource("WeatherDataSet", weathers);

            // if user select pdf file
            if (fileName.ToLower() == "pdf")
            {
                var result = report.Execute(RenderType.Pdf);
                return File(result.MainStream, "application/pdf");
            }
            // if user select word file
            else if (fileName.ToLower() == "word")
            {
                var result = report.Execute(RenderType.Word);
                return File(result.MainStream, "application/msword");
            }

            // no selection here
            return NotFound();
        }

        private IEnumerable<WeatherModel> GetWeathers()
        {
            // Get default weather forcast list
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecasts = Enumerable.Range(1, 50).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            });

            // Convert list to new model <WeatherModel>
            var models = new List<WeatherModel>();
            foreach (var item in forecasts)
            {
                models.Add(new WeatherModel
                {
                    Date = item.Date.ToString(),
                    TemperatureC = item.TemperatureC,
                    Summary = item.Summary,
                });
            }
            return models;
        }
    }
}
