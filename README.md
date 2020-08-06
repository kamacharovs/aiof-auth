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
