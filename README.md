# Poppin
 Poppin app


# API Basics
See API formats in Swagger docs @ /swagger in dev or local

## Locations

All location information can be found and updated from /locations APIs

## Users

Data cannot be manipulated through /identity APIs
+ Login: /api/identity/login
+ Registration: /api/identity/register
+ Basic User info: /api/identity/me (contains user role)
+ Validate Auth: /api/identity/is-authenticated (GET)
  
Refresh tokens have a life of 8 hours as of 28-08-2020. Will likely update to permanent or 30 days.
  
### Profiles
  
+ User Profile: /api/profile (GET, PUT, POST)
    + /api/profile/updateGeo
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
+ Other User's Profile: /api/profile/{userId} (GET, PUT, DELETE; only available to Admins as of 28-08-2020)
+ Locations
    + /api/profile/addfavorite/{locationId}
    + /api/profile/hidefavorite/{locationId}
    + /api/profile/hidelocation/{locationId} (hides from search)
    + /api/profile/unhidelocation/{locationId} (unhides from search)