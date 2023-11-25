using System.Xml.Linq;
using task_4_3_2_HttpLib;
using HttpClient = task_4_3_2_HttpLib.HttpClient;

namespace task_4_3_2_Server;

internal class Server : TcpServer
{
    private const string BrokerUrl = "http://127.0.0.1:14000/";

    protected override string ReceiveMessage(string message, string endpoint)
    {
        Console.WriteLine("Server: incoming message: " + message);
        return "";
    }

    private static void Main()
    {
        Console.WriteLine("Press any key to start the server...");
        Console.ReadKey();
        var server = new Server();
        server.SendStockUpdate("FB", "35.5");
        server.SendStockUpdate("TW", "30.0", true);
        server.SendStockUpdate("LI", "15.0", true);
        server.Run("127.0.0.1", 13000);
    }


    private void SendStockUpdate(string company, string price, bool encrypted = false)
    {
        var client = new HttpClient();
        var tries = 0;
        bool clientAcknowledged;
        do
        {
            // signal encrypted content in SOAP headers
            var headerForEncrypted = encrypted
                ? """
                  <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
                      <IsSecured>True</IsSecured>
                      <wsse:EncryptedKey>veryStrongKey</wsse:EncryptedKey>
                  </wsse:Security>
                  """
                : """
                  <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
                      <IsSecured>False</IsSecured>
                  </wsse:Security>
                  """;

            var content =
                "<env:Envelope xmlns:env=\"http://www.w3.org/2001/09/soap-envelope\">" +
                "<env:Header>" +
                headerForEncrypted +
                "<n:StatusRequest xmlns:n=\"http://example.org/status\" env:mustUnderstand=\"true\"/>" +
                "</env:Header><env:Body>" +
                CreateSoapBody(company, price, encrypted) +
                "</env:Body></env:Envelope>";
            var response = client.Post(BrokerUrl, "application/soap+xml", content);
            clientAcknowledged = CheckIfClientAcknowledgedReceipt(response.Content);
            if (!clientAcknowledged)
                Console.WriteLine("Server: broker is busy. repeating transmission...");
            tries++;
        } while (tries < 15 && !clientAcknowledged);
    }

    private string CreateSoapBody(string company, string price, bool encrypted)
    {
        var body =
            "<ex:StockUpdate xmlns:ex=\"http://example.org\">" +
            $"<ex:Company>{company}</ex:Company>" +
            $"<ex:Price>{price}</ex:Price>" +
            "</ex:StockUpdate>";

        return encrypted
            ? Encryption.Encrypt(body, "veryStrongKey")
            : body;
    }

    private bool CheckIfClientAcknowledgedReceipt(string response)
    {
        var soapDocument = XDocument.Parse(response);
        XNamespace envNamespace = "http://www.w3.org/2001/09/soap-envelope";
        var statusRequestElement = soapDocument
            .Descendants(envNamespace + "Header")
            .Elements()
            .FirstOrDefault(e => e.Name.LocalName == "StatusRequest");

        return statusRequestElement?.Value == "RECEIVED";
    }
}