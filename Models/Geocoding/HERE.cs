using Newtonsoft.Json;
using System.Collections.Generic;

namespace Poppin.Models.Geocoding.HERE
{
				public class Place
				{
								public Place() { }

								public string ResultType { get; set; }
								public string Title { get; set; }
								public string Id { get; set; }
								public Address Address { get; set; }
								public Category[] Categories { get; set; }
								public Category[] FoodTypes { get; set; }
								public Dictionary<string, double> Position { get; set; }
								public Dictionary<string, double>[] Access { get; set; }
								public Dictionary<string,Dictionary<string, string>[]> Contacts { get; set; }
								public OpeningHours OpeningHours { get; set; }
				}

				public class OpeningHours
				{
								public string[] Text { get; set; }
								public bool IsOpen { get; set; }
								public Dictionary<string,string>[] Structured { get; set; }
				}

				public class Geocode
				{
								public Geocode() { }

								public string ResultType { get; set; }
								public string Title { get; set; }
								public string Id { get; set; }
								public Address Address { get; set; }
								public Dictionary<string, double> Position { get; set; }
								public Dictionary<string, double>[] Access { get; set; }
								public Dictionary<string, double> MapView { get; set; }
								public object Scoring { get; set; }
				}

				public class Scoring
				{
								public double QueryScore { get; set; }
								public FieldScore FieldScore { get; set; }
								public double HouseNumber { get; set; }
								public double PostalCode { get; set; }
				}

				public class FieldScore
				{
								public double Country { get; set; }
								public double City { get; set; }
								public double[] Streets { get; set; }
				}

				public class Address
				{
								public Address() { }

								[JsonProperty("label")]
								public string Full { get; set; }
								public string HouseNumber { get; set; }
								public string Street { get; set; }
								public string District { get; set; }
								public string City { get; set; }
								[JsonProperty("postalCode")]
								public string ZipCode { get; set; }
								public string County { get; set; }
								public string State { get; set; }
								public string StateCode { get; set; }

								public string Line1
								{
												get
												{
																return HouseNumber + " " + Street;
												}
								}
				}

				public class Category
				{
								public Category() { }

								public string Id { get; set; }
								public string Name { get; set; }
								public bool Primary { get; set; }
				}

				public class Places
				{
								public Place[] Items { get; set; }
				}

				public class Geocodes
				{
								public Geocode[] Items { get; set; }
				}
}
