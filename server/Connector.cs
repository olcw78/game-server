using System.Net;
using System.Net.Sockets;
using server.Session;

namespace dummy;

public class Connector {
  private readonly SocketAsyncEventArgs _connArgs;
  private readonly IPEndPoint _ipEndPoint;
  private readonly Func<ISession> SessionFactory;

  public Connector(IPEndPoint ipEndPoint, Func<ISession> sessionFactory) {
    _ipEndPoint = ipEndPoint;
    SessionFactory = sessionFactory;

    var conn = new Socket(
      ipEndPoint.AddressFamily,
      SocketType.Stream,
      ProtocolType.Tcp
    );
    _connArgs = new SocketAsyncEventArgs();
    _connArgs.Completed += OnConnectComplete;
    _connArgs.RemoteEndPoint = ipEndPoint;
    _connArgs.UserToken = conn;

    StartConnect(_connArgs);
  }

  public void StartConnect(SocketAsyncEventArgs args) {
    var conn = args.UserToken as Socket;
    if (conn == null)
      return;

    bool isPending = conn.ConnectAsync(args);
    if (!isPending) OnConnectComplete(null, args);
  }

  public void OnConnectComplete(object? sender, SocketAsyncEventArgs args) {
    if (args.SocketError == SocketError.Success) {
      ISession session = SessionFactory();
      session.Start(args.ConnectSocket);
      session.OnConnect(args.RemoteEndPoint);
    }
    else {
      Console.WriteLine(
        $"{nameof(OnConnectComplete)}> fail: {args.SocketError}");
    }
  }

  private void Disconnect() {
  }
}