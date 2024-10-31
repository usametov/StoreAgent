using System.Runtime.CompilerServices;
using StoreAgent;

public sealed class Program {
    public static void Main(string[] args) {
        var dispatcher = new Dispatcher();
        dispatcher.Init();
        var story = dispatcher.TestOpenAI("tell me a bedtime story");
        Console.WriteLine(story);
    }
}