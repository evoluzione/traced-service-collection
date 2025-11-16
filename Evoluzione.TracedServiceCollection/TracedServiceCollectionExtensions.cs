using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Evoluzione.TracedServiceCollection;

/// <summary>
/// Provides extension methods for adding traced services to the IServiceCollection.
/// </summary>
public static class TracedServiceCollectionExtensions
{
	#region Scoped

	public static IServiceCollection AddTracedScoped<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddScoped<TImplementation>();
		services.AddScoped<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	public static IServiceCollection AddTracedScoped<TInterface, TImplementation>(
			this IServiceCollection services,
			Func<IServiceProvider, TImplementation> factory)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddScoped(factory);
		services.AddScoped<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced scoped keyed service to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedScoped<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedScoped<TImplementation>(serviceKey);
		services.AddKeyedScoped<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced scoped keyed service with a factory to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedScoped<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey,
		Func<IServiceProvider, object?, TImplementation> factory)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedScoped(serviceKey, factory);
		services.AddKeyedScoped<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	#endregion

	#region Transient

	public static IServiceCollection AddTracedTransient<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddTransient<TImplementation>();
		services.AddTransient<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	public static IServiceCollection AddTracedTransient<TInterface, TImplementation>(
			this IServiceCollection services,
			Func<IServiceProvider, TImplementation> factory)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddTransient(factory);
		services.AddTransient<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced transient keyed service to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedTransient<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedTransient<TImplementation>(serviceKey);
		services.AddKeyedTransient<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced transient keyed service with a factory to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedTransient<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey,
		Func<IServiceProvider, object?, TImplementation> factory)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedTransient(serviceKey, factory);
		services.AddKeyedTransient<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	#endregion

	#region Singleton

	public static IServiceCollection AddTracedSingleton<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddSingleton<TImplementation>();
		services.AddSingleton<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	public static IServiceCollection AddTracedSingleton<TInterface, TImplementation>(
			this IServiceCollection services,
			Func<IServiceProvider, TImplementation> factory)
			where TInterface : class
			where TImplementation : class, TInterface
	{
		services.AddSingleton(factory);
		services.AddSingleton<TInterface>(provider =>
		{
			var decorated = provider.GetRequiredService<TImplementation>();
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}
	
	public static IServiceCollection AddTracedSingleton<TInterface>(
		this IServiceCollection services,
		TInterface instance)
		where TInterface : class
	{
		var proxy = CreateProxy(instance);
		services.AddSingleton(proxy);
		return services;
	}

	public static IServiceCollection TryAddTracedSingleton<TInterface, TImplementation>(
		this IServiceCollection services)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		return services.All(x => x.ServiceType != typeof(TInterface))
			? AddTracedSingleton<TInterface, TImplementation>(services)
			: services;
	}

	/// <summary>
	/// Adds a traced singleton service registered as itself (no separate interface).
	/// </summary>
	public static IServiceCollection AddTracedSingleton<TImplementation>(this IServiceCollection services)
		where TImplementation : class
	{
		return AddTracedSingleton<TImplementation, TImplementation>(services);
	}

	/// <summary>
	/// Adds a traced singleton service with a factory registered as itself (no separate interface).
	/// </summary>
	public static IServiceCollection AddTracedSingleton<TImplementation>(
		this IServiceCollection services,
		Func<IServiceProvider, TImplementation> factory)
		where TImplementation : class
	{
		return AddTracedSingleton<TImplementation, TImplementation>(services, factory);
	}

	/// <summary>
	/// Adds a traced singleton keyed service to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedSingleton<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedSingleton<TImplementation>(serviceKey);
		services.AddKeyedSingleton<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced singleton keyed service with a factory to the IServiceCollection.
	/// </summary>
	public static IServiceCollection AddTracedKeyedSingleton<TInterface, TImplementation>(
		this IServiceCollection services,
		object? serviceKey,
		Func<IServiceProvider, object?, TImplementation> factory)
		where TInterface : class
		where TImplementation : class, TInterface
	{
		services.AddKeyedSingleton(serviceKey, factory);
		services.AddKeyedSingleton<TInterface>(serviceKey, (provider, key) =>
		{
			var decorated = provider.GetRequiredKeyedService<TImplementation>(key);
			return CreateProxy<TInterface>(decorated);
		});
		return services;
	}

	#endregion

	#region HostedService

	/// <summary>
	/// Adds a traced hosted service to the IServiceCollection.
	/// </summary>
	/// <typeparam name="THostedService">The type of the hosted service to add.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <returns>The service collection for chaining.</returns>
	public static IServiceCollection AddTracedHostedService<THostedService>(this IServiceCollection services)
		where THostedService : class, IHostedService
	{
		services.AddSingleton<THostedService>();
		services.AddSingleton<IHostedService>(provider =>
		{
			var decorated = provider.GetRequiredService<THostedService>();
			return CreateProxy<IHostedService>(decorated);
		});
		return services;
	}

	/// <summary>
	/// Adds a traced hosted service to the IServiceCollection with a factory.
	/// </summary>
	/// <typeparam name="THostedService">The type of the hosted service to add.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <param name="factory">The factory function to create the hosted service.</param>
	/// <returns>The service collection for chaining.</returns>
	public static IServiceCollection AddTracedHostedService<THostedService>(
		this IServiceCollection services,
		Func<IServiceProvider, THostedService> factory)
		where THostedService : class, IHostedService
	{
		services.AddSingleton(factory);
		services.AddSingleton<IHostedService>(provider =>
		{
			var decorated = provider.GetRequiredService<THostedService>();
			return CreateProxy<IHostedService>(decorated);
		});
		return services;
	}

	#endregion

	/// <summary>
	/// Creates a proxy for the specified interface that traces method calls.
	/// </summary>
	/// <typeparam name="TInterface">The type of the service interface.</typeparam>
	/// <param name="decorated">The concrete implementation of the service.</param>
	/// <returns>A proxy that traces method calls.</returns>
	private static TInterface CreateProxy<TInterface>(TInterface decorated) where TInterface : class
	{
		object proxy = DispatchProxy.Create<TInterface, TracingProxy<TInterface>>();
		((TracingProxy<TInterface>)proxy).Decorated = decorated;
		return (TInterface)proxy;
	}
}
