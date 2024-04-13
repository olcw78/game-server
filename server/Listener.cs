using System.Net;
using System.Net.Sockets;

namespace server;

internal sealed class Listener {
  private Socket? _listenerSocket;
  private event Action<Socket?> OnAccepted;
  private readonly SocketAsyncEventArgs _listenerArgs;

  public Listener(Action<Socket?> onAccepted) {
    OnAccepted -= onAccepted;
    OnAccepted += onAccepted;

    _listenerArgs = new SocketAsyncEventArgs();
    _listenerArgs.Completed += this.OnAcceptComplete;
  }

  public void Listen(int port, Action onListenSuccess) {
    string host = Dns.GetHostName();
    IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
    IPAddress me = ipHostEntry.AddressList[0];
    IPEndPoint _ipEndPoint = new IPEndPoint(me, port);

    _listenerSocket = new Socket(
      _ipEndPoint.AddressFamily,
      SocketType.Stream,
      ProtocolType.Tcp
    );

    _listenerSocket.Bind(localEP: _ipEndPoint);
    _listenerSocket.Listen(backlog: 10);

    StartAccept(_listenerArgs);
    onListenSuccess.Invoke();
  }

  private void StartAccept(SocketAsyncEventArgs args) {
    args.AcceptSocket = null;

    bool isPending = _listenerSocket!.AcceptAsync(args);
    if (!isPending) {
      this.OnAcceptComplete(null, args);
    }
  }

  private void OnAcceptComplete(object? sender, SocketAsyncEventArgs args) {
    if (args.SocketError == SocketError.Success) {
      OnAccepted?.Invoke(args.AcceptSocket);
    }
    else {
      Console.WriteLine(args.SocketError);
    }

    // start accept again
    StartAccept(args);
  }
}