using System.Net;
using System.Net.Sockets;
using server.Session;

namespace server;

public sealed class Listener {
  private readonly IPEndPoint _ipEndPoint;
  private readonly SocketAsyncEventArgs _listenerArgs;
  private readonly Socket? _listenerSocket;

  public Listener(
    IPEndPoint ipEndPoint,
    Func<ISession> sessionFactory
  ) {
    _listenerArgs = new SocketAsyncEventArgs();
    _listenerArgs.Completed += this.OnAcceptComplete;

    _ipEndPoint = ipEndPoint;
    SessionFactory = sessionFactory;

    _listenerSocket = new Socket(
      _ipEndPoint.AddressFamily,
      SocketType.Stream,
      ProtocolType.Tcp
    );

    _listenerSocket.Bind(localEP: _ipEndPoint);
    _listenerSocket.Listen(backlog: 10);

    StartAccept(_listenerArgs);

    Console.WriteLine(
      $"Server Listening at {ipEndPoint.Address}:{ipEndPoint.Port}");
  }

  private event Func<ISession> SessionFactory;

  private void StartAccept(SocketAsyncEventArgs args) {
    args.AcceptSocket = null;

    bool isPending = _listenerSocket!.AcceptAsync(args);
    if (!isPending) {
      this.OnAcceptComplete(null, args);
    }
  }

  private void OnAcceptComplete(object? sender, SocketAsyncEventArgs args) {
    if (args.SocketError == SocketError.Success) {
      ISession session = SessionFactory();

      Socket? conn = args.AcceptSocket;
      if (conn != null) {
        session.Start(conn);
        session.OnConnect(conn.RemoteEndPoint);
      }
    }
    else {
      Console.WriteLine(args.SocketError);
    }

    // start accept again
    StartAccept(args);
  }
}