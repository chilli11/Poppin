# Identity
Reference for `api/locations` endpoints. Contains OAuth;

## Response Classes

#### AuthSuccessResponse
```
{
  token: string,
  refreshToken: string
}
```

#### AuthFailedResponse
```
{
  errors: string[]
}
```

#### UserSuccessResponse
Extends Microsoft.AspNetCore.Identity.IdentityUser
```
{
  user: {
    role: string,
    refreshTokens: string[],
    roleIdentity: Microsoft.AspNetCore.Identity.IdentityRole
  }
}
```


## API Reference
Any failures will get a 400 or 404 with an `AuthFailedResponse`

### GET api/identity/is-authenticated
Request: Empty  
Response: `{ authorized: bool }`

Simple check for authentication. Always returns 200.  
Include the most recent Bearer token in the headers.

### POST api/identity/register
Request:
```
{
  email: string,
  password: string,
  password2: string
}
```
Response: AuthSuccessResponse

### GET api/identity/confirm-email/{id}
Request: `{ t: string }` // auth token  
Response: AuthSuccessResponse

### POST api/identity/login
Request:
```
{
  email: string,
  password: string
}
```
Response: AuthSuccessResponse

### POST api/identity/forgot-password
Request: `{ email: string }`  
Response: AuthSuccessResponse

Initiates password reset email to email address on file

### GET api/identity/reset-password/{id}
Request: `{ t: string }` // auth token  
Response: AuthSuccessResponse

This is the url that the UI will hit after the user click's the link in their email. It will validate
the token before loading the reset form

### POST api/identity/reset-password/{id}
Request: 
```
{
  token: string, // received by email
  password: string,
  password2: string
}
```
Response: AuthSuccessResponse

The endpoint will validate the token again, then reset the password if it meets requirements.

### POST api/identity/refresh-token
Request: Empty  
Response: AuthSuccessResponse

Checks the cookies for a valid refresh token

### POST api/identity/revoke-token
Request: RevokeTokenRequest  
Response: `{ message: string }`

### POST api/identity/me
Request: Empty  
Response: UserSuccessResponse

Returns Identity data, *not user profile data* (see [User Profiles](./UserProfiles.md))

### GET api/identity/{id}/refresh-tokens
Request: Empty  
Response: string[] // refresh tokens

### GET api/identity/facebook-login-uri
Request: Empty  
Response: `{ fbLoginUri: string }`

Returns a url that will load a facebook login interface. Not currently in use.

### GET api/identity/facebook-auth
Request: `{ code: string }`  
Response: AuthSuccessResponse

Handles the callback from the Facebook login interface.
Validates the code, then runs the FacebookLogin method implemented in `POST api/identity/facebook-login`

### POST api/identity/facebook-login
Request: `{ accessToken: string }`  
Response: AuthSuccessResponse

### Other OAuth endpoints to come