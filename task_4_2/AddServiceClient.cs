using System.Xml.Linq;

namespace task_4_2;

public class AddServiceClient
{
    private readonly string _serviceLocation;

    public AddServiceClient(string serviceLocation)
    {
        _serviceLocation = serviceLocation;
    }

    /// <summary>
    ///     Sends a SOAP request via HTTP to a Web service endpoint.
    /// </summary>
    public async Task<int> Add(int a, int b)
    {
        var content = $"""
                       <?xml version="1.0" encoding="utf-8"?>
                                   <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                                     xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                                     xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                                     <soap12:Body>
                                       <Add xmlns="http://vsr-demo.informatik.tu-chemnitz.de/webservices/SoapWebService/">
                                         <a>{a}</a>
                                         <b>{b}</b>
                                       </Add>
                                     </soap12:Body>
                                   </soap12:Envelope>
                       """;
        var headers = new Dictionary<string, string>
        {
            { "content-type", "application/soap+xml; charset=utf-8" }
            // { "Host", "vsr-demo.informatik.tu-chemnitz.de" },
            // { "Content-Length", $"{content.Length}" }
        };

        var answer = await HttpRequest.Post(_serviceLocation, content, headers);

        // Parse the SOAP response using XDocument
        var soapDocument = XDocument.Parse(answer.Content);

        // Define the SOAP namespace
        XNamespace soapNamespace = "http://www.w3.org/2003/05/soap-envelope";

        // Find the AddResult element within the SOAP Body
        var addResultElement = soapDocument
            .Descendants(soapNamespace + "Body")
            .Elements()
            .Where(e => e.Name.LocalName == "AddResponse")
            .Elements()
            .FirstOrDefault(e => e.Name.LocalName == "AddResult");

        return int.Parse(addResultElement.Value);
    }
}