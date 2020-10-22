using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				[BsonIgnoreExtraElements]
				public class PoppinLocation
				{

								public PoppinLocation() { }

								public PoppinLocation(PoppinLocationRequest v)
								{
												Id = v.Id;
												Name = v.Name;
												YelpId = v.YelpId;
												Phone = v.Phone;
												Address = new Address(v.Address);
												Capacity = v.Capacity;
												Hours = v.Hours;
												VisitLength = v.VisitLength;
								}

								public PoppinLocation(YelpBusiness v)
								{
												YelpId = v.Id;
												Name = v.Name;
												Phone = v.Phone;
												Address = new Address(v.Location, v.Coordinates);
												Hours = v.Hours.FirstOrDefault().Open.Select(h => new HourSet(h)).ToList();
												YelpDetails = v;
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string YelpId { get; set; }

								public string VendorId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public Address Address { get; set; }
								public int Capacity { get; set; }
								public int CrowdSize { get; set; }

								/// <summary>
								/// VisitLength is in minutes
								/// </summary>
								public int VisitLength { get; set; }
								public string MenuUrl { get; set; }
								public IList<HourSet> Hours { get; set; }

								[BsonDateTimeOptions]
								public DateTime LastUpdate { get; set; }

								public bool AtCapacity
								{
												get
												{
																return CrowdSize >= Capacity;
												}
								}

								public YelpBusiness YelpDetails { get; set; }

								public async Task<Vendor> GetVendor(IVendorService vs)
								{
												return await vs.GetVendorById(VendorId);
								}

								public async Task SetCrowdSize(ILocationService _locationService)
								{
												var s = await _locationService.GetCheckinsForLocation(Id);
												CrowdSize = (int)Math.Round(s.Select(c => c.ReliabilityScore).Sum());
												return;
								}
				}
}
