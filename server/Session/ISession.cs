using System.Net;
using System.Net.Sockets;

namespace server.Session;

public interface ISession {
  bool Disconnected { get; }

  void Disconnect();
  void StartRecv(SocketAsyncEventArgs args);
  void OnRecvComplete(object? sender, SocketAsyncEventArgs args);

  void StartSend();
  void OnSendComplete(object? sender, SocketAsyncEventArgs args);

  void OnConnect(EndPoint endPoint);
  void OnDisconnect(EndPoint endPoint);
  void OnRecv(ArraySegment<byte> data);
  void OnSend(int byteTransferred);
  void Start(Socket conn);
}