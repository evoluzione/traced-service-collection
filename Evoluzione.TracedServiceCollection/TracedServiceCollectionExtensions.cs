using Microsoft.Extensions.DependencyInjection;
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
