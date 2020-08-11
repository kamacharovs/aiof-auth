# Overview

All in one finance authentication API. Generates JWT for entities such as users and clients. Full documentation of the API can be found at [aiof-auth](https://kamacharovs.github.io/aiof-auth/)

## Documentation

Overall documentation for the AIOF Auth Microservice

### Tests

Unit tests are ran on each pipeline build. The pipelines are built with `Azure DevOps` from the `azure-pipelines.yml` file

Additionally, there are test result coverage reports done by [Coverlet](https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#collect-code-coverage-metrics-with-coverlet). An example of a pipeline build with unit test coverage report can be found below

- [#20200804.10](https://gkamacharov.visualstudio.com/gkama-cicd/_build/results?buildId=681&view=codecoverage-tab)

The authentication microservice is built to additionally leverage the following libraries:

- [Fluent Validation](https://github.com/FluentValidation/FluentValidation#get-started) for validation
- [Polly](https://github.com/App-vNext/Polly#polly) for resiliency and transient-fault-handling

### Microsoft

- [Implement the Circuit Breaker pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern)
- [Microsoft Feature Flag Management](https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core)

### JWT

- [IANA JSON Web Token (JWT)](https://www.iana.org/assignments/jwt/jwt.xhtml)
- [OpenID Connect Discovery 1.0](https://openid.net/specs/openid-connect-discovery-1_0.html)
- [Configure Applications with OpenID Connect Discovery](https://auth0.com/docs/protocols/oidc/openid-connect-discovery)

### OpenSSL

If you plan on using RSA256 algorithm to sign the JWT's then you need a Certificate. For this scenario we use OpenSSL to generate a private key. In order to do so follow the below steps:

- Install `openssl` tools from Chocolatey by running the following command: `choco install openssl.light` (needs to only be done once)
- Then restart PowerShell, if required
- Navigate to a desired directory to create a Certificate
- Run the command: `openssl genrsa -out private-key.pem 2048`
- Run the command: `openssl rsa -in private-key.pem -outform PEM -pubout -out public-key.pem`

A good article with detailed documentation can be found [here](https://dotnetuniversity.com/jwt-authentication-in-asp-net-core/)
