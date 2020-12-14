using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
	public class UserRegistrationRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public string Password { get; set; }
		public string Password2 { get; set; }
	}
	public class UserLoginRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class ResetPasswordRequest
	{
		[Required]
		public string Token { get; set; }
		public string Password { get; set; }
		public string Password2 { get; set; }
	}
}
