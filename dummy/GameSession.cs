using System.Net;
using System.Text;
using server.Session;

namespace dummy;

public class GameSession : Session {
  public override void OnConnect(EndPoint endPoint) {
    Console.WriteLine($"{nameof(OnConnect)}> connected to {endPoint}");

    for (var i = 0; i < 10; i++) {
      byte[] sendBuf = Encoding.UTF8.GetBytes("I'm dummy!!");
      Send(sendBuf);
    }

    Thread.Sleep(1000);

    Disconnect();
  }

  public override void OnDisconnect(EndPoint endPoint) {
    Console.WriteLine($"{nameof(OnDisconnect)}> disconnected from {endPoint}");
  }

  public override void OnRecv(ArraySegment<byte> recvBuf) {
    string content = Encoding.UTF8.GetString(recvBuf);
    Console.WriteLine($"{nameof(OnRecv)}> {content}");
  }

  public override void OnSend(int byteTransferred) {
    Console.WriteLine($"{nameof(OnSend)}> byte transferred: {byteTransferred}");
  }
}