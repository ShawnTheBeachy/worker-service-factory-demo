using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using WorkerTestDemo.Abstractions;
using WorkerTestDemo.Tests.WorkerFactory;

namespace WorkerTestDemo.Tests;

public sealed class MyWorkerFactory : WorkerServiceFactory<Program>
{
    protected override void ConfigureApplicationHost(IHostApplicationBuilder builder)
    {
        // Program.cs creates a HostApplicationBuilder (Host.CreateApplicationBuilder(args)),
        // so we override `ConfigureApplicationHost` rather than `ConfigureHost`.
        var testGreeter = Substitute.For<IGreeter>();
        builder.Services.AddSingleton(testGreeter);
    }
}