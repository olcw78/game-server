using System.Net.Sockets;

namespace server.Session;

public interface ISession {
  bool Disconnected { get; }

  void Disconnect();
  void StartRecv(SocketAsyncEventArgs args);
  void OnRecvComplete(object? sender, SocketAsyncEventArgs args);

  void StartSend();
  void OnSendComplete(object? sender, SocketAsyncEventArgs args);
}