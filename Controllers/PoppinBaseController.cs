using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Tracking;
using Segment;

namespace Poppin.Controllers
{
				[Route("api/[controller]")]
				[ApiController]
				public class PoppinBaseController : ControllerBase
    {
        protected IIdentityService _identityService;
        protected IUserService _userService;
        protected IYelpService _yelpService;
        protected ILocationService _locationService;
        protected IVendorService _vendorService;
        protected ILogActionService _logActionService;

        protected string GetUserId()
        {
            if (HttpContext.User.Claims.Any())
            {
                var id = HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
                _identityService.Identify(id, SegmentIOKeys.Categories.Identity, "GetUserId");
                return id;
            }
            return string.Empty;
        }

        protected string GetUserId(string action)
        {
            if (HttpContext.User.Claims.Any())
            {
                var id = HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
                _identityService.Identify(id, SegmentIOKeys.Categories.Identity, action);
                return id;
            }
            return string.Empty;
        }

        protected string GetUserRole()
        {
            if (HttpContext.User.Claims.Any())
            {
                return HttpContext.User.Claims.Single(u => u.Type == "Role").Value;
            }
            return string.Empty;
        }

        protected async Task<PoppinUser> GetUserProfile(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                var u = await _identityService.GetUserById(id);
                if (u.User != null)
                {
                    user = new PoppinUser(u.User);
                    _userService.AddUser(user);
                }
            }
            return user;
        }

        protected async Task<PoppinUser> GetUserProfileByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                var u = await _identityService.GetUserByEmail(email);
                if (u != null)
                {
                    user = new PoppinUser(u.User);
                    _userService.AddUser(user);
                }
            }
            return user;
        }

        protected void Track(string id, string search)
        {
            var trackingId = string.IsNullOrEmpty(id) ? SegmentIOKeys.AnonId : id;
            Analytics.Client.Track(trackingId, search);
        }

        protected void Track(string id, string search, IDictionary<string, object> properties)
        {
            var trackingId = string.IsNullOrEmpty(id) ? SegmentIOKeys.AnonId : id;
            Analytics.Client.Track(trackingId, search, properties);
        }
    }
}
