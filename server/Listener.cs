using System.Net;
using System.Net.Sockets;
using System.Text;

namespace gs.server;

internal sealed class Listener
{
    readonly Socket _listener;
    event Action<Socket?> OnAccepted;

    public Listener(int port, Action<Socket?> onAccepted)
    {
        OnAccepted = onAccepted;

        string host = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
        IPAddress mezelf = ipHostEntry.AddressList[0];
        IPEndPoint ipEndPoint = new(mezelf, port);

        _listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        _listener.Bind(ipEndPoint);
        _listener.Listen(backlog: 10);

        SocketAsyncEventArgs args = new();
        args.Completed += this.OnAcceptCompleted;

        RegisterAccept(args);
    }

    public void RegisterAccept(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;

        bool isPending = _listener.AcceptAsync(args);
        if (!isPending)
        {
            // completed after a line.
            this.OnAcceptCompleted(null, args);
            return;
        }
    }

    void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            OnAccepted?.Invoke(args.AcceptSocket);
        }
        else
        {
            Console.WriteLine(args.SocketError);
        }

        RegisterAccept(args);
    }

    public Socket Accept()
    {
        return _listener.Accept();
    }
}
