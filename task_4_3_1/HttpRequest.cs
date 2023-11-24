using System.Net;
using System.Net.Sockets;

namespace task_4_3_1;

public class HttpRequest
{
    private HttpRequest()
    {
    }

    public static Task<HttpMessage> Get(string url)
    {
        return Request(HttpMessage.Get, new Url(url), null, new Dictionary<string, string>());
    }

    public static Task<HttpMessage> Post(string url, string content, Dictionary<string, string> headers)
    {
        return Request(HttpMessage.Post, new Url(url), content, headers);
    }

    private static async Task<HttpMessage> Request(string method, Url url, string content,
        Dictionary<string, string> headers)
    {
        // resolve DNS
        var addrs = await Dns.GetHostAddressesAsync(url.Host);

        // extract a IPv4 from the list of available addresses (since tu-chemnitz.de
        // is not available through IPv6 yet)
        // a more general approach would be to try a request to each address
        // in the list in a return the result from the first successfull request
        IPAddress ipv4 = null;
        foreach (var addr in addrs)
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                ipv4 = addr;
                break;
            }

        // if there is no IPv4 in the list of addresses for the given host
        if (ipv4 == null) throw new ArgumentException("Cannot resolve IPv4 for host: " + url.Host);

        // construct HTTP request
        var request = new HttpMessage(method, url.Host, url.Path + "?" + url.Query, headers, content);

        // send TCP request
        var resp = await TcpRequest.Do(ipv4, url.Port, request.ToString());
        return new HttpMessage(resp);
    }
}