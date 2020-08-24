using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class LogActionService : ILogActionService
				{
								private readonly IMongoCollection<UserLog> _userLogs;

								public LogActionService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_userLogs = database.GetCollection<UserLog>("UserLogs");
								}

								public void LogUserAction(string userId, int actionType, LogAction logAction)
								{
												var action = new LogEntry()
												{
																ActionType = actionType,
																Action = logAction
												};

												var log = _userLogs.Find(l => l.UserId == userId).FirstOrDefault();
												if (log == null)
												{
																log = new UserLog()
																{
																				UserId = userId,
																				Entries = new List<LogEntry>()
																};
												}

												log.Entries.Append(action);
								}
				}
}
