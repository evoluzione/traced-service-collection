using System.Diagnostics;
using System.Reflection;

namespace Evoluzione.TracedServiceCollection;

/// <summary>
/// A proxy class that traces method calls for a specified type.
/// </summary>
/// <typeparam name="T">The type of the service being proxied.</typeparam>
public class TracingProxy<T> : DispatchProxy
{
    /// <summary>
    /// Gets or sets the concrete implementation of the service being proxied.
    /// </summary>
    public T Decorated { get; set; } = default!;

    private readonly ActivitySource _activitySource = new("TracedServiceCollection");

    /// <summary>
    /// Invokes the specified method on the proxied service and traces the method call.
    /// </summary>
    /// <param name="targetMethod">The method to be invoked.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the target method is null.</exception>
    /// <exception cref="TargetInvocationException">Thrown when the target method invocation throws an exception.</exception>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
            throw new ArgumentNullException(nameof(targetMethod));

        var methodName = targetMethod.Name;

        using var activity = _activitySource.StartActivity(methodName);

        activity?.SetTag("method", methodName);

        try
        {
            var result = targetMethod.Invoke(Decorated, args);
            return result;
        }
        catch (TargetInvocationException tie)
        {
            if (activity != null)
            {
                activity.SetTag("error", true);
                activity.SetTag("exception", tie.InnerException?.ToString());
            }

            throw tie.InnerException ?? tie;
        }
    }
}