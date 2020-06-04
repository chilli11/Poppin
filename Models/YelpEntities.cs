using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Yelp
{
				public class YelpBusiness
				{
								public YelpBusiness() { }

								public YelpBusiness(YelpShortBusiness y)
								{
												Id = y.Id;
												Alias = y.Alias;
												Name = y.Name;
												Phone = y.Phone;
												Location = new YelpLocation(y.Location);
												Coordinates = y.Coordinates;
								}

								public string Id { get; set; }
								public string Alias { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public string ImageUrl { get; set; }
								public bool IsClaimed { get; set; }
								public bool IsClosed { get; set; }
								public string Url { get; set; }
								public string DisplayPhone { get; set; }
								public int ReviewCount { get; set; }
								public IList<YelpCategory> Categories { get; set; }
								public double Rating { get; set; }
								public YelpLocation Location { get; set; }
								public Coord Coordinates { get; set; }
								public IList<string> Photos { get; set; }
								public string Price { get; set; }
								public IList<string> Transactions { get; set; }
								public YelpHourSet Hours { get; set; }
				}

				public class YelpShortBusiness
				{
								public string Id { get; set; }
								public string Alias { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public YelpShortLocation Location { get; set; }
								public Coord Coordinates { get; set; }

				}

				public class YelpLocation : YelpShortLocation
				{
								public YelpLocation() { }

								public YelpLocation(YelpShortLocation y)
								{
												Address1 = y.Address1;
												Address2 = y.Address2;
												Address3 = y.Address3;
												City = y.City;
												ZipCode = y.ZipCode;
												Country = y.Country;
												State = y.State;
								}

								public IList<string> DisplayAddress { get; set; }
								public string CrossStreets { get; set; }
				}
				
				public class YelpShortLocation
				{
								public string Address1 { get; set; }
								public string Address2 { get; set; }
								public string Address3 { get; set; }
								public string City { get; set; }
								public string ZipCode { get; set; }
								public string Country { get; set; }
								public string State { get; set; }
				}

				public class YelpCategory
				{
								public string Alias { get; set; }
								public string Title { get; set; }
								public string[] ParentAliases { get; set; }
								public string[] CountryWhitelist { get; set; }
								public string[] CountryBlacklist { get; set; }
				}

				public class YelpSpecialHourSet : YelpHourSet
				{
								 public string Date { get; set; }
				}

				public class YelpHourSet
				{
								public List<YelpHourEntity> Open { get; set; }
								public string HoursType { get; set; }
								public bool IsOpenNow { get; set; }
				}

				public class YelpHourEntity
				{
								public bool IsOvernight { get; set; }
								public string Start { get; set; }
								public string End { get; set; }
								public int Day { get; set; }
				}
	}
