using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Console.WriteLine(envPath);
Dotenv.Load(envPath);

int port = Dotenv.GetI("PORT") ?? 3000;

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
IPAddress localhost = ipHostEntry.AddressList[0];
IPEndPoint ipEndPoint = new IPEndPoint(localhost, port);

while (true) {
  try {
    Socket server = new Socket(ipEndPoint.AddressFamily, SocketType.Stream,
      ProtocolType.Tcp);
    await server.ConnectAsync(ipEndPoint);
    Console.WriteLine($"connected to {server.RemoteEndPoint}");

    byte[] buf = Encoding.UTF8.GetBytes("dummy client is connected");
    server.Send(buf);

    buf = new byte[1024];
    int recvBytes = server.Receive(buf);
    if (recvBytes > 0) {
      string recvContent = Encoding.UTF8.GetString(buf, 0, recvBytes);
      Console.WriteLine($"[From Server] {recvContent}[{recvBytes}]");
    }

    server.Shutdown(SocketShutdown.Both);
    server.Close();
  }
  catch (Exception ex) {
    Console.WriteLine(ex.Message);
  }

  Thread.Sleep(3000);
}