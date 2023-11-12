using System.Globalization;

namespace task_2_3;

public sealed class Middleware
{
    public const string RpcDelimiter = ";";
    public const string RpcPrefix = "RPC";
    private Func<object, string> _serverCallback;
    private Task _serverTask;
    private CancellationTokenSource _ts;

    #region CLIENT

    /// <summary>
    ///     Serialize the parameter
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string Marshall(object input)
    {
        if (input is string[] strings)
            return string.Join(' ', strings);

        return input.ToString() ?? string.Empty;
    }

    public async Task<string> SendObjectTo(string address, object input)
    {
        var ip = address.Split(':')[0];
        var port = int.Parse(address.Split(':')[1]);

        var payload = Marshall(input);

        Console.WriteLine("\n\r  Middleware: Transferring payload '{0}' to {1}\n\r", payload, address);

        var answer = await TcpRequest.Do(ip, port, payload);
        // deleting all \0 of buffer for printing
        return answer[..answer.IndexOf("\0", StringComparison.Ordinal)];
    }

    /// <summary>
    ///     Contact server and pass procedure call
    /// </summary>
    /// <param name="address"></param>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public async Task<string> CallFunction(string address, string name, string[] args)
    {
        var ip = address.Split(':')[0];
        var port = int.Parse(address.Split(':')[1]);
        var payload = RpcPrefix +
                      ' ' + name +
                      ' ' + string.Join(RpcDelimiter, args).Replace(' ', '\t');

        Console.WriteLine("\n\r  Middleware: Calling RPC '{0}' to {1} with '{2}'\n\r", name, address, payload);

        var answer = await TcpRequest.Do(ip, port, payload);
        // deleting all \0 of buffer for printing
        return string.Join(' ', answer[..answer.IndexOf("\0", StringComparison.Ordinal)]);
    }

    #endregion

    #region SERVER

    /// <summary>
    ///     Deserialize the parameter
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private object DeMarshall(string line)
    {
        if (double.TryParse(line, out var result))
            return result.ToString(CultureInfo.InvariantCulture);

        var parts = line.Split(' ');
        if (parts.Length > 1)
            return parts;

        return line;
    }

    private string ProcessIncomingRequest(string line)
    {
        line = line[..line.IndexOf("\0", StringComparison.Ordinal)]; // deleting all \0 of buffer for printing
        Console.WriteLine("\n\r  Middleware: Received payload '{0}'\n\r", line);
        var answer = DeMarshall(line);
        return _serverCallback(answer);
    }

    public void StartServer(
        string ip,
        int port,
        Func<object, string> callback)
    {
        _serverCallback = callback;
        _ts = new CancellationTokenSource();
        _serverTask = TcpServer.Start(ip, port, _ts, ProcessIncomingRequest);
    }

    public void StopServer()
    {
        _ts.Cancel();
        _serverTask.Wait();
    }

    #endregion
}