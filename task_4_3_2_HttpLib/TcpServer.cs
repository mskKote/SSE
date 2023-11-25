// Course Material "Protokolle Verteilte Systeme"
// (c) 2008 by Professur Verteilte und Selbstorganisierende Rechnersysteme, TUC

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace task_4_3_2_HttpLib;

/// <summary>
///     Simple implementation of a TCP server that uses threads to manage concurrent requests.
/// </summary>
public abstract class TcpServer
{
    /// <summary>
    ///     The socket used for handling an established connection.
    /// </summary>
    protected Socket connectionSocket;

    /// <summary>
    ///     Sends the specified data back via the connectionSocket.
    /// </summary>
    protected void SendString(string message)
    {
        var sendBuffer = Encoding.UTF8.GetBytes(message);
        connectionSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
        //Console.WriteLine("TCP: Sent answer message, {0} bytes to {1}.", sendBuffer.Length,
        //  connectionSocket.RemoteEndPoint.ToString());
    }

    /// <summary>
    ///     Shuts down the connectionSocket.
    /// </summary>
    protected void CloseCurrentConnection()
    {
        //Console.WriteLine("TCP: Shutting down connection with {0} in both directions.",
        //    connectionSocket.RemoteEndPoint.ToString());

        // shut down
        connectionSocket.Shutdown(SocketShutdown.Both);
        connectionSocket.Close();
    }

    /// <summary>
    ///     Waits for bytes arriving on connectionSocket and handles them.
    /// </summary>
    protected void ReceiveBytes()
    {
        //Console.WriteLine("TCP: Waiting for bytes from {0} in thread {1}.",
        //    connectionSocket.RemoteEndPoint.ToString(), Thread.CurrentThread.ManagedThreadId);

        var request = "";

        while (true)
        {
            // receive chunk of bytes
            var receiveBuffer = new byte[10000];
            var receivedBytes = connectionSocket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);
            //Console.WriteLine("TCP: Received {0} bytes from {1}.",
            //    receivedBytes, connectionSocket.RemoteEndPoint.ToString());

            // add received bytes to input buffer            
            request += Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);

            var answer = ReceiveMessage(request, connectionSocket.RemoteEndPoint.ToString());

            SendString(answer);
            CloseCurrentConnection();
            return;
        }
    }

    /// <summary>
    ///     Starts the TCP server, which keeps on running until the program is aborted.
    /// </summary>
    /// <param name="ip">The IP address to listen tp.</param>
    /// <param name="port">The port to listen to.</param>
    public void Run(string ip, int port)
    {
        Console.WriteLine("TCP: Starting server.");

        // create listening socket                        
        var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Console.WriteLine("TCP: Created listening socket with handle {0}.", listeningSocket.Handle.ToString());

        // bind 
        var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        listeningSocket.Bind(endpoint);
        Console.WriteLine("TCP: Bound to {0}.", endpoint);

        // listen
        listeningSocket.Listen(10);
        //Console.WriteLine("TCP: Start listening in Thread {0}.", Thread.CurrentThread.ManagedThreadId);
        //Console.WriteLine();

        // accept loop 
        while (true)
        {
            // accept new connection
            var newSocket = listeningSocket.Accept();
            //Console.WriteLine("TCP: Connection established with {0} over socket handle {1}.",
            //   newSocket.RemoteEndPoint.ToString(), newSocket.Handle);

            // create a new instance of the current server class
            var newServerInstance = MemberwiseClone() as TcpServer;
            newServerInstance.connectionSocket = newSocket;

            // start a new thread
            var newThread = new Thread(newServerInstance.ReceiveBytes);
            newThread.Start();
        }
    }

    /// <summary>
    ///     Handles an incoming line of text.
    /// </summary>
    /// <param name="line">Incoming data.</param>
    /// <param name="endpoint">url</param>
    /// <returns>The answer to be sent back as a reaction to the received line or null.</returns>
    protected abstract string ReceiveMessage(string line, string endpoint);
}