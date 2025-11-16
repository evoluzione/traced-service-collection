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

### Integration with OpenTelemetry

To visualize the traced method calls with OpenTelemetry:

```csharp
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddSource("TracedServiceCollection")  // Add this source!
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();  // Or use OTLP, Jaeger, etc.
    });

// Register your traced services
builder.Services.AddTracedScoped<IMyService, MyService>();
```

This will create activity spans for each method call on your traced services, allowing you to see the execution flow in your telemetry tools (Jaeger, Zipkin, Application Insights, etc.).

## Features

### Complete Lifetime Support
- ✅ Transient services
- ✅ Scoped services  
- ✅ Singleton services
- ✅ Hosted services (IHostedService)

### Advanced Registration
- ✅ Factory functions
- ✅ Self-registration (no interface)
- ✅ Keyed services (.NET 8+)
- ✅ Try-Add methods to prevent duplicates
- ✅ Instance-based registration

### OpenTelemetry Integration
- ✅ Automatic activity creation for method calls
- ✅ Method name tagging
- ✅ Exception tracking
- ✅ Compatible with all OpenTelemetry exporters

## Example: Complete Setup

```csharp
using Evoluzione.TracedServiceCollection;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("TracedServiceCollection")
        .AddConsoleExporter());

// Register traced services
builder.Services.AddTracedScoped<IOrderService, OrderService>();
builder.Services.AddTracedScoped<IPaymentService, PaymentService>();
builder.Services.AddTracedKeyedScoped<INotifier, EmailNotifier>("email");
builder.Services.AddTracedKeyedScoped<INotifier, SmsNotifier>("sms");
builder.Services.AddTracedHostedService<DataSyncService>();

var app = builder.Build();
app.Run();
```

## How It Works

The library uses a proxy pattern based on `DispatchProxy` to intercept method calls:

1. When you register a traced service, the library creates a proxy that wraps your implementation
2. Each method call on the service goes through the proxy
3. The proxy creates an OpenTelemetry Activity (span) for the method
4. The actual method is invoked
5. The activity is completed (with success or exception information)

This provides automatic, non-invasive distributed tracing for your services without modifying your code.

## Requirements

- .NET 9.0 or later
- Microsoft.Extensions.DependencyInjection 9.0+
- Microsoft.Extensions.Hosting.Abstractions 9.0+ (for hosted services)

## License

MIT License - see LICENSE file for details
