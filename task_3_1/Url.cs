using System.Text.RegularExpressions;

namespace task_3_1;

/// <summary>
///     A class for generating and parsing HTTP-URIs.
/// </summary>
public class Url
{
    private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$-_.~";
    public readonly string FragmentId;
    public readonly string Host;
    public readonly string Path;
    public readonly int Port;
    public readonly string Query;
    public readonly string Scheme;

    /// <summary>
    ///     Constructor for parsing URLs.
    /// </summary>
    public Url(string urlStr)
    {
        var urlPattern = $@"^(https?|ftp)://[{Regex.Escape(ValidCharacters)}]+(/[^\s]*)?$";
        var match = Regex.Match(urlStr, urlPattern, RegexOptions.IgnorePatternWhitespace);
        if (match.Success)
        {
            Scheme = match.Groups[1].Value;

            // HOST
            const string hostPattern = @"^(?:https?|ftp)://([^\/:?#]+)";
            Host = Regex.Match(urlStr, hostPattern).Groups[1].Value;

            // PORT
            const string portPattern = @"(?::(\d+))";
            var portCandidate = Regex.Match(urlStr, portPattern).Groups[1].Value;
            if (string.IsNullOrEmpty(portCandidate))
                Port = Scheme switch
                {
                    "http" => 80,
                    "https" => 443,
                    "ftp" => 21,
                    _ => -1
                };
            else Port = int.Parse(portCandidate);
            var urlWithoutHostAndPort =
                urlStr[(
                        Scheme.Length +
                        "://".Length +
                        Host.Length + (string.IsNullOrEmpty(portCandidate) ? 0 : ":".Length + portCandidate.Length))
                    ..];

            // PATH
            const string pathPattern = @"/([^?\s]*[^?\s%])?";
            var path = Regex.Match(urlWithoutHostAndPort, pathPattern).Value;
            Path = Decode(path);

            // QUERY
            var urlWithoutSchemeHostPortPath = urlWithoutHostAndPort[path.Length..];
            const string queryPattern = @"(?:\?([^#]*))?";
            Query = Regex.Match(urlWithoutSchemeHostPortPath, queryPattern).Groups[1].Value;

            // FRAGMENT
            const string fragmentIdPattern = "#(.*)";
            FragmentId = Regex.Match(urlWithoutSchemeHostPortPath, fragmentIdPattern).Groups[1].Value;
        }
        else
        {
            throw new FormatException("Could not parse URL: " + urlStr);
        }
    }

    /// <summary>
    ///     Constructor for building URLs from their components.
    /// </summary>
    public Url(string scheme, string host, int port, string path, string query, string fragmentId)
    {
        Scheme = scheme;
        Host = host;
        Port = port;
        Path = path;
        Query = query;
        FragmentId = fragmentId;
    }

    /// <summary>
    ///     Returns the string representation of the URL.
    /// </summary>
    public override string ToString()
    {
        var port = Port is 80 or 443 or 21 ? "" : $":{Port}";
        var path = string.IsNullOrEmpty(Path) ? "" : $"{Encode(Path)}";
        var fragment = string.IsNullOrEmpty(FragmentId) ? "" : $"#{FragmentId}";
        var query = string.IsNullOrEmpty(Query) ? "" : $"?{Query}";
        return $"{Scheme}://{Host}{port}{path}{query}{fragment}";
    }

    /// <summary>
    ///     Encodes any special characters in the URL with an escaping sequence.
    /// </summary>
    public static string Encode(string s)
    {
        return string.Join('/', s
            .Split('/')
            .Select(Uri.EscapeDataString)
        );
    }

    /// <summary>
    ///     Decodes any escaping sequence in the URL with the corresponding characters.
    /// </summary>
    public static string Decode(string s)
    {
        return string.Join('/', s
            .Split('/')
            .Select(Uri.UnescapeDataString)
        );
    }
}