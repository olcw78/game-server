using System.Net;
using System.Net.Sockets;
using System.Text;

namespace gs.dummy;

public class Session {
  Socket _server;
  readonly IPEndPoint _ipEndPoint;

  public Session(int port) {
    string host = Dns.GetHostName();
    IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
    IPAddress localhost = ipHostEntry.AddressList[0];

    _ipEndPoint = new IPEndPoint(localhost, port);
  }

  public async Task ConnectAsync() {
    while (true) {
      try {
        _server = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await _server.ConnectAsync(_ipEndPoint);
        Console.WriteLine($"connected {_server.RemoteEndPoint}");

        byte[] buf = Encoding.UTF8.GetBytes("dummy client is connected");
        _server.Send(buf);

        buf = new byte[1024];
        int recvBytes = _server.Receive(buf);
        if (recvBytes > 0) {
          string recvContent = Encoding.UTF8.GetString(buf, 0, recvBytes);
          Console.WriteLine($"[{recvBytes}]received: {recvContent}");
        }
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
      finally {
        _server.Shutdown(SocketShutdown.Both);
        _server.Close();

        Thread.Sleep(1000);
      }
    }
  }
}