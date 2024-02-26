namespace task_4_1;

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
        // loop through lines in message
        var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var firstLine = lines.First();

        // parse first line
        var parts = firstLine.Split(' ');
        if (parts.Length != 3) throw new FormatException("Malformed HTTP message: " + firstLine);

        if (parts[0] == "HTTP/1.0" || parts[0] == "HTTP/1.1")
        {
            // is response
            StatusCode = parts[1];
            StatusMessage = parts[2];
        }
        else
        {
            // is request
            Method = parts[0];
            Resource = parts[1];
        }

        // parse other lines
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line == "")
            {
                // empty line -> end of header; the rest is content
                Content = string.Join("\n", lines.Skip(i + 1));
                break;
            }

            // parse header (name-value-pair)
            var colonAt = line.IndexOf(':');
            if (colonAt == -1) throw new FormatException("Malformed header: " + line);

            var name = line[..colonAt].ToLower().Trim();
            var value = line[(colonAt + 1)..].Trim();

            if (name == "cookie")
                foreach (var cookie in value.Split(' '))
                {
                    var keyValue = cookie.Split('=');
                    SetCookie(keyValue[0], keyValue[1]);
                }

            if (name == "host")
                Host = value;
            else
                Headers[name] = value;
        }
    }

    /// <summary>
    ///     Returns the string representation of the message.
    /// </summary>
    public override string ToString()
    {
        // set content length
        Headers["content-length"] = Content.Length.ToString();

        // build first line
        string message;
        if (Method != null)
            message = Method + " " + Resource + " HTTP/1.1\nhost: " + Host + "\n";
        else
            message = "HTTP/1.1 " + StatusCode + " " + StatusMessage + "\n";

        // add Headers
        foreach (var name in Headers.Keys)
            message += name + ": " + Headers[name] + "\n";

        // add content
        message += "\n" + Content;

        return message;
    }

    /// <summary>
    ///     Sets the value of a cookie inside an HTTP Message.
    /// </summary>
    public void SetCookie(string name, string value)
    {
        var hasCookieHeader = Headers.TryGetValue("Set-Cookie", out var cookie);
        if (hasCookieHeader == false) Headers.Add("Set-Cookie", $"{name}={value}");
        else Headers["Set-Cookie"] = cookie + $" {name}={value}";
    }

    /// <summary>
    ///     Returns a list of cookie parts inside the HTTP message.
    /// </summary>
    public Dictionary<string, string> GetCookies()
    {
        var cookiesDict = new Dictionary<string, string>();
        var cookiesExists = Headers.TryGetValue("Cookie", out var cookies);
        if (cookiesExists == false) return cookiesDict;

        var cookieKeyValue = cookies?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (cookieKeyValue == null) return cookiesDict;

        foreach (var cookie in cookieKeyValue)
        {
            var keyValue = cookie.Split('=');
            cookiesDict[keyValue[0]] = keyValue[1];
        }

        return cookiesDict;
    }
}