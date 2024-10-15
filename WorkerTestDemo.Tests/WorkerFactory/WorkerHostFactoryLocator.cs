using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace WorkerTestDemo.Tests.WorkerFactory;

internal sealed class WorkerHostFactoryLocator
{
    private readonly Type _entryPoint;
    private readonly string _environment;
    private readonly string? _methodName;
    private const BindingFlags FactoryMethodFlags =
        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    public WorkerHostFactoryLocator(Type entryPoint, string? methodName, string environment)
    {
        _entryPoint = entryPoint;
        _environment = environment;
        _methodName = methodName;

        if (TryGetFactoryMethodByName(out var factoryMethod))
            CreateBuilder(factoryMethod);
        else if (TryGetHostApplicationBuilderFactory(out factoryMethod))
            CreateHostApplicationBuilder(factoryMethod);
        else if (TryGetIHostBuilderFactory(out factoryMethod))
            CreateHostBuilder(factoryMethod);
        else
            ThrowNoFactoryFound();
    }

    private void CreateBuilder(MethodInfo factoryMethod)
    {
        if (factoryMethod.ReturnType == typeof(IHostBuilder))
            CreateHostBuilder(factoryMethod);
        else if (factoryMethod.ReturnType == typeof(HostApplicationBuilder))
            CreateHostApplicationBuilder(factoryMethod);
        else
            throw new Exception(
                $"Method '{factoryMethod.Name}' does not return a valid host builder."
            );
    }

    private void CreateHostApplicationBuilder(MethodInfo factoryMethod) =>
        HostApplicationBuilder = (HostApplicationBuilder)
            factoryMethod.Invoke(
                null,
                [
                    new HostApplicationBuilderSettings
                    {
                        ApplicationName = _entryPoint.Assembly.GetName().Name,
                        EnvironmentName = _environment
                    }
                ]
            )!;

    private void CreateHostBuilder(MethodInfo factoryMethod) =>
        HostBuilder = (IHostBuilder)
            factoryMethod.Invoke(null, [Array.Empty<string>()])!;

    private void ThrowNoFactoryFound() =>
        throw new Exception(
            $"Unable to find a suitable factory method on type `{_entryPoint.FullName}`."
        );

    private bool TryGetFactoryMethodByName(out MethodInfo factoryMethod)
    {
        if (string.IsNullOrWhiteSpace(_methodName))
        {
            factoryMethod = default!;
            return false;
        }

        var methodByName = _entryPoint.GetMethod(_methodName, FactoryMethodFlags);

        factoryMethod =
            methodByName
            ?? throw new Exception(
                $"Expected to find method '{_methodName}' on type `{_entryPoint.FullName}`."
            );
        return true;
    }

    private bool TryGetHostApplicationBuilderFactory(out MethodInfo factoryMethod)
    {
        factoryMethod = default!;
        var hostBuilderFactory = _entryPoint
            .GetMethods(FactoryMethodFlags)
            .SingleOrDefault(
                method =>
                    method.ReturnType == typeof(HostApplicationBuilder)
                    || method.ReturnType == typeof(IHostApplicationBuilder)
            );

        if (hostBuilderFactory is null)
            return false;

        factoryMethod = hostBuilderFactory;
        return true;
    }

    private bool TryGetIHostBuilderFactory(out MethodInfo factoryMethod)
    {
        factoryMethod = default!;
        var hostBuilderFactory = _entryPoint
            .GetMethods(FactoryMethodFlags)
            .SingleOrDefault(method => method.ReturnType == typeof(IHostBuilder));

        if (hostBuilderFactory is null)
            return false;

        factoryMethod = hostBuilderFactory;
        return true;
    }

    public HostApplicationBuilder? HostApplicationBuilder { get; private set; }
    public IHostBuilder? HostBuilder { get; private set; }
}