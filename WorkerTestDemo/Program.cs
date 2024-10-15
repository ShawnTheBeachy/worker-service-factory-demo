using WorkerTestDemo;
using WorkerTestDemo.Abstractions;

var builder = CreateHostBuilder(new HostApplicationBuilderSettings { Args = args });
// We inject the greeter outside of `CreateHostBuilder` because
// we will want to inject a mocked version in our tests.
builder.Services.AddSingleton<IGreeter, Greeter>();

var host = builder.Build();
host.Run();

public sealed partial class Program
{
    private static HostApplicationBuilder CreateHostBuilder(HostApplicationBuilderSettings settings)
    {
        var builder = Host.CreateApplicationBuilder(settings);
        builder.Services.AddHostedService<Worker>();
        return builder;
    }
}
