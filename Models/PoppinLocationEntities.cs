using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				public class Address
				{
								public Address() { }

								public Address(YelpLocation v, Coord c)
								{
												Line1 = v.Address1;
												Line2 = v.Address2;
												City = v.City;
												State = v.State;

												var spl = v.ZipCode.Split('-');
												ZipCode = int.Parse(spl[0]);

												if (!string.IsNullOrEmpty(spl[1]))
												{
																ZipCodeTrailing = int.Parse(spl[1]);
												}

												Coordinates = c;
								}

								public string Line1 { get; set; }
								public string Line2 { get; set; }
								public string City { get; set; }
								public string State { get; set; }
								public int ZipCode { get; set; }
								public int? ZipCodeTrailing { get; set; }
								public Coord Coordinates { get; set; }
				}

				public class PoppinInsertActionResponse
				{
								public PoppinLocation location { get; set; }
								public YelpBusinessSearchResponse yelpMatches { get; set; }
				}
}
