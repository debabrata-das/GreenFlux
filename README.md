# GreenFlux
GreenFlux Assignment

A RESTful ASP.NET Core API for the GreenFlux Assignment

## Prerequirements

* Visual Studio 2019

## Frameworks & Packages used:
* .NET 5
* Microsoft.EntityFrameworkCore 5.0.7
* AutoMapper
* FluentValidation
* MediatR
* prometheus
* log4net
* Swagger

### Next Steps
- Add more metrics using prometheus/grafana dashboard
- Use LogStash and push logs to a Kibana instance generated from log4net
- Add more integration & unit tests to extend test coverage
- Add sorting, filtering, and pagination support for the GET methods
- Add an exception middleware to catch all unhandled errors
- Dockerize the application.
- Test against a real database (this application uses EF Core In-Memory Database Provider)
