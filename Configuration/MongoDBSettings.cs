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
        private IMongoCollection<Category> _categories;
        private IMongoCollection<UserLocationRequest> _userLocationRequests;

        public ConfigureMongoDbIndexesService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _vendors = database.GetCollection<Vendor>("Vendors");
            _poppinUsers = database.GetCollection<PoppinUser>("PoppinUsers");
            _locations = database.GetCollection<PoppinLocation>("Locations");
            _checkins = database.GetCollection<Checkin>("Checkins");
            _userLogs = database.GetCollection<UserLog>("UserLogs");
            _categories = database.GetCollection<Category>("Categories");
            _userLocationRequests = database.GetCollection<UserLocationRequest>("UserLocationRequests");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var unique = new CreateIndexOptions()
            {
                Unique = true
            };

            var lNameIndex = new CreateIndexModel<PoppinLocation>(Builders<PoppinLocation>.IndexKeys.Text(l => l.Name));
            var lSpatialIndex = new CreateIndexModel<PoppinLocation>(Builders<PoppinLocation>.IndexKeys.Geo2DSphere(l => l.Address.Geo));
            var lCatsIndex = new CreateIndexModel<PoppinLocation>(Builders<PoppinLocation>.IndexKeys.Ascending(l => l.Categories));
            var lSpatialCatsIndex = new CreateIndexModel<PoppinLocation>(Builders<PoppinLocation>.IndexKeys.Combine(lSpatialIndex.Keys, lCatsIndex.Keys));
            _locations.Indexes.CreateManyAsync(new[] {
                lNameIndex, lSpatialCatsIndex
            });

            var vOrgNameIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Text(c => c.OrganizationName));
            var vOrgEmailIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Ascending(c => c.OrganizationContactEmail));
            var vOrgCombinedIndex = new CreateIndexModel<Vendor>(Builders<Vendor>.IndexKeys.Combine(vOrgNameIndex.Keys, vOrgEmailIndex.Keys), unique);
            _vendors.Indexes.CreateManyAsync(new[] {
                vOrgEmailIndex, vOrgCombinedIndex
            });

            var cUserIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Ascending(c => c.UserId));
            var cLocationIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Ascending(c => c.LocationId));
            var cTimeoutIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Descending(c => c.Timeout));
            var cUserTimeoutIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Combine(cUserIndex.Keys, cTimeoutIndex.Keys));
            var cLocTimeoutIndex = new CreateIndexModel<Checkin>(Builders<Checkin>.IndexKeys.Combine(cLocationIndex.Keys, cTimeoutIndex.Keys));
            _checkins.Indexes.CreateManyAsync(new[] {
                cUserTimeoutIndex, cLocTimeoutIndex
            });

            var catIndex = new CreateIndexModel<Category>(Builders<Category>.IndexKeys.Ascending(c => c.Slug), unique);
            _categories.Indexes.CreateOneAsync(catIndex);

            var uUserIdIndex = new CreateIndexModel<PoppinUser>(Builders<PoppinUser>.IndexKeys.Ascending(u => u.UserId), unique);
            var uEmailIndex = new CreateIndexModel<PoppinUser>(Builders<PoppinUser>.IndexKeys.Ascending(u => u.Email), unique);
            _poppinUsers.Indexes.CreateManyAsync(new[] {
                uUserIdIndex, uEmailIndex
            });

            var ulSpatialIndex = new CreateIndexModel<UserLocationRequest>(Builders<UserLocationRequest>.IndexKeys.Geo2DSphere(l => l.LocationGeo));
            _userLocationRequests.Indexes.CreateOneAsync(ulSpatialIndex);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

}
