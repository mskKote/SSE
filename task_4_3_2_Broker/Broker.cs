using task_4_3_2_HttpLib;

namespace task_4_3_2_Broker;

internal class Broker : TcpServer
{
    private readonly List<Tuple<string, int>> _clientEndpoints = new()
    {
        new Tuple<string, int>("127.0.0.1", 15001),
        new Tuple<string, int>("127.0.0.1", 15002)
    };

    protected override string ReceiveMessage(string messagePlain, string endpoint)
    {
        // Simulate interrupt
        var rand = new Random().Next(0, 2);
        if (rand != 0)
        {
            // Construct SOAP message with appropriate headers
            const string busyAcknowledgement =
                "<env:Envelope xmlns:env=\"http://www.w3.org/2001/09/soap-envelope\">" +
                "<env:Header>" +
                "<n:StatusRequest xmlns:n=\"http://example.org/status\" env:mustUnderstand=\"true\">BUSY</n:StatusRequest>" +
                "</env:Header><env:Body></env:Body>" +
                "</env:Envelope>";

            return new HttpMessage(
                "200",
                "OK",
                new Dictionary<string, string>(),
                busyAcknowledgement).ToString();
        }

        Console.WriteLine("=============\nBroker: received server update: \n" + messagePlain + "\n=============\n");

        messagePlain = messagePlain.Replace(
            "<n:StatusRequest xmlns:n=\"http://example.org/status\" env:mustUnderstand=\"true\"/>",
            "");

        var message = new HttpMessage(messagePlain);
        // Forward message to clientEndpoints             
        foreach (var clientEndpoint in _clientEndpoints)
        {
            var client = new TcpClient();
            client.Request(clientEndpoint.Item1, clientEndpoint.Item2, message.Content);
        }


        // Construct SOAP message with appropriate headers
        const string receivedAcknowledgement =
            "<env:Envelope xmlns:env=\"http://www.w3.org/2001/09/soap-envelope\">" +
            "<env:Header>" +
            "<n:StatusRequest xmlns:n=\"http://example.org/status\" env:mustUnderstand=\"true\">RECEIVED</n:StatusRequest>" +
            "</env:Header><env:Body></env:Body>" +
            "</env:Envelope>";

        return new HttpMessage(
            "200",
            "OK",
            new Dictionary<string, string>(),
            receivedAcknowledgement).ToString();
    }

    private static void Main()
    {
        var server = new Broker();
        Console.WriteLine("Broker started...");
        server.Run("127.0.0.1", 14000);
    }
}