using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Yelp
{

				public class YelpBusinessSearchResponse
				{
								public int Total { get; set; }
								public List<YelpBusiness> Businesses { get; set; }
								public YelpRegion Region { get; set; }
								public IYelpSearchParams SearchParams { get; set; }
				}

				public class YelpRegion
				{
								public Coord Center { get; set; }
				}

				public class YelpBusinessSearchParams : IYelpSearchParams
				{
								public YelpBusinessSearchParams() { }

								public YelpBusinessSearchParams(PoppinLocation l)
								{
												term = l.Name;
												location = l.Address.Line1 + ", " + l.Address.City;
												categories = string.Join(',', l.Categories);
												latitude = l.Address.Coordinates.Latitude.ToString();
												longitude = l.Address.Coordinates.Longitude.ToString();
								}

								public string term { get; set; }
								public string location { get; set; }
								public string latitude { get; set; }
								public string longitude { get; set; }
								public string radius { get; set; }
								public string categories { get; set; }
								public string locale { get; set; }
								public string limit { get; set; }
								public string offset { get; set; }
								public string sort_by { get; set; }
								public string price { get; set; }
								public string open_now { get; set; }
								public string open_at { get; set; }
								public string attributes { get; set; }
				}

				public class YelpPhoneSearchParams : IYelpSearchParams
				{
								public string phone { get; set; }
								public string locale { get; set; }
				}

				public class YelpTransactionParams : IYelpSearchParams
				{
								public string location { get; set; }
								public string latitude { get; set; }
								public string longitude { get; set; }
				}

				public class YelpBusinessMatchParams : IYelpSearchParams
				{
								public YelpBusinessMatchParams() { }
								public YelpBusinessMatchParams(PoppinLocation l)
								{
												name = l.Name;
												address1 = l.Address.Line1;
												address2 = l.Address.Line2;
												city = l.Address.City;
												state = l.Address.State;
												zip_code = l.Address.ZipCode.ToString();
												yelp_business_id = l.YelpId;
												phone = l.Phone;
												country = "US";

												if (l.Address.Coordinates != null)
												{
																latitude = l.Address.Coordinates.Latitude.ToString();
																longitude = l.Address.Coordinates.Longitude.ToString();
												}
												else
												{
																latitude = null;
																longitude = null;
												}
								}

								public string name { get; set; }
								public string address1 { get; set; }
								public string address2 { get; set; }
								public string address3 { get; set; }
								public string city { get; set; }
								public string state { get; set; }
								public string country { get; set; }
								public string latitude { get; set; }
								public string longitude { get; set; }
								public string phone { get; set; }
								public string zip_code { get; set; }
								public string yelp_business_id { get; set; }
								public string match_threshold { get; set; }
				}

				public interface IYelpSearchParams
				{

				}
}
