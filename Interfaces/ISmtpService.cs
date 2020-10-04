using Microsoft.AspNetCore.Identity;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface ISmtpService
				{
								public void SendConfirmationEmail(User user, string token);
								public void SendPasswordResetEmail(User user, string token);
				}
}
