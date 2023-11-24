namespace task_4_1;

internal class Program
{
    private static void Main()
    {
        Task.Run(async () =>
        {
            const string host = "127.0.0.1";
            const int port = 3000;

            var server = new HttpServer();

            Console.WriteLine($"Listening on http://{host}:{port}");
            Console.WriteLine("Stop server with CTRL+C");
            await server.Start(host, port);
        }).Wait();
    }
}