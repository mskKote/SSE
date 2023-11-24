using System.Xml.Linq;

namespace task_4_3_1;

public static class SoapClient
{
    public static async Task<object> Invoke(
        string url,
        string ns,
        string operationName,
        Dictionary<string, object> parameters)
    {
        var content =
            "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body>";
        var parametersStr = string.Join("", parameters.Select(kv => $"<{kv.Key}>{kv.Value}</{kv.Key}>"));
        content += $"<{operationName} xmlns=\"{ns}\">{parametersStr}</{operationName}>";
        content += "</soap12:Body></soap12:Envelope>";

        // Send HTTP request
        var headers = new Dictionary<string, string>
        {
            { "content-type", "application/soap+xml; charset=utf-8" }
        };

        var answer = await HttpRequest.Post(url, content, headers);

        // Parse the SOAP response using XDocument
        var soapDocument = XDocument.Parse(answer.Content);
        XNamespace soapNamespace = "http://www.w3.org/2003/05/soap-envelope";

        // Find the <operation>Result element within the SOAP Body
        var response = soapDocument
            .Descendants(soapNamespace + "Body")
            .Elements()
            .Where(e => e.Name.LocalName == $"{operationName}Response")
            .Elements()
            .FirstOrDefault(e => e.Name.LocalName == $"{operationName}Result");

        return response.Value;
    }
}