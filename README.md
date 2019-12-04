# grpc-dotnet-validator
Request message validator middleware for grpc.aspnet

![](https://github.com/enif-lee/grpc-dotnet-validator/workflows/Build/badge.svg)
![](https://github.com/enif-lee/grpc-dotnet-validator/workflows/Test/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/GrpcExtensions.AspNetCore.Validation)](https://www.nuget.org/packages/GrpcExtensions.AspNetCore.Validation)


## How to use.

This package is integrated with [Fluent Validation](https://github.com/JeremySkinner/FluentValidation). 
If you want to know how build your own validation rules, please checkout [Fluent Validation Docs](https://fluentvalidation.net/start)

```csharp
public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // Enable message validation feature.
        services.AddGrpc(options => options.EnableMessageValidation());

        // Add custom validators for messages, default scope is scope.
        services.AddValidator<HelloRequestValidator>();
        services.AddValidator<HelloRequestValidator>(LifeStyle.Singleton);

        // Add Validator locator, if you didn't satisfy container based locator,
        // You just implement IValidatorLocator and register as service. 
        services.AddValidatorLocator();
    }
    // ...
}
```

Then, If the message is invalid, Grpc Validator return with `InvalidArgument` code and empty message object.


## How to test my validation

If you want to write integration test. [This test sample](src/Grpc.AspNetCore.FluentValidation.Test/IntegrationTest.cs) may help you.
