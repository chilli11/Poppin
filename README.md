# Poppin
Poppin app


# API Basics
See API formats in Swagger docs @ /swagger in dev or local

## Locations

All location information can be found and updated from /locations APIs. These APIs use a basic cache. 

`GET /api/locations/{locationId}`: does not return Yelp data unless it's been previously retrieved and in cache; don't expect it.
+ use `GET /api/locations/with-yelp/{locationId}` instead

### Checkins

`​GET /api​/Locations​/checkin​/{locationId}`: Can be anonymous, represents a user initiated check in
+ Has a value of 1.5
+ vendor checkin has value of 1 (`incrementcrowd` and `decrementcrowd`)
+ location checkin has value of .5

`GET /api/locations/incrementcrowd` and `GET /api/locations/decrementcrowd` are for Vendor view only.

### Yelp
`/api/yelp/match/{locId}`: returns just the Yelp info for the location

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

Other User's Profile: `/api/profile/{userId}` (GET, PUT, DELETE)
+ only available to Admins as of 28-08-2020)

User Locations:
+ `GET /api/profile/addfavorite/{locationId}`
+ `GET /api/profile/hidefavorite/{locationId}`
+ `GET /api/profile/hidelocation/{locationId}` (hides from search)
+ `GET /api/profile/unhidelocation/{locationId}` (unhides from search)
