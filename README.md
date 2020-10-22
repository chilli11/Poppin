# Poppin
Poppin app


# API Basics
See API formats in Swagger docs @ /swagger in dev or local

## Locations
[Reference](Docs/Locations.md)

All location information can be found and updated from /locations APIs. These APIs use a basic cache (TODO: Redis). 

`GET /api/locations/{locationId}`: does not return Yelp data unless it's been previously retrieved and in cache; don't expect it.
+ use `GET /api/locations/with-yelp/{locationId}` instead

### Checkins
[Reference](Docs/Locations.md#checkins)

`​GET /api​/Locations​/checkin​/{locationId}`: Can be anonymous, represents a user initiated check in
+ Has a value of 1.5
+ Vendor Checkin has value of 1 (`incrementcrowd` and `decrementcrowd`)
+ Geo Checkin has value of .5

`GET /api/locations/incrementcrowd` and `GET /api/locations/decrementcrowd` are for Vendor view only.

### Yelp
`GET /api/yelp/match/{locactionId}`: returns just the Yelp info for the location

`GET /api/yelp/{yelpId}`: returns the Yelp info for a given Yelp ID

`POST /api/yelp/businesses`: a basic Yelp search
+ Body:
    ```
    {
      "term": "string",
      "location": "string",
      "latitude": "string",
      "longitude": "string",
      "radius": "string",
      "categories": "string",
      "locale": "string",
      "limit": "string",
      "offset": "string",
      "sort_by": "string",
      "price": "string",
      "open_now": "string",
      "open_at": "string",
      "attributes": "string"
    }
    ```

`POST /api/yelp/match`: gets Yelp data based on business name and location or phone number
+ Body
    ```
    {
      "name": "string",
      "address1": "string",
      "address2": "string",
      "address3": "string",
      "city": "string",
      "state": "string",
      "country": "string",
      "latitude": "string",
      "longitude": "string",
      "phone": "string",
      "zip_code": "string",
      "yelp_business_id": "string",
      "match_threshold": "string"
    }
    ```

## Users

Data cannot be manipulated through /identity APIs

Login: `/api/identity/login`

Registration: `/api/identity/register`
+ Password requirements: 1 upper, 1 lower, 1 number, 1 symbol (_!@#$%^&*)

Basic User info: `POST /api/identity/me` (contains user role)

Validate Auth: `GET /api/identity/is-authenticated`
  
Refresh Token: `POST /api/identity/refresh-token`
+ Refresh tokens have a life of 8 hours as of 28-08-2020. Will likely update to permanent or 30 days.

Revoke Token: `POST /api/identity/revoke-token`
  
### Profiles
  
User Profile: `/api/profile` (GET, PUT, POST)

Track User Location: `POST /api/profile/updateGeo`
+ Uses GeoJson Point geometry type as body
    Example: 
    ```
    {
        "type": "Point",
        "coordinates": [
            -105.01621,
            39.57422
        ]
    }
    ```
+ If the nearest location searched in the previous 2 hours is within 50 meters, a Geo Checkin will be logged

Other User's Profile: `/api/profile/{userId}` (GET, PUT, DELETE)
+ only available to Admins as of 28-08-2020)

User Locations:
+ `GET /api/profile/recently-viewed` returns list of locations viewed today and yesterday
+ `GET /api/profile/addfavorite/{locationId}`
+ `GET /api/profile/hidefavorite/{locationId}`
+ `GET /api/profile/hidelocation/{locationId}` (hides from search)
+ `GET /api/profile/unhidelocation/{locationId}` (unhides from search)
