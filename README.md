# Overview

All in one finance authentication API. Generates JWT for entities such as users and clients

## Tests

Unit tests are ran on each pipeline build. The pipelines are built with `Azure DevOps` from the `azure-pipelines.yml` file

Additionally, there are test result coverage reports done by [Coverlet](https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#collect-code-coverage-metrics-with-coverlet). An example of a pipeline build with unit test coverage report can be found below

- [#20200804.10](https://gkamacharov.visualstudio.com/gkama-cicd/_build/results?buildId=681&view=codecoverage-tab)

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

### Endpoints

Below are descriptions and examples of the important available endpoints

#### /auth/.well-known/openid-configuration

HttpMethod: `GET`
Relative endpoint: `/auth/.well-known/openid-configuration`

Response

```json
{
  "issuer": "aiof:auth",
  "token_endpoint": "http://localhost:5000/auth/token",
  "token_refresh_endpoint": "http://localhost:5000/auth/token/refresh",
  "response_types_supported": [
    "code token"
  ],
  "subject_types_supported": [
    "public",
    "pairwise"
  ],
  "token_endpoint_auth_signing_alg_values_supported": [
    "RS256"
  ],
  "claim_types_supported": [
    "normal"
  ],
  "claims_supported": [
    "sub",
    "iss",
    "public_key",
    "given_name",
    "family_name",
    "name",
    "email",
    "slug"
  ]
}
```

## Documentation

Overall documentation for the AIOF Auth Microservice

The authentication microservice is built to additionally leverage the following libraries:

- [Fluent Validation](https://github.com/FluentValidation/FluentValidation#get-started) for validation
- [Polly](https://github.com/App-vNext/Polly#polly) for resiliency and transient-fault-handling

### Microsoft

- [Implement the Circuit Breaker pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern)
- [Microsoft Feature Flag Management](https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core)

### JWT

- [IANA JSON Web Token (JWT)](https://www.iana.org/assignments/jwt/jwt.xhtml)
