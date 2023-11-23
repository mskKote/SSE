using System.Globalization;
using System.Text.RegularExpressions;

namespace task_3_5;

/// <summary>
///     A class for generating and parsing HTTP-URIs.
/// </summary>
public class Url
{
    private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$-_.~";
    public string FragmentId = "";
    public string Host = "";
    public string Path = "";
    public int Port = 80;
    public string Query = "";

    public string Scheme = "";

    /// <summary>
    ///     Constructor for parsing URLs.
    /// </summary>
    public Url(string urlStr)
    {
        var match = Regex.Match(urlStr, @"
                ^(?:
                (?<scheme>[^:]*)
                \:\/\/(?<host>[^:^/^?^#]*)
                (?:\:(?<port>\d*))?
                )?
                (?<path>\/[^?^#]*)?
                (?:\?(?<query>[^#]*))?
                (?:\#(?<fragmentid>.*))?$            
            ", RegexOptions.IgnorePatternWhitespace);
        if (match.Success)
        {
            Scheme = match.Groups["scheme"].Value.ToLower();
            Host = Decode(match.Groups["host"].Value);
            if (match.Groups["port"].Value != "")
                Port = Convert.ToInt32(match.Groups["port"].Value);
            Path = match.Groups["path"].Value == "" ? "/" : Decode(match.Groups["path"].Value); // Updated
            Query = Decode(match.Groups["query"].Value);
            FragmentId = Decode(match.Groups["fragmentid"].Value);
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
        var url = Scheme + "://" + Host;
        if (Port != 80) url += ":" + Port;
        if (Path != "") url += "/" + Encode(Path.Substring(1));
        if (Query != "")
        {
            var queryParts = Query.Split('&');
            var qEncoded = "";
            foreach (var queryPart in queryParts)
            {
                var nameValue = queryPart.Split('=');
                qEncoded += "&" + Encode(nameValue[0]) + "=" + Encode(nameValue[1]);
            }

            url += "?" + qEncoded.Substring(1);
        }

        if (FragmentId != "") url += "#" + Encode(FragmentId);

        return url;
    }

    /// <summary>
    ///     Encodes any special characters in the URL with an escaping sequence.
    /// </summary>
    public static string Encode(string s)
    {
        var result = "";
        foreach (var c in s)
            if (ValidCharacters.Contains(c.ToString()))
                // allowed character
                result += c;
            else
                // character has to be encoded as "%" + HexDigit + HexDigit
                result += "%" + Convert.ToByte(c).ToString("X");

        return result;
    }

    /// <summary>
    ///     Decodes any escaping sequence in the URL with the corresponding characters.
    /// </summary>
    public static string Decode(string s)
    {
        while (s.Contains('%'))
        {
            var pos = s.IndexOf('%');
            var b = byte.Parse(s.Substring(pos + 1, 2), NumberStyles.HexNumber);
            s = s[..pos] + Convert.ToChar(b) + s.Substring(pos + 3);
        }

        return s;
    }
}