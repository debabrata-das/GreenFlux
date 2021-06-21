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
* SpecFlow
* NUnit

### Next Steps
- Add sorting, filtering, and pagination support for the GET methods
- Add more integration & unit tests to extend test coverage
- Add more metrics using prometheus and create a grafana dashboard  
- Use LogStash and push the logs generated from log4net to a Kibana instance
- Add an exception middleware to catch all unhandled errors
- Test against a real database (this application uses EF Core In-Memory Database Provider)
- Dockerize the application.

