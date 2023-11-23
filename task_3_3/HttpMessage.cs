using System.Text;
using System.Text.RegularExpressions;

namespace task_3_3;

public class HttpMessage
{
    public const string Get = "GET";
    public const string Post = "POST";
    public const string Put = "PUT";
    public const string Delete = "DELETE";
    public const string Head = "HEAD";
    public const string Options = "OPTIONS";
    public const string Trace = "TRACE";

    public string Content = "";
    public Dictionary<string, string> Headers = new();
    public string Host = "";

    public string Method = "";
    public string Resource = "";
    public string StatusCode = "";
    public string StatusMessage = "";

    /// <summary>
    ///     Construct an HTTP request.
    /// </summary>
    public HttpMessage(string method, string host, string resource, Dictionary<string, string> headers, string content)
    {
        Method = method;
        Host = host;
        Resource = resource;
        Headers = headers;
        if (Headers == null)
            Headers = new Dictionary<string, string>();
        Content = content;
        StatusCode = null;
        StatusMessage = null;
    }

    /// <summary>
    ///     Construct an HTTP response.
    /// </summary>
    public HttpMessage(string statusCode, string statusMessage, Dictionary<string, string> headers, string content)
    {
        Method = null;
        Host = null;
        Resource = null;
        Headers = headers;
        if (Headers == null)
            Headers = new Dictionary<string, string>();
        Content = content;
        StatusCode = statusCode;
        StatusMessage = statusMessage;
    }

    /// <summary>
    ///     Constructs an HTTP message by parsing a (received) string.
    /// </summary>
    /// <param name="message"></param>
    public HttpMessage(string message)
    {
        var messageParts = message.Split('\n');

        // Check if Request
        var messageFirstParts = messageParts[0].Split(' ');
        Method = messageFirstParts[0] switch
        {
            Get => Get,
            Post => Post,
            Put => Put,
            Head => Head,
            Options => Options,
            Trace => Trace,
            Delete => Delete,
            _ => null
        };

        var isRequest = Method != null;
        if (isRequest)
        {
            Resource = messageFirstParts[1];
        }
        else
        {
            StatusCode = messageFirstParts[1];
            StatusMessage = messageFirstParts[2];
        }

        // Headers
        // REQ: "POST /test HTTP/1.1\nHost: example.org\nContent-Length: 5\n\nhallo"
        // RES: "HTTP/1.1 200 OK\nContent-Type: text/html\nContent-Length: 12\n\nhello world\n"
        var headerPattern = new Regex(@"^(?<Key>[^:\r\n]+):\s*(?<Value>[^\r\n]*)$", RegexOptions.Multiline);

        foreach (Match match in headerPattern.Matches(message))
        {
            if (match.Groups["Key"].Value == "Host")
            {
                Host = match.Groups["Value"].Value;
                continue;
            }

            Headers[match.Groups["Key"].Value] = match.Groups["Value"].Value;
        }

        // Content
        var potentialContent = message.Split("\n\n");
        Content = potentialContent.Length == 2 ? potentialContent[1] : null;
    }

    /// <summary>
    ///     Returns the string representation of the message.
    /// </summary>
    public override string ToString()
    {
        var isRequest = Method != null;

        var sb = new StringBuilder(isRequest
            ? $"{Method} {Resource} HTTP/1.1\nHost: {Host}\n"
            : $"HTTP/1.1 {StatusCode} {StatusMessage}\n");

        foreach (var (key, value) in Headers)
            sb.AppendLine($"{key}: {value}");

        if (Content != null)
            sb.Append($"\n{Content}");

        return sb.ToString();
    }
}