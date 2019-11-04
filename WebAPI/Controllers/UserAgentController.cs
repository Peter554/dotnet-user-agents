using Microsoft.AspNetCore.Mvc;
using DeviceDetectorNET;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAgentController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public UserAgentController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Get()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return BadRequest("User agent could not be determined.");
            }

            var dd = new DeviceDetector(userAgent);
            dd.Parse();

            var device = OrUnknown(dd.GetDeviceName(), "device");
            var brand = OrUnknown(dd.GetBrandName(), "brand");
            var model = OrUnknown(dd.GetModel(), "model");

            var osMatch = dd.GetOs()?.Match;
            var osName = OrUnknown(osMatch?.Name, "os");
            var osPlatform = OrUnknown(osMatch?.Platform, "os-platform");
            var osVersion = OrUnknown(osMatch?.Version, "os-version");

            var clientMatch = dd.GetClient()?.Match;
            var clientName = OrUnknown(clientMatch?.Name, "client");
            var clientType = OrUnknown(clientMatch?.Type, "client-type");
            var clientVersion = OrUnknown(clientMatch?.Version, "client-version");

            // var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString(); // This didn't work
            var ip = HttpContext.Request.Headers["X -Forwarded-For"].ToString();

            var city = string.Empty;
            if (!string.IsNullOrWhiteSpace(ip))
            {
                var apiKey = Environment.GetEnvironmentVariable("IPREGISTRY_KEY");
                var url = $"https://api.ipregistry.co/{ip}?key={apiKey}";
                var client = _clientFactory.CreateClient();
                var reponse = await client.GetAsync(url);
                var ipReponse = JsonConvert.DeserializeObject<IPResponse>(await reponse.Content.ReadAsStringAsync());
                city = ipReponse?.location?.city;
            }

            ip = OrUnknown(ip, "ip-address");
            city = OrUnknown(city, "city");

            return Ok($"" +
                $"Device: {device} {brand} {model}\n" +
                $"OS: {osName} {osPlatform} {osVersion}\n" +
                $"Client: {clientName} {clientType} {clientVersion}\n" +
                $"IP: {ip}, near {city}");
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

    internal class IPResponse
    {
        public IPReponseLocation location { get; set; }
    }

    internal class IPReponseLocation
    {
        public string city { get; set; }
    }
}
