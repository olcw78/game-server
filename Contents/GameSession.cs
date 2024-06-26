using System.Net;
using System.Text;
using server.Session;

namespace Contents;

public class GameSession : Session {
  public override void OnConnect(EndPoint? endPoint) {
    Console.WriteLine($"{nameof(OnConnect)}> connected to {endPoint}");

    byte[] sendBuf = Encoding.UTF8.GetBytes("Welcome to MMORPG server!");
    Send(sendBuf);

    Thread.Sleep(1000);

    Disconnect();
  }

  public override void OnDisconnect(EndPoint? endPoint) {
    Console.WriteLine($"{nameof(OnDisconnect)}> disconnected from {endPoint}");
  }

  public override int OnRecv(ArraySegment<byte> data) {
    string content = Encoding.UTF8.GetString(data);
    Console.WriteLine($"{nameof(OnRecv)}> {content}");
    
    return data.Count;
  }

  public override void OnSend(int byteTransferred) {
    Console.WriteLine($"{nameof(OnSend)}> byte transferred: {byteTransferred}");
  }
}