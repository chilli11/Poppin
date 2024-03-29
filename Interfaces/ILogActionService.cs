﻿using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
	public interface ILogActionService
	{
		public void LogUserAction(string userId, string actionType);
		public void LogUserAction(string userId, string actionType, IDictionary<string, string> logAction);
		public Task<List<UserLog>> GetUserActivity(string userId);
		public List<UserLog> GetUserActivity(string userId, DateTime day);
	}
}
