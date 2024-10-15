using Microsoft.Extensions.Hosting;

namespace WorkerTestDemo.Tests.WorkerFactory;

public sealed class TestWorkerService : IDisposable
{
    internal readonly IHost Host;

    internal TestWorkerService(
        HostApplicationBuilder builder,
        Action<IHostApplicationBuilder> configure
    )
    {
        configure(builder);
        Host = builder.Build();
        Host.StartAsync().GetAwaiter().GetResult();
    }

    internal TestWorkerService(IHostBuilder builder, Action<IHostBuilder> configure)
    {
        configure(builder);
        Host = builder.Build();
        Host.StartAsync().GetAwaiter().GetResult();
    }

    public void Dispose() => Host.Dispose();
}