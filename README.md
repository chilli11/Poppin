# Poppin
Poppin app v 1.2.1

# API Basics
See API formats in Swagger docs @ /swagger in dev or local  
This site uses standard bearer authorization with JWT. 

## Locations
[Reference](Docs/Locations.md)

All location information can be found and updated from /locations APIs. These APIs use a basic cache (TODO: Redis). 

### Checkins
[Reference](Docs/Locations.md#checkins)

`​GET /api​/Locations​/checkin​/{locationId}`: Can be anonymous, represents a user initiated check in  
- Has a value of 1.2  
- Vendor Checkin has value of 1 (`incrementcrowd` and `decrementcrowd`)  
- Geo Checkin has value of .6

`GET /api/locations/increment-crowd` and `GET /api/locations/decrement-crowd` are for Vendor view only.

### Forecasts
[Reference](Docs/Locations.md#forecastweek-class)  
- `PoppinLocation.Forecast` {int}  
- Forecasts are from [BestTime](https://besttime.app).  

Every time a location is accessed - by search, direct access, user profile, or vendor page - the app checks `PoppinLocation.ForecastWeek.ForecastUpdatedOn`. If it has been more than 14 days, a new forecast is pulled and stored in the background. It WILL NOT be returned during that action. Forecasts can take up to several seconds, so the API won't wait. The forecast will be updated on the `PoppinLocation` object and stored in Mongo. 

### Yelp
`GET /api/yelp/match/{locactionId}`: returns just the Yelp info for the location  
`GET /api/yelp/{yelpId}`: returns the Yelp info for a given Yelp ID  
`POST /api/yelp/businesses`: a basic Yelp search  
- Body:
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
- Body
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
[Reference](Docs/Identity.md)

Data cannot be manipulated through /identity APIs

Login: `/api/identity/login`  
Registration: `/api/identity/register`  
- Password requirements: 1 upper, 1 lower, 1 number, 1 symbol (_!@#$%^&*)  
Basic User info: `POST /api/identity/me` (contains user role)  
Validate Auth: `GET /api/identity/is-authenticated`  
Refresh Token: `POST /api/identity/refresh-token`  
- Refresh tokens have a life of 8 hours as of 28-08-2020. Will likely update to permanent or 30 days.

Revoke Token: `POST /api/identity/revoke-token`
  
### Profiles
[Reference](Docs/Profiles.md)   
User Profile: `/api/profile` (GET, PUT, POST)

Track User Location: `POST /api/profile/update-geo`  
- Uses GeoJson Point geometry type as body
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
- If the nearest location searched in the previous 2 hours is within 50 meters, a Geo Checkin will be logged

