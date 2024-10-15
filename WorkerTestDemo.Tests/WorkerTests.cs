using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using WorkerTestDemo.Abstractions;

namespace WorkerTestDemo.Tests;

public sealed class WorkerTests : IClassFixture<MyWorkerFactory>
{
    private readonly MyWorkerFactory _workerFactory;

    public WorkerTests(MyWorkerFactory workerFactory)
    {
        _workerFactory = workerFactory;
    }

    [Fact]
    public void Worker_ShouldCallGreeter_WhenItIsStarted()
    {
        var greeter = _workerFactory.Services.GetRequiredService<IGreeter>();
        greeter.Received(1)
            .Greet("Frodo");
    }
}