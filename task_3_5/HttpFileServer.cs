using System.Reflection;

namespace task_3_5;

public class HttpFileServer : TcpServer
{
    // Take the folder DocumentRoot within the project
    private static readonly string DocumentRoot =
        Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) + "/../../../DocumentRoot";

    /// <summary>
    ///     Buffer for HTTP message.
    /// </summary>
    protected string Buffer = "";

    /// <summary>
    ///     Handles an incoming line of text.
    /// </summary>
    /// <param name="line">Incoming data.</param>
    /// <returns>The answer to be sent back as a reaction to the received line or null.</returns>
    protected override string ReceiveLine(string line)
    {
        Buffer += line + "\r\n";
        if (line != "") return null;

        // parse message
        var request = new HttpMessage(Buffer);

        // build answer message
        var answer = ReceiveRequest(request);

        // send answer message
        Console.WriteLine("HTTP: Sending answer.");
        return answer.ToString();
    }

    /// <summary>
    ///     Handle an incoming HTTP request.
    /// </summary>
    /// <param name="request">Incoming request.</param>
    /// <returns>The answer message to be sent back.</returns>
    protected virtual HttpMessage ReceiveRequest(HttpMessage request)
    {
        // check HTTP method
        if (request.Method == HttpMessage.Post)
            return new HttpMessage("201", "Created", new Dictionary<string, string>(), "");

        // look for requested file
        if (!File.Exists(DocumentRoot + "/index.html"))
            return new HttpMessage("404", "Not Found", new Dictionary<string, string>(), "");

        // create answer message                        
        var content = File.ReadAllText(DocumentRoot + "/index.html");

        var answerMessage = new HttpMessage("200", "OK", new Dictionary<string, string>
        {
            { "Content-Type", "text/html" },
            // { "Content-Length", $"{content.Length}" }
        }, content);

        return answerMessage;
    }
}