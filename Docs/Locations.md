# Locations
Reference for `api/locations` endpoints

NATIVE SEARCH: No more yelp search. A number of fields have been added to request and response  
classes in order to support full location details from our own database.

## Request/Response Classes

#### PoppinLocation Class
```
{
  id: string,
  yelpId: string,
  hereId: string, :: not yet in use
  vendorId: string,
  name: string,
  phone: string,
  logoUrl: string,
  mainPhotoUrl: string,
  addlPhotoUrls: string[],
  website: string,
  menus: [Menu](#menu-class)[], 
  yelpUrl: string,
  address: [Address](#address-class),
  categories: string[],
  capacity: int,
  crowdsize: int,
  visitLength: int,
  hours: [{
    opening: string, // 11:00
    closing: string, // 22:00
    day: string // "Monday"
  }],
  lastUpdate: Date,
  atCapacity: bool
  yelpDetails: YelpBusiness or YelpShortBusiness // available in limited scenarios
}
```

#### PoppinLocationRequest Class
```
{
  id: string, // validated as 24-digit alphanumeric
  yelpId: string,
  name: string,
  phone: string,
  mainPhotoUrl: string,
  addlPhotoUrls: string[],
  website: string,
  menus: [Menu](#menu-class)[],
  yelpUrl: string,
  address: [Address](#address-class),
  categories: string[],
  capacity: int,
  visitLength: int,
  hours: [{
  		opening: string, // 11:00
  		closing: string, // 22:00
  		day: string // "Monday"
  }],
}
```

#### LocationSearchRequest Class
```
{
  term: string,
  location: string, // required if no geo,
  geo: [GeoJsonPoint](https://docs.mongodb.com/manual/reference/geojson/#point),
  radius: string, // float, in meters
	offset: int, // for pagination, 0 is first page // NEW
	pageLength: int, // default 20 // NEW
  categories: string // comma separated list of slugs from the category list
}
```

#### PoppinSearchResponse Class
```
{
	total: int, // total locations in search
	offset: int, // page offset; number of locations, not page number // NEW
	pageLength: int, // number of items to return // NEW
	businesses: PoppinLocation[],
	searchParams: LocationSearchRequest
}
```

#### Address Class
```
{
  line1: string,
  line2: string,
  city: string,
  state: string,
  zipCode: string,
  coordinates: {
    latitude: float,
    longitude: float
  }
  geo: GeoJsonPoint
  }
```

#### Menu Class
```
{
  name: string,
  url: string
}
```

#### Checkin Class
```
{
  id: string,
  userId: string,
  locationId: string,
  timestamp: date,
  timeout: date,
  reliabilityScore: float,
  isValid: bool // if the timeout is in the future
}
```

## API Reference

### GET api/locations/{locationId}
Return: [PoppinLocation](#poppinlocation-class) without `yelpDetails`

### PUT api/locations or PUT api/locations/{locationId}
*Requires Admin role*  
Request: [PoppinLocationRequest](#poppinlocationrequest-class)  
Return: updated [PoppinLocation](#poppinlocation-class) without Yelp Details

Updates existing [PoppinLocation](#poppinlocation-class)

### DELETE api/locations/{locationId}
*Requires Admin role*  
Request: Empty  
Return: Empty

Deletes existing [PoppinLocation](#poppinlocation-class)

### GET api/locations/with-yelp/{locationId}
Request: Empty  
Return: [PoppinLocation](#poppinlocation-class)

Due to the daily request limit on Yelp's Fusion API (5000), we have to limit the number of requests for their data,
and so it is not included in `api/locations/{locationId}. This API addresses that need.
`api/locations/yelp-search` returns yelp data with each result. 

TODO: Implement GraphAPI to get details as needed. Potentially higher usage limit, and will be a good candidate for 
Redis integration. This will not be deprecated with `api/locations/yelp-search`

### POST api/locations/search
Request: [LocationSearchRequest](#locationsearchrequest-class)  
Response: 
```
{
  total: int,
  businesses: [PoppinLocation](#poppinlocation-class)[],
  searchParams: [LocationSearchRequest](#locationsearchrequest-class)  
}
```

Initiates a native search with the submitted query. Returns all locations in the `PoppinStore.Locations` collection that
are _within the radius_ => _have one of the selected categories (all, if none selected)_ => have the search term in the `name`.  

### POST api/locations
Request: [PoppinLocationRequest](#poppinlocationrequest-class)  
Response: [PoppinLocation](#poppinlocation-class)

## Checkins
Each [Checkin](#checkin-class) has a reliability score attached.  
- User direct checkin: 1.5
- Vendor checkin (increment- or decrement-crowd): 1
- User geogrpahic checkin: .5

`crowdSize` = The sum of the reliability scores for valid checkins (those that haven't
timed out or been invalidated)

### GET api/locations/checkin/{locationId}
Request: Empty  
Response: [PoppinLocation](#poppinlocation-class) with updated `crowdSize`

User direct checkin (score 1.5)

### GET api/locations/geo-checkin/{locationId}
Request: Empty  
Response: [PoppinLocation](#poppinlocation-class) with updated `crowdSize`

User geo checkin (score .5)

### GET api/locations/increment-crowd/{locationId}
Request: Empty  
Repsonse: [PoppinLocation](#poppinlocation-class) with updated `crowdSize

Vendor checkin (score 1)

### GET api/locations/decrement-crowd/{locationId}
Request: Empty  
Repsonse: [PoppinLocation](#poppinlocation-class) with updated `crowdSize

Invalidates *oldest* checkin at the location 
