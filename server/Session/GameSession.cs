using System.Net.Sockets;
using System.Text;

namespace server.Session;

public class GameSession : Session {
  protected override void OnConnect(Socket conn) {
    Console.WriteLine($"{nameof(OnConnect)}");
  }

  protected override void OnDisconnect() {
    Console.WriteLine($"{nameof(OnDisconnect)}");
  }

  protected override void OnRecv(ArraySegment<byte> recvBuf) {
    Console.WriteLine($"{nameof(OnRecv)}");

    string content = Encoding.UTF8.GetString(recvBuf);
    Console.WriteLine($"[From Client] : {content}");
  }

  protected override void OnSend(int byteTransferred) {
    Console.WriteLine($"{nameof(OnSend)} byte transferred: {byteTransferred}");
  }
}