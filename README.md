# grpc-aspnetcore-validator
Request message validator middleware for [Grpc.AspNetCore](https://github.com/grpc/grpc-dotnet)

![](https://github.com/AnthonyGiretti/grpc-aspnetcore-validator/workflows/Build/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/Calzolari.Grpc.AspNetCore.Validation)](https://www.nuget.org/packages/Calzolari.Grpc.AspNetCore.Validation)

## Features

- Server Side validation
- Client Side detailed errors fetching in RpcException

### Client Side usage

Download the client side package here: [Calzolari.Grpc.Net.Client.Validation](https://www.nuget.org/packages/Calzolari.Grpc.Net.Client.Validation/)

```csharp
try
{

    using var channel = GrpcChannel.ForAddress("https://localhost:5001");
    var client =  new Greeter.GreeterClient(channel);
     // Empty value that raises an error validation
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "" });
}
catch (RpcException e)
{
    var errors = e.GetValidationErrors(); // Gets validation errors list
}
```

### Server Side usage

Download the server side package here: [Calzolari.Grpc.AspNetCore.Validation](https://www.nuget.org/packages/Calzolari.Grpc.AspNetCore.Validation/)

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

        // 2. Add custom validators for messages, default scope is scoped.
        services.AddValidator<HelloRequestValidator>();
        services.AddValidator<HelloRequestValidator>(LifeStyle.Singleton);

        // 3. Add Validator locator
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

        // 3. Add Validator locator
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

        // 1. Just put at service collection your own custom message handler that implement IValidatorErrorMessageHandler.
        // This should be placed before calling AddGrpcValidation();
        services.AddSingleton<IValidatorErrorMessageHanlder>(new CustomMessageHandler())

        // If you don't reigster any message handler, AddGrpcValidation register default message handler.  
        services.AddGrpcValidation();
    }
    // ...
}
```

## How to test validation

If you want to write integration tests. [This test sample](src/Grpc.AspNetCore.FluentValidation.Test/Integration/) may help you.
