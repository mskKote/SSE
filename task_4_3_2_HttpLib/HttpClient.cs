// Course Material "Protokolle Verteilte Systeme"
// (c) 2008 by Professur Verteilte und Selbstorganisierende Rechnersysteme, TUC

using System.Net;

namespace task_4_3_2_HttpLib;

/// <summary>
///     Simple implementation of an HTTP client.
/// </summary>
public class HttpClient : TcpClient
{
    /// <summary>
    ///     Performs an HTTP operation.
    /// </summary>
    /// <param name="operation">The operation (e.g. "GET")</param>
    /// <param name="urlStr">The URL to perform the operation on.</param>
    /// <param name="content">The content passed within the request (if any).</param>
    /// <param name="headerParameters">Additional HTTP parameters to be included inside the header.</param>
    /// <returns>The HTTP response message.</returns>
    public HttpMessage Request(
        string operation,
        string urlStr,
        string content,
        Dictionary<string, string> headerParameters)
    {
        // parse url
        var url = new Url(urlStr);
        if (url.Scheme != "http")
            throw new Exception("The protocol scheme in " + urlStr + " is not supported. Only HTTP is supported.");

        // determine IP address
        var ip = Dns.GetHostAddresses(url.Host)[0].ToString();
        if (url.Host.ToLower().Equals("localhost"))
            ip = "127.0.0.1";

        // contruct HTTP message
        var requestMessage = new HttpMessage(operation, url.Host, url.Path + url.Query, headerParameters, content);

        // send query via TCP
        var resultMessageStr = Request(ip, url.Port, requestMessage.ToString());

        // parse HTTP message
        var resultMessage = new HttpMessage(resultMessageStr);

        return resultMessage;
    }

    /// <summary>
    ///     Performs an HTTP GET.
    /// </summary>
    /// <param name="urlStr">The URL of the resource</param>
    /// <returns></returns>
    public HttpMessage Get(string urlStr)
    {
        return Request(HttpMessage.Get, urlStr, "", null);
    }

    /// <summary>
    ///     Performs an HTTP POST.
    /// </summary>
    /// <param name="urlStr">The POST of the resource</param>
    /// <param name="contentType"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public HttpMessage Post(string urlStr, string contentType, string content)
    {
        var para = new Dictionary<string, string>
        {
            ["content-type"] = contentType
        };
        return Request(HttpMessage.Post, urlStr, content, para);
    }

    /// <summary>
    ///     Performs an HTTP DELETE.
    /// </summary>
    /// <param name="urlStr">The URL of the resource</param>
    /// <returns></returns>
    public HttpMessage Delete(string urlStr)
    {
        return Request(HttpMessage.Delete, urlStr, "", null);
    }
}