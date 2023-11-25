using System.Xml.Linq;
using System.Xml.XPath;
using task_4_3_2_HttpLib;

namespace task_4_3_2_Client2;

internal class Client2 : TcpServer
{
    protected override string ReceiveMessage(string message, string endpoint)
    {
        var doc = XDocument.Parse(message);
        // detect encryption

        // Define the SOAP and WS-Security namespaces
        XNamespace envNamespace = "http://www.w3.org/2001/09/soap-envelope";
        XNamespace wsseNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        // Find the IsSecured element within the WS-Security header
        var isSecuredElement = doc
            .Descendants(envNamespace + "Header")
            .Elements(wsseNamespace + "Security")
            .Elements()
            .FirstOrDefault(e => e.Name.LocalName == "IsSecured");

        bool.TryParse(isSecuredElement?.Value, out var encrypted);
        if (encrypted)
        {
            Console.WriteLine("Client 2: ignoring encrypted request");
            return "";
        }

        var company = doc.XPathSelectElement("//*[1]/*[2]/*[1]/*[1]").Value;
        var price = doc.XPathSelectElement("//*[1]/*[2]/*[1]/*[2]").Value;

        Console.WriteLine("Client 2: incoming update: {0},{1}",
            company,
            price);
        return "";
    }

    private static void Main()
    {
        var server = new Client2();
        Console.WriteLine("Client2 started...");
        server.Run("127.0.0.1", 15002);
    }
}