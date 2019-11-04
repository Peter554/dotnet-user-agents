using Microsoft.AspNetCore.Mvc;
using DeviceDetectorNET;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAgentController : ControllerBase
    {
        public IActionResult Get()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();

            // TODO If null

            var dd = new DeviceDetector(userAgent);

            dd.Parse();

            var client = dd.GetClient()?.Match;
            var os = dd.GetOs()?.Match;
            var deviceName = dd.GetDeviceName();
            var brandName = dd.GetBrandName();
            var modelName = dd.GetModel();

            // return Ok(client);
            return Ok(os);
            // return Ok($"{deviceName} {brandName} {modelName}");
        }
    }
}
