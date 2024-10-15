using WorkerTestDemo.Abstractions;

namespace WorkerTestDemo;

public class Worker : BackgroundService
{
    private readonly IGreeter _greeter;

    public Worker(IGreeter greeter)
    {
        _greeter = greeter;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _greeter.Greet("Frodo");
        return Task.CompletedTask;
    }
}