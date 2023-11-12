using System.Net.Sockets;
using System.Text;

namespace task_2_3;

public static class TcpRequest
{
    public static async Task<string> Do(string ip, int port, string message)
    {
        var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var r = new StreamReader(client.GetStream(), Encoding.ASCII);
        await using var w = new StreamWriter(client.GetStream(), Encoding.ASCII);
        await w.WriteAsync(message);
        await w.FlushAsync();

        var buffer = new char[4096];
        var byteCount = await r.ReadAsync(buffer, 0, buffer.Length);
        return new string(buffer);
    }
}