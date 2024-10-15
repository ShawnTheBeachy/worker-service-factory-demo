using WorkerTestDemo.Abstractions;

namespace WorkerTestDemo;

internal sealed class Greeter : IGreeter
{
    public void Greet(string name) =>
        Console.WriteLine($"Greetings from the program, {name}!");
}