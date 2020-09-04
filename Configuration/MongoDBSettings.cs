using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
				public class MongoDBSettings : IMongoDBSettings
				{
								public string ConnectionString { get; set; }
								public string DatabaseName { get; set; }
				}
				public interface IMongoDBSettings
				{
								string ConnectionString { get; set; }
								string DatabaseName { get; set; }
				}

				public class ConfigureMongoDbIndexesService : IHostedService
				{
								private IMongoCollection<Vendor> _vendors;
								private IMongoCollection<PoppinUser> _poppinUsers;
								private IMongoCollection<PoppinLocation> _locations;
								private IMongoCollection<Checkin> _checkins;
								private IMongoCollection<UserLog> _userLogs;

								public ConfigureMongoDbIndexesService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_vendors = database.GetCollection<Vendor>("Vendors");
												_poppinUsers = database.GetCollection<PoppinUser>("PoppinUsers");
												_locations = database.GetCollection<PoppinLocation>("Locations");
												_checkins = database.GetCollection<Checkin>("Checkins");
												_userLogs = database.GetCollection<UserLog>("UserLogs");
								}

								public Task StartAsync(CancellationToken cancellationToken)
								{
												var lYelpIndex = new CreateIndexModel<PoppinLocation>(Builders<PoppinLocation>.IndexKeys.Text(l => l.YelpId));
												_locations.Indexes.CreateOneAsync(lYelpIndex);

												var vOrgNameIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Text(c => c.OrganizationName));
												var vOrgEmailIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Text(c => c.OrganizationContactEmail));
												var vOrgContactIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Text(c => c.OrganizationContactName));
												var vIndexes = new List<CreateIndexModel<Vendor>>();
												vIndexes.Add(vOrgNameIndex);
												vIndexes.Add(vOrgEmailIndex);
												vIndexes.Add(vOrgContactIndex);
												_vendors.Indexes.CreateManyAsync(vIndexes);

												var cUserIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Text(c => c.UserId));
												var cTimeoutIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Descending(c => c.Timeout));
												var cIndexes = new List<CreateIndexModel<Checkin>>();
												cIndexes.Add(cUserIndex);
												cIndexes.Add(cTimeoutIndex);

												return _checkins.Indexes.CreateManyAsync(cIndexes);
								}

								public Task StopAsync(CancellationToken cancellationToken)
												=> Task.CompletedTask;
				}

}
