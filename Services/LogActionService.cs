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

								public async void LogUserAction(string userId, string actionType, Dictionary<string, string> logAction)
								{
												var action = new LogEntry()
												{
																ActionType = actionType,
																Action = logAction
												};

												if (userId == string.Empty)
												{
																userId = "anon";
												}

												var log = _userLogs.Find(l => l.UserId == userId && l.Date == DateTime.Today).FirstOrDefault();
												if (log == null)
												{
																var entries = new List<LogEntry>();
																entries.Add(action);
																log = new UserLog()
																{
																				UserId = userId,
																				Entries = entries
																};
																await _userLogs.InsertOneAsync(log);
												}
												else
												{
																log.Entries.Add(action);
																await _userLogs.ReplaceOneAsync(l => l.Id == log.Id, log);
												}

								}

								public Task<List<UserLog>> GetUserActivity(string userId) =>
												_userLogs.FindAsync(ul => ul.UserId == userId).Result.ToListAsync();

								public List<UserLog> GetUserActivity(string userId, DateTime day)
								{
												var logs = _userLogs.Find(ul => ul.UserId == userId).ToList();
												return logs.Where(l => l.Date >= day).ToList();
								}
				}
}
