namespace task_2_3;

public class Client
{
    private readonly Middleware _middleware;

    public Client(Middleware middleware)
    {
        _middleware = middleware;
    }

    public async Task Print(string address, double d)
    {
        var response = await _middleware.SendObjectTo(address, d);
        Console.WriteLine("Client: Received response: " + response);
    }

    public async Task Print(string address, string line)
    {
        var response = await _middleware.SendObjectTo(address, line);
        Console.WriteLine("Client: Received response: " + response);
    }

    public async Task Print(string address, string[] page)
    {
        var response = await _middleware.SendObjectTo(address, page);
        Console.WriteLine("Client: Received response: " + response);
    }


    #region RPC methods to call

    /// <summary>
    ///     Call server method Concat
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    public async Task<string> Concat(string arg1, string arg2, string arg3)
    {
        return await _middleware.CallFunction(
            "127.0.0.1:13000",
            nameof(Concat),
            new[] { arg1, arg2, arg3 });
    }

    /// <summary>
    ///     Call server method Substring
    /// </summary>
    /// <param name="s"></param>
    /// <param name="startingPosition"></param>
    /// <returns></returns>
    public async Task<string> Substring(string s, string startingPosition)
    {
        return await _middleware.CallFunction(
            "127.0.0.1:13000",
            nameof(Substring),
            new[] { s, startingPosition });
    }

    #endregion
}