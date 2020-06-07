using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				public class PoppinLocation
				{
								public PoppinLocation() { }

								public PoppinLocation(PoppinLocationDTO v)
								{
												Id = v.Id;
												Name = v.Name;
												YelpId = v.YelpId;
												Phone = v.Phone;
												Address = v.Address;
												Categories = v.Categories.ToList();
												Capacity = v.Capacity;
												CrowdSize = v.CrowdSize;
												Hours = new BusinessSchedule(v.Hours);
								}

								public PoppinLocation(YelpBusiness v)
								{
												YelpId = v.Id;
												Name = v.Name;
												Phone = v.Phone;
												Address = new Address(v.Location, v.Coordinates);
												Categories = v.Categories.Select(c => c.Alias).ToList();
												Hours = new BusinessSchedule(v.Hours.FirstOrDefault());
												YelpDetails = v;
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string YelpId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public Address Address { get; set; }
								public IList<string> Categories { get; set; }
								public int Capacity { get; set; }
								public int CrowdSize { get; set; }
								public BusinessSchedule Hours { get; set; }

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
				}

				public class PoppinLocationDTO
				{
								public PoppinLocationDTO() { }

								public string Id { get; set; }
								public string YelpId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public Address Address { get; set; }
								public string[] Categories { get; set; }
								public int Capacity { get; set; }
								public int CrowdSize { get; set; }
								public BusinessScheduleDTO Hours { get; set; }
				}
}
