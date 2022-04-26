using Calzolari.Grpc.AspNetCore.Validation.Internal;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Calzolari.Grpc.AspNetCore.Validation
{
    public static class ServiceCollectionHelper
    {
        /// <summary>
        ///     Add default component for validating grpc messages
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns>service collection</returns>
        public static IServiceCollection AddGrpcValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidatorLocator>(provider => new ServiceCollectionValidationProvider(provider));
            
            if (services.All(r => r.ServiceType != typeof(IValidatorErrorMessageHandler)))
                services.AddSingleton<IValidatorErrorMessageHandler, DefaultErrorMessageHandler>();

            return services;
        }

        /// <summary>
        ///     Add custom message validator.
        /// </summary>
        /// <param name="services">service collection</param>
        /// <param name="lifetime">specific life time for validator</param>
        /// <typeparam name="TValidator">custom validator type</typeparam>
        /// <returns></returns>
        /// <exception cref="AggregateException">When try to register along validator class.</exception>
        public static IServiceCollection AddValidator<TValidator>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped) where TValidator : class
        {
            var implementationType = typeof(TValidator);
            var validatorType = implementationType.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>));

            if (validatorType == null)
                throw new AggregateException(implementationType.Name + "is not implement with IValidator<>.");

            var messageType = validatorType.GetGenericArguments().First();
            var serviceType = typeof(IValidator<>).MakeGenericType(messageType);

            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            return services;
        }

        /// <summary>
        ///     Add inline validator for simple rule.
        /// </summary>
        /// <param name="services">service collection</param>
        /// <param name="validator">configure validation rules</param>
        /// <typeparam name="TMessage">grpc message type</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddInlineValidator<TMessage>(this IServiceCollection services, 
            Action<AbstractValidator<TMessage>> validator)
        {
            services.AddSingleton<IValidator<TMessage>>(new Validation.InlineValidator<TMessage>(validator));
            return services;
        }

        /// <summary>
        /// Adds all validators in specified assembly
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Transient,
            Func<AssemblyScanner.AssemblyScanResult, bool> filter = null)
        {
            foreach (var scanResult in AssemblyScanner.FindValidatorsInAssembly(assembly).Where(filter ?? (_ => true)))
                services.AddValidator(scanResult.ValidatorType);
            return services;
        }

        /// <summary>
        ///     Add custom message validator.
        /// </summary>
        /// <param name="services">service collection</param>
        /// <param name="lifetime">specific life time for validator</param>
        /// <param name="type">custom validator type</param>
        /// <returns></returns>
        public static IServiceCollection AddValidator(this IServiceCollection services, Type type,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var implementationType = type;
            var validatorType = implementationType.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>));

            if (validatorType == null)
                throw new AggregateException(implementationType.Name + "is not implement with IValidator<>.");

            var messageType = validatorType.GetGenericArguments().First();
            var serviceType = typeof(IValidator<>).MakeGenericType(messageType);

            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            return services;
        }
    }
}