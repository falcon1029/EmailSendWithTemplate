using Microsoft.AspNetCore.Mvc;

namespace EmailSend.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IEmailService _emailService;

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;

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

   [HttpPost("send-normal")]
    public IActionResult SendWelcomeEmail([FromBody] EmailRequestNormal request)
    {
      
        _emailService.SendEmailNormal(request.toEmail,request.Subject,request.Body);

        return Ok("Email Sent");
    }

     [HttpPost("send-template")]
    public IActionResult SendWelcomeEmail([FromBody] EmailRequest request)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "UserName", request.UserName },
           
        };

        _emailService.SendEmailWithTemplate(request.Email, "Welcome!", request.TemplateName, placeholders);

        return Ok("Email Sent");
    }

    public class EmailRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string TemplateName { get; set; }
}

  public class EmailRequestNormal
{
    public string toEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
}
