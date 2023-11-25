using System.Xml.Linq;
using System.Xml.XPath;
using task_4_3_2_HttpLib;

namespace task_4_3_2_Client1;

internal class Client1 : TcpServer
{
    protected override string ReceiveMessage(string message, string endpoint)
    {
        var doc = XDocument.Parse(message);
        string company;
        string price;

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
            var key = doc
                .Descendants(envNamespace + "Header")
                .Elements(wsseNamespace + "Security")
                .Elements()
                .FirstOrDefault(e => e.Name.LocalName == "EncryptedKey")
                .Value;

            var body = doc.Descendants(envNamespace + "Body").FirstOrDefault().Value;
            var bodyDecrypted = Encryption.Decrypt(body, key);

            // Retrieve values from Company and Price elements
            var bodyElement = XElement.Parse(bodyDecrypted);
            company = (string)bodyElement.Element("{http://example.org}Company");
            price = (string)bodyElement.Element("{http://example.org}Price");
        }
        else
        {
            company = doc.XPathSelectElement("//*[1]/*[2]/*[1]/*[1]").Value;
            price = doc.XPathSelectElement("//*[1]/*[2]/*[1]/*[2]").Value;
        }

        Console.WriteLine("Client 1: incoming {2}update: {0},{1}",
            company,
            price,
            encrypted ? "encrypted " : "");
        return "";
    }

    private static void Main()
    {
        var server = new Client1();
        Console.WriteLine("Client1 started...");
        server.Run("127.0.0.1", 15001);
    }
}