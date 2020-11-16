# Overview

All in one finance authentication API. Generates JWT for entities such as users and clients. Full documentation of the API can be found at [aiof-auth](https://kamacharovs.github.io/aiof-auth/)

[![Build Status](https://gkamacharov.visualstudio.com/gkama-cicd/_apis/build/status/kamacharovs.aiof-auth?branchName=master)](https://gkamacharov.visualstudio.com/gkama-cicd/_build/latest?definitionId=22&branchName=master)

## Documentation

Overall documentation for the aiof Auth microservice

### Authentication

Authentication can be done via the `/auth/token` endpoint. There are several ways an entity can authenticate:

- `username` and `password` for `User`
- `api_key` for `User` or `Client`
- `refresh_token` for `User` or `Client`

#### Example for `User`

Request

```json
{
    "username": "test",
    "password": "test"
}
```

Response

```json
{
    "user": {
        "id": 1,
        "publicKey": "581f3ce6-cf2a-42a5-828f-157a2bfab763",
        "firstName": "test",
        "lastName": "test",
        "email": "test@test.com",
        "username": "test",
        "role": {
            "name": "Admin"
        },
        "created": "2020-09-08T15:54:08.277753"
    },
    "token_type": "Bearer",
    "expires_in": 900,
    "access_token": "jwt_access_token",
    "refresh_token": "refresh_token"
}
```

#### Example for `Client`

Request

```json
{
    "api_key": "api_key_here"
}
```

Response

```json
{
    "token_type": "Bearer",
    "expires_in": 900,
    "access_token": "jwt_access_token",
    "refresh_token": "refresh_token"
}
```

### Tests

Unit tests are ran on each pipeline build. The pipelines are built with `Azure DevOps` from the `azure-pipelines.yml` file. Additionally, as part of the build pipeline, there are test result coverage reports done by [Coverlet](https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#collect-code-coverage-metrics-with-coverlet). Also, you can click on the build pipeline badge and check the unit test coverage for the latest run

### Libraries

- [Fluent Validation](https://github.com/FluentValidation/FluentValidation#get-started) for validation

### JWT

- [IANA JSON Web Token (JWT)](https://www.iana.org/assignments/jwt/jwt.xhtml)
- [OpenID Connect Discovery 1.0](https://openid.net/specs/openid-connect-discovery-1_0.html)
- [Configure Applications with OpenID Connect Discovery](https://auth0.com/docs/protocols/oidc/openid-connect-discovery)

### OpenSSL

The service currently uses RSA256 algorithm to sign the JWT's. For this scenario we use OpenSSL to generate a private and public key. In order to do so follow the below steps:

- Install `openssl` tools from Chocolatey by running the following command: `choco install openssl.light` (needs to only be done once)
- Then restart PowerShell, if required
- Navigate to a desired directory to create the `.pem` files
- Run the command: `openssl genrsa -out private-key.pem 2048`
- Run the command: `openssl rsa -in private-key.pem -outform PEM -pubout -out public-key.pem`

A good article with detailed documentation can be found [here](https://dotnetuniversity.com/jwt-authentication-in-asp-net-core/). Also, a `.pem` to `XML` converter tool can be found [here](https://superdry.apphb.com/tools/online-rsa-key-converter)

## How to run it

The API is designed to be run as a standalone API leveraging in memory database. The fake data comes from the `FakeDataManager` class. This is achieved when this is enabled in conjunction with the `ASPNETCORE_ENVIRONMENT='Development'`. These configurations are configured in the `appsettings.Development.json`

```json
...
"Data": {
    "InMemory": false,
    "PostgreSQL": "connectionstring"
},
...
```

The default value for the `InMemory` data is `false`. If changed to `true`, then the API can be ran locally as a standalone instance. Additionally, it can be ran in conjunction with `aiof-data` Docker image as a full API.

From the root project directory

```powershell
dotnet run -p .\aiof.auth.core\
```

Or change directories and run from the core `.csproj`

```powershell
cd .\aiof.auth.core\
dotnet run
```

Make API calls to

```text
http://localhost:5000
```

### Docker

Pull the latest image from Docker Hub

```powershell
docker pull gkama/aiof-auth:latest
```

Run it

```powershell
docker run -it --rm -e ASPNETCORE_ENVIRONMENT='Development' -e Data__InMemory='true' -p 8001:80 gkama/aiof-auth:latest
```

Make API calls to

```text
http://localhost:8001/
```

(Optional) Clean up `none` images

```powershell
docker rmi $(docker images -f “dangling=true” -q)
```

### Docker compose

From the root project directory

```powershell
docker-compose up
```

This will initialize the `aiof-data` database. Then you can run the API from the root project directory with `dotnet run -p .\aiof.auth.core\` as have it as a fully functioning local API
