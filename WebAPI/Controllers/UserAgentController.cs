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
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return BadRequest("User agent could not be determined.");
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                return BadRequest("IP could not be determined.");
            }

            var dd = new DeviceDetector(userAgent);
            dd.Parse();

            var device = OrUnknown(dd.GetDeviceName(), "device");
            var brand = OrUnknown(dd.GetBrandName(), "brand");
            var model = OrUnknown(dd.GetModel(), "model");

            var osMatch = dd.GetOs()?.Match;
            var osName = OrUnknown(osMatch?.Name, "os");
            var osVersion = OrUnknown(osMatch?.Version, "os-version");
            var osPlatform = OrUnknown(osMatch?.Platform, "os-platform");

            var clientMatch = dd.GetClient()?.Match;
            var clientName = OrUnknown(clientMatch?.Name, "client");
            var clientType = OrUnknown(clientMatch?.Type, "client-type");
            var clientVersion = OrUnknown(clientMatch?.Version, "client-version");

            ip = OrUnknown(ip, "ip-address");

            return Ok($"" +
                $"Device: {device} {brand} {model}\n" +
                $"OS: {osName} {osVersion} {osPlatform}\n" +
                $"Client: {clientName} {clientType} {clientVersion}\n" +
                $"IP: {ip}");
        }

        private string OrUnknown(string s, string specifier)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return $"unknown-{specifier}";
            }

            return s;
        }
    }
}
