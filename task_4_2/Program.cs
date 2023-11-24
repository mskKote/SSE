namespace task_4_2;

public static class Program
{
    public static void Main()
    {
        Task.Run(async () =>
        {
            const string serviceLocation =
                "http://vsr-demo.informatik.tu-chemnitz.de/webservices/SoapWebService/Service.asmx";
            var client = new AddServiceClient(serviceLocation);
            var answer = await client.Add(2, 3);

            // display result and wait for user
            Console.WriteLine("Result: " + answer);
        }).Wait();
    }
}