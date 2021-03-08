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
  vendorIds: string[],
  requestedLocations: string[], // IDs of locations the user requested to be added as partners
}
```

#### PoppinUserRequest Class
Used for updating profile info. Does not include `id`, as it is not editable.  
Also excludes `favorites`, `hidden`, and `vendorIds` as those are edited elsewhere.
```
{
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
}
```

#### PoppinUserResult Class
```
{
  user: [PoppinUser](#poppinuserresult),
  vendors: [Vendor](./Vendors.md#vendor-class)[],
  favorites: [PoppinLocation](./Locations.md#poppinlocation-class)[],
  hidden: [PoppinLocation](./Locations.md#poppinlocation-class)[]
}
```

## API Reference

### GET api/profile/
Return: [PoppinUserResult](#poppinuserresult-class) / 401

Returns the currently logged in user's profile.

### GET api/profile/{id}
*Requires Admin role*  
Request: Empty  
Return: [PoppinUserResult](#poppinuserresult-class) / 403 if not admin / 400 if user not found

For site admins to see user data. Probably not going to last long.

### GET api/profile/recently-viewed
Request: Empty  
Return: [PoppinLocation](./Locations.md#poppinlocation-class)[]

List of locations the user recently clicked to view. This does not pull recent search results.

### NEW in 1.2.1
### GET api/profile/request/{locationId}
Request: Empty
Response: Empty

Adds the location to the list of user requests for partnership, and to the user's list of requested locations.

### GET api/profile/favorites/add/{locationId}
Request: Empty  
Return: Empty / 400 if failed

Adds location to the favorites list

### GET api/profile/favorites/remove/{locationId}
Request: Empty  
Return: Empty / 400 if failed

Removes location from the favorites list

### GET api/profile/hide/{locationId}
Request: Empty  
Return: Empty / 400 if failed

Adds location to the hidden list

### GET api/profile/unhide/{locationId}
Request: Empty  
Return: Empty / 400 if failed

Removes location from the hidden list

__NOTE: Hidden locations are NOT hidden from search on the back end as of 2020-10-27. This may change at a later date.__

### PUT api/profile
Request: [PoppinUserRequest](#poppinuserrequest-class)  
Response: [PoppinUser](#poppinuser-class) / 400 if failed

Updates the user's profile info.

### POST api/profile/update-geo
*Experimental*  
Request: [GeoJsonPoint](https://docs.mongodb.com/manual/reference/geojson/#point)  
Response: Empty

This is an attempt at server side geographic check in. It measures the user's location against the recently viewed  
locations list. If the user is within 50 meters of any location in the list, it will create a Geo checkin (value: .5) at the nearest one.  

Currently this would invalidate the user's previous check in.