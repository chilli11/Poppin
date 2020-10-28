# UserProfile
Reference for `api/locations` endpoints

## Request/Response Classes

#### PoppinUser Class
```
{
  id: string,
  userId: string,
  username: string,
  role: string,
  email: string,
  firstName: string,
  lastName: string,
  profilePhoto: string, // URL
  ageRange: string, // see below for codes
  gender: string, // see below for codes
  categories: string[], // aliases to Yelp categories
  favorites: string[], // location IDs
  hidden: string[], // location IDs
  vendorIds: string[]
}
```

#### PoppinUserRequest Class
```
{
  id: string,
  yelpId: string,
  name: string,
  phone: string,
  address: {
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
  },
  capacity: int,
  visitLength: int,
  hours: [{
  		opening: string, // 11:00
  		closing: string, // 22:00
  		day: string // "Monday"
  }],
}
```

#### YelpBusinessSearchParams Class
All fields are strings, but represent other types for Yelp Fusion API
```
{
  term: string,
  location: string, // required if no lat/lon,
  latitude: string, // lat/lon required if no location
  longitude: string,
  radius: string, // float, in meters
  categories: string, // comma separated list of aliases from Yelp category list
  locale: string,
  limit: string, // int, for pagination
  offset: string // int, for pagination
  sort_by: string,
  price: string,
  open_now: string, // bool
  open_at: string,
  attributes: string, // comma separated list
}
```

## API Reference

### GET api/locations/{locationId}
Return: `PoppinLocation` without `yelpDetails`

### PUT api/locations or PUT api/locations/{locationId}
*Requires Admin role*  
Request: PoppinLocationRequest  
Return: updated `PoppinLocation` without Yelp Details

Updates existing PoppinLocation

### DELETE api/locations/{locationId}
*Requires Admin role*  
Request: Empty  
Return: Empty

Deletes existing PoppinLocation

### GET api/locations/with-yelp/{locationId}
Request: Empty  
Return: `PoppinLocation`

Due to the daily request limit on Yelp's Fusion API (5000), we have to limit the number of requests for their data,
and so it is not included in `api/locations/{locationId}. This API addresses that need.
`api/locations/yelp-search` returns yelp data with each result. 

TODO: Implement GraphAPI to get details as needed. Potentially higher usage limit, and will be a good candidate for 
Redis integration. This will not be deprecated with `api/locations/yelp-search`

### POST api/locations/yelp-search (deprecated)
Request: YelpBusinessSearchParams  
Response: 
```
{
  total: int,
  businesses: PoppinLocation[],
  region: {
  	 center: {
  	   latitude: float,
  	   longitude: float
  	 }
  },
  searchParams: YelpBusinessSearchParams
}
```

Initiates a proper Yelp search with the submitted query. Returns all locations in the `PoppinStore.Locations` collection that
have YelpIds that are in the Yelp search results. This limits the number of results we can get back with generic searches, since Yelp's
results will be noisy because of results that aren't relevant to us.

Native search is ready and being tested.

### POST api/locations
Request: PoppinLocationRequest  
Response: PoppinLocation

## Checkins
Each `Checkin` has a reliability score attached.  
- User direct checkin: 1.5
- Vendor checkin (increment- or decrement-crowd): 1
- User geogrpahic checkin: .5

`crowdSize` = The sum of the reliability scores for valid checkins (those that haven't
timed out or been invalidated)

### GET api/locations/checkin/{locationId}
Request: Empty  
Response: PoppinLocation with updated `crowdSize`

User direct checkin (score 1.5)

### GET api/locations/geo-checkin/{locationId}
Request: Empty  
Response: PoppinLocation with updated `crowdSize`

User geo checkin (score .5)

### GET api/locations/increment-crowd/{locationId}
Request: Empty  
Repsonse: PoppinLocation with updated `crowdSize

Vendor checkin (score 1)

### GET api/locations/decrement-crowd/{locationId}
Request: Empty  
Repsonse: PoppinLocation with updated `crowdSize

Invalidates *oldest* checkin at the location 
