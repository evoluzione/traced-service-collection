# Traced Service Collection

A library for tracing method calls in services registered with the Microsoft.Extensions.DependencyInjection framework.

## Overview

This library provides extension methods for adding traced services to the `IServiceCollection`. It uses a proxy class to intercept method calls and log tracing information.

## Usage

To use this library, add the `Evoluzione.TracedServiceCollection` NuGet package to your project.

### Registering Traced Services

To register a traced service, use one of the following extension methods:

* `AddTracedTransient<TInterface, TImplementation>`: Adds a transient service with tracing.
* `AddTracedScoped<TInterface, TImplementation>`: Adds a scoped service with tracing.
* `AddTracedSingleton<TInterface, TImplementation>`: Adds a singleton service with tracing.

Example:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTracedTransient<IMyService, MyService>();
    // or
    services.AddTracedScoped<IMyService, MyService>();
    // or
    services.AddTracedSingleton<IMyService, MyService>();
}
```
Replace `IMyService` with the interface of your service, and `MyService` with the concrete implementation of the service.

### Attaching to OpenTelemetry

To attach this tracing to OpenTelemetry, add the following code in your `Startup.cs` file:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                // ...
                
                tracing.AddSource("TracedServiceCollection");

                // ...
            });
}
```
This will add the `TracedServiceCollection` source to the OpenTelemetry tracing.

## Internals

The library uses a proxy class, `TracingProxy<T>`, to intercept method calls and log tracing information. The proxy class is created using the `DispatchProxy` class from the `System.Reflection` namespace.