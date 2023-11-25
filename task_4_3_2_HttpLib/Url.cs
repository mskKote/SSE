// Course Material "Protokolle Verteilte Systeme"
// (c) 2008 by Professur Verteilte und Selbstorganisierende Rechnersysteme, TUC

using System.Globalization;
using System.Text.RegularExpressions;

namespace task_4_3_2_HttpLib;

/// <summary>
///     A class for generating and parsing HTTP-URIs.
/// </summary>
public class Url
{
    private const string ValidCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$-_@.&+-!*\"'(),/#?:";

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
        urlStr = Decode(urlStr);

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
            Host = match.Groups["host"].Value;
            if (match.Groups["port"].Value != "")
                Port = Convert.ToInt32(match.Groups["port"].Value);
            Path = match.Groups["path"].Value;
            Query = match.Groups["query"].Value;
            FragmentId = match.Groups["fragmentid"].Value;
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
        if (Path != "") url += Path;
        if (Query != "") url += "?" + Query;
        if (FragmentId != "") url += "#" + FragmentId;

        return Encode(url);
    }

    /// <summary>
    ///     Encodes any special characters in the URL with an escaping sequence.
    /// </summary>
    public static string Encode(string url)
    {
        var result = "";
        foreach (var c in url)
            if (ValidCharacters.Contains("" + c))
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
    public static string Decode(string url)
    {
        while (url.Contains('%'))
        {
            var pos = url.IndexOf('%');
            var b = byte.Parse(url.Substring(pos + 1, 2), NumberStyles.HexNumber);
            url = url[..pos] + Convert.ToChar(b) + url[(pos + 3)..];
        }

        return url;
    }
}