# grpc-aspnetcore-validator
Request message validator middleware for [Grpc.AspNetCore](https://github.com/grpc/grpc-dotnet)

[![Nuget](https://img.shields.io/nuget/v/GrpcExtensions.AspNetCore.Validation)](https://www.nuget.org/packages/GrpcExtensions.AspNetCore.Validation)


## Feature

- Support async validation
- Support IoC LifeStyle scopes and dependency injection

## How to use.

This package is integrated with [Fluent Validation](https://github.com/JeremySkinner/FluentValidation). 
If you want to know how build your own validation rules, please checkout [Fluent Validation Docs](https://fluentvalidation.net/start)

#### Add custom message validator

```csharp
// Write own message validator
public class HelloRequestValidator : AbstractValidator<HelloRequest>
{
    public HelloRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
    }
}

public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Enable message validation feature.
        services.AddGrpc(options => options.EnableMessageValidation());

        // 2. Add custom validators for messages, default scope is scope.
        services.AddValidator<HelloRequestValidator>();
        services.AddValidator<HelloRequestValidator>(LifeStyle.Singleton);

        // 3. Add Validator locator, if you didn't satisfy container based locator,
        // You just implement IValidatorLocator and register as service. 
        services.AddGrpcValidation();
    }
    // ...
}
```

Then, If the message is invalid, Grpc Validator return with `InvalidArgument` code and empty message object.

#### Add inline custom validator

if you don't want to create many validation class for simple validation rule in your project,
you just use below inline validator feature like below example.

Note that, Inline validator always be registered **singleton** in your service collection.
Because, There are no way for using other dependency.

```csharp
public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Enable message validation feature.
        services.AddGrpc(options => options.EnableMessageValidation());

        // 2. Add inline validators for messages, scope is always singleton
        services.AddInlineValidator<HelloRequest>(rules => rules.RuleFor(request => request.Name).NotEmpty());

        // 3. Add Validator locator, if you didn't satisfy container based locator,
        // You just implement IValidatorLocator and register as service. 
        services.AddGrpcValidation();
    }
    // ...
}
```


#### Customize validation failure message.

If you want to custom validation message handler for using your own error message system,
Just implement IValidatorErrorMessageHandler and put it service collection.

```csharp
public class CustomMessageHandler : IValidatorErrorMessageHandler
{
    public Task<string> HandleAsync(IList<ValidationFailure> failures)
    {
        return Task.FromResult("Validation Error!");
    }
}

public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options => options.EnableMessageValidation());
        services.AddInlineValidator<HelloRequest>(rules => rules.RuleFor(request => request.Name).NotEmpty());

        // 1. Just put at service collection your own custom message handler that implement IValidatorErrorMessageHnadler.
        // This should be placed before calling AddGrpcValidation();
        services.AddSingleton<IValidatorErrorMessageHanlder>(new CustomMessageHandler())

        // If yor don't reigster any message handler, AddGrpcValidation register default message handler.  
        services.AddGrpcValidation();
    }
    // ...
}
```

## How to test my validation

If you want to write integration tests. [This test sample](src/Grpc.AspNetCore.FluentValidation.Test/Integration/) may help you.


## Versioning

This pakage`s versioning is following version of [Grpc.AspNetCore](https://github.com/grpc/grpc-dotnet)

 
