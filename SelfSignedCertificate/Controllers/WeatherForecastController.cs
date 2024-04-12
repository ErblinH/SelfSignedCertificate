using Microsoft.AspNetCore.Mvc;

namespace SelfSignedCertificate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public void Get()
        {

        }
    }
}
