namespace task_2_3;

public class Server
{
    private readonly Middleware _middleware;

    public Server(Middleware middleware)
    {
        _middleware = middleware;
    }

    private string IncomingConnection(object request)
    {
        // Check if the incoming object is a RPC,
        if (request is string[] strings &&
            strings[0] == Middleware.RpcPrefix)
        {
            var methodName = strings[1];
            var args = strings[2]
                .Split(Middleware.RpcDelimiter)
                .Select(x => x.Replace('\t', ' '))
                .ToArray();

            var publicMethods = typeof(Server).GetMethods()
                .Where(method => method.IsDefined(typeof(CallableAttribute), false) &&
                                 method.Name == methodName &&
                                 method.GetParameters().Length == args.Length);

            // invoke the requested method using Reflection
            foreach (var method in publicMethods)
                return method.Invoke(this, args) as string;
        }

        return PrintToConsole(request);
    }

    private string PrintToConsole(object request)
    {
        if (request is double)
        {
            Console.WriteLine("Server: Printing double: " + double.Parse(request.ToString()).ToString("0#.##0"));
            return "Double printed";
        }

        if (request is string)
        {
            Console.WriteLine("Server: Printing string: <<" + request + ">> (length " + request.ToString().Length +
                              ")");
            return "String printed";
        }

        if (request is string[] strings)
        {
            var output = "Server: Printing string array: [" + string.Join(" - ", strings) +
                         "] (number of elements: " + strings.Length + ")";
            Console.WriteLine(output);
            return "String array printed";
        }

        throw new ArgumentException("Can not print the object " + request);
    }

    public void Start(string ip, int port)
    {
        _middleware.StartServer(ip, port, IncomingConnection);
    }

    public void Stop()
    {
        _middleware.StopServer();
    }

    #region RPC procedures

    [Callable]
    public string Concat(string arg1, string arg2, string arg3)
    {
        return arg1 + arg2 + arg3;
    }

    [Callable]
    public string Substring(string s, string startingPosition)
    {
        return s.Substring(int.Parse(startingPosition));
    }

    #endregion
}