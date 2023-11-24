namespace task_4_1;

public class HttpServer : TcpServer
{
    /// <summary>
    ///     Handles an incoming line of text.
    /// </summary>
    /// <param name="msg">Incoming data</param>
    /// <returns>The answer to be sent back as a reaction to the received line or null.</returns>
    protected override string HandleRequest(string msg)
    {
        // parse message
        var request = new HttpMessage(msg);

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
        // parse relative URL in request
        var requestUrl = new Url(request.Resource);

        // not found
        if (requestUrl.Path != "/" || request.Method != HttpMessage.Get)
            return new HttpMessage("404", "Not Found", null,
                "<html><body>The requested file could not be found.</body></html>");

        // simple hardcoded HTML response
        // read cookies, count up cookie and server content accordingly
        var reqCookies = request.GetCookies();
        var response = new HttpMessage("200", "Ok", null,
            $"<html><body>This is your {reqCookies.Count}. visit to this site.<hr /></body></html>");

        return response;
    }
}