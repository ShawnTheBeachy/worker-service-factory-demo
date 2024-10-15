using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerTestDemo.Tests.WorkerFactory;

public abstract class WorkerServiceFactory<T> : IDisposable
{
    private readonly HostApplicationBuilder? _hostAppBuilder;
    private readonly IHostBuilder? _hostBuilder;

    protected WorkerServiceFactory()
    {
        var entryPoint = typeof(T).Assembly.EntryPoint!.DeclaringType!;
        var locator = new WorkerHostFactoryLocator(
            entryPoint,
            CreateBuilderMethodName,
            Environment
        );
        _hostAppBuilder = locator.HostApplicationBuilder;
        _hostBuilder = locator.HostBuilder;
    }

    protected virtual void ConfigureApplicationHost(IHostApplicationBuilder builder)
    {
    }

    protected virtual void ConfigureHost(IHostBuilder builder)
    {
    }

    public void Dispose()
    {
        try
        {
            _workerService?.Dispose();
            ServiceScope.Dispose();
        }
        catch (ObjectDisposedException) { }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }

    public TService GetService<TService>()
        where TService : class => ServiceScope.ServiceProvider.GetRequiredService<TService>();

    public void Start() => _ = WorkerService;

    private bool TryHostAppBuilder(out TestWorkerService service)
    {
        service = default!;

        if (_hostAppBuilder is null)
            return false;

        service = new TestWorkerService(_hostAppBuilder, ConfigureApplicationHost);
        return true;
    }

    private bool TryHostBuilder(out TestWorkerService service)
    {
        service = default!;

        if (_hostBuilder is null)
            return false;

        service = new TestWorkerService(_hostBuilder, ConfigureHost);
        return true;
    }

    protected virtual string? CreateBuilderMethodName { get; }
    protected virtual string Environment { get; } = Environments.Development;

    private IServiceScope? _serviceScope;
    private IServiceScope ServiceScope => _serviceScope ??= Services.CreateScope();

    public IServiceProvider Services => WorkerService.Host.Services;

    private TestWorkerService? _workerService;

    public TestWorkerService WorkerService =>
        _workerService ??=
            TryHostAppBuilder(out _workerService) || TryHostBuilder(out _workerService)
                ? _workerService
                : throw new Exception("No valid host has been provided.");
}