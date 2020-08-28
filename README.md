# Poppin
 Poppin app


 # API Basics
 See API formats in Swagger docs @ /swagger in dev or local

 ## Locations

 All location information can be found and updated from /locations APIs

 ## Users

 Data cannot be manipulated through /identity APIs
 + Login: /identity/login
 + Registration: /identity/register
 + Basic User info: /identity/me (contains user role)

 ### Profiles

 + User Profile: /profile (GET, PUT, POST)
 + Other User Profile: /profile/{userId} (GET, PUT, DELETE; only available to Admins as of 28-08-2020)
 + Locations
 ++ /profile/addfavorite/{locationId}
 ++ /profile/hidefavorite/{locationId}
 ++ /profile/hidelocation/{locationId} (hides from search)
 ++ /profile/unhidelocation/{locationId} (unhides from search)
