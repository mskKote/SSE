// Course Material "Protokolle Verteilte Systeme"
// (c) 2008 by Professur Verteilte und Selbstorganisierende Rechnersysteme, TUC

using System.Globalization;
using System.Text.RegularExpressions;

namespace task_4_3_2_HttpLib;

/// <summary>
///     A class for generating and parsing HTTP 1.1 messages.
/// </summary>
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
    ///     Constructs an HTTP message by parsing a (received) string.
    /// </summary>
    /// <param name="message"></param>
    public HttpMessage(string message)
    {
        var firstTime = true;

        // loop through lines in message
        while (message.Contains('\n'))
        {
            // separate first line from rest of messge
            var line = message[..message.IndexOf('\n')].Trim();
            message = message[(message.IndexOf('\n') + 1)..];

            if (firstTime)
            {
                // parse first line 
                if (line.StartsWith("HTTP"))
                {
                    // response message                        
                    var match = Regex.Match(line, @"^HTTP\/1\.1\s(\S*)\s(.*)$");
                    // extract important Headers
                    if (match.Groups.Count == 3)
                    {
                        StatusCode = match.Groups[1].Value.Trim();
                        StatusMessage = match.Groups[2].Value.Trim();
                    }
                    else
                    {
                        throw new FormatException("Could not parse HTTP message, the first line is misformed: " + line);
                    }
                }
                else
                {
                    // request message
                    var match = Regex.Match(line, @"^(\S*)\s(\S*)\sHTTP\/1\.1$");
                    // extract important Headers
                    if (match.Groups.Count == 3)
                    {
                        Method = match.Groups[1].Value.Trim();
                        Resource = match.Groups[2].Value.Trim();
                    }
                    else
                    {
                        throw new FormatException("Could not parse HTTP message, the first line is misformed: " + line);
                    }
                }
            }
            else if (line.Equals(""))
            {
                // empty line -> end of header; the rest is content
                Content = message;
                message = "";
            }
            else
            {
                // parse name-value-pairs
                var pos = line.IndexOf(':');
                if (pos == -1)
                    throw new FormatException("Could not parse HTTP message, invalid header line: " + line);
                var name = line[..pos].ToLower().Trim();
                var value = line[(pos + 1)..].Trim();

                if (name.Equals("host"))
                    // store host parameter in extra variable
                    Host = value;
                else
                    // store all other Headers
                    Headers[name] = value;
            }

            firstTime = false;
        }

        // handle chunked encoding
        if (!Headers.ContainsKey("transfer-encoding")) return;
        var encoding = Headers["transfer-encoding"];
        int length;
        if (encoding == null || !encoding.Contains("chunked")) return;
        var chunked = Content;
        Content = "";
        do
        {
            var lengthStr = chunked[..chunked.IndexOf('\n')].Trim();
            length = int.Parse(lengthStr, NumberStyles.AllowHexSpecifier);
            Content += chunked.Substring(chunked.IndexOf('\n') + 1, length);
            chunked = chunked[(chunked.IndexOf('\n') + 3 + length)..];
        } while (length > 0);
    }

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
    ///     Returns the string representation of the message.
    /// </summary>
    public override string ToString()
    {
        // automatically set content length
        Headers["content-length"] = "" + Content.Length;

        // build obligatory part
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
        Headers["set-cookie"] = name + "=" + value;
    }

    /// <summary>
    ///     Returns a list of cookie parts inside the HTTP message.
    /// </summary>
    public Dictionary<string, string> GetCookies()
    {
        // access HTTP header parameter
        var cookies = new Dictionary<string, string>();
        if (!Headers.ContainsKey("cookie")) return cookies;

        // split into individual cookie parts
        var parts = Headers["cookie"].Split(new char[1] { ';' });
        foreach (var part in parts)
        {
            // parse name and value
            var middle = part.IndexOf('=');
            if (middle == -1) continue;
            var name = part[..middle].Trim();
            var value = part[(middle + 1)..].Trim();
            cookies[name] = value;
        }

        return cookies;
    }
}