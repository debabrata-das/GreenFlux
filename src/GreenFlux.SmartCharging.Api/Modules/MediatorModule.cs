using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Api.Validators;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GreenFlux.SmartCharging.Api.Modules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler)
            // in assembly holding the Commands
            builder.RegisterAssemblyTypes(
                    typeof(SaveConnectorHandler).GetTypeInfo().Assembly).
                AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(
                    typeof(SaveChargeStationHandler).GetTypeInfo().Assembly).
                AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(
                    typeof(SaveGroupHandler).GetTypeInfo().Assembly).
                AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder
                .RegisterAssemblyTypes(typeof(SaveGroupValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();
            // Other types registration
            //...
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).
                As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).
                As(typeof(IPipelineBehavior<,>));
        }

        public class LoggingBehavior<TRequest, TResponse>
            : IPipelineBehavior<TRequest, TResponse>
        {
            //private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
            private readonly ILoggerManager _loggerManager;
            public LoggingBehavior(//ILogger<LoggingBehavior<TRequest, TResponse>> logger, 
                ILoggerManager loggerManager)
            {
                //_logger = logger;
                _loggerManager = loggerManager;
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
            {
                _loggerManager.Info($"Handling {typeof(TRequest).Name}");
                var response = await next();
                _loggerManager.Info($"Handled {typeof(TResponse).Name}");
                return response;
            }
        }

        public class ValidatorBehavior<TRequest, TResponse>
            : IPipelineBehavior<TRequest, TResponse>
        {
            private readonly IValidator<TRequest>[] _validators;
            public ValidatorBehavior(IValidator<TRequest>[] validators) =>
                _validators = validators;

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
            {
                var failures = _validators
                    .Select(v => v.Validate(request))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();

                if (failures.Any())
                {
                    throw new GreenFluxDomainException($"Command Validation Errors for type {typeof(TRequest).Name}", 
                        new AggregateException(failures.Select(failure => new ValidationException(failure.ErrorMessage))));
                }

                var response = await next();
                return response;
            }
        }
    }
}
