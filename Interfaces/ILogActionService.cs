using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface ILogActionService
				{
								public void LogUserAction(string userId, int actionType, LogAction logAction);
								public Task<List<UserLog>> GetUserActivity(string userId);
				}
}
