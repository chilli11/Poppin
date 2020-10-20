﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Geocoding.HERE;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
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
												Categories = v.Categories.ToHashSet();
												Hours = v.Hours;
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

								public PoppinLocation(Place p)
								{
												Name = p.Title;
												Address = new Address(p.Address, p.Position);
												HereId = p.Id;
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string YelpId { get; set; }
								public string HereId { get; set; }
								public string VendorId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public Address Address { get; set; }
								public HashSet<string> Categories { get; set; }
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

				public class PoppinLocComparer : IEqualityComparer<PoppinLocation>
				{
								public bool Equals(PoppinLocation loc1, PoppinLocation loc2)
								{
												if (loc1 == null && loc2 == null) { return true; }
												if (loc1 == null | loc2 == null) { return false; }
												if (loc1.Id == loc2.Id) { return true; }
												return false;
								}
								public int GetHashCode(PoppinLocation l)
								{
												string code = l.Id;
												return code.GetHashCode();
								}
				}
}
