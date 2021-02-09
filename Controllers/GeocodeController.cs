using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests.HERE;
using Poppin.Contracts.Responses;
using Poppin.Interfaces;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeocodeController : ControllerBase
    {
        private readonly IHEREGeocoder _hereGeocoder;

        public GeocodeController(IHEREGeocoder hereGeocoder)
        {
            _hereGeocoder = hereGeocoder;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string q)
        {
            try
            {
                var place = await _hereGeocoder.Geocode(new Dictionary<string, string> { { "q", q } });
                return Ok(new { Place = place });
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(FullGeocodeRequest req)
        {
            try
            {
                var place = await _hereGeocoder.Geocode(new Dictionary<string, string>
                {
                    { "q", req.Q },
                    { "qq",  req.QQ.ToString() }
                });

                return Ok(new { Place = place });
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}
