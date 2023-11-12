namespace task_2_3;

internal static class Program
{
    private static void Main()
    {
        Task.Run(async () =>
        {
            var server = new Server(new Middleware());
            server.Start("127.0.0.1", 13000);

            var client = new Client(new Middleware());
            await client.Print("127.0.0.1:13000", 2.25);
            await client.Print("127.0.0.1:13000", "hello");
            await client.Print("127.0.0.1:13000", new[] { "a", "b", "c" });

            Console.WriteLine("Performing RPC...");
            Console.WriteLine(await client.Concat("a", "b", "c"));
            Console.WriteLine(await client.Substring("hello world", "6"));
            server.Stop();
        }).Wait();
    }
}