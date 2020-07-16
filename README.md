# Overview

All in one finance authentication API

## Request

A request for a token is done through the `ITokenRequest<T>` object. The end user/entity will sent a HTTP Post method call to the `/auth/token` endpoint and either provide an `ApiKey` or a `Username` and `Password`. Then, you will receive a typical `ITokenResponse` where the access token is provided

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


## Documentation

Overall documentation for the AIOF Auth Microservice

### JWT

- [IANA JSON Web Token (JWT)](https://www.iana.org/assignments/jwt/jwt.xhtml)