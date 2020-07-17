# Overview

All in one finance authentication API. Generates JWT for entities such as users and clients

## Request

A request for a token is done through the `ITokenRequest<T>` object. The end user/entity will sent a HTTP Post method call to the `/auth/token` endpoint and either provide an `ApiKey` or a `Username` and `Password`. Then, you will receive a typical `ITokenResponse` where the access token is provided

Current supported entities are:
- `User`
- `Client`

Request

```csharp
public interface ITokenRequest<T>
    where T : class
{
    string ApiKey { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    T Entity { get; set; }
    string EntityType { get; }
}
```

Response

```json
{
  "token_type": "Bearer",
  "expires_in": 900,
  "access_token": "token.here"
}
```

## Documentation

Overall documentation for the AIOF Auth Microservice

### JWT

- [IANA JSON Web Token (JWT)](https://www.iana.org/assignments/jwt/jwt.xhtml)