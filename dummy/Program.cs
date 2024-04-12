using System.Net;
using System.Net.Sockets;
using System.Text;
using gs.shared;

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Console.WriteLine(envPath);
Dotenv.Load(envPath);

int port = Dotenv.GetI("PORT") ?? 3000;

Socket server;
string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
IPAddress localhost = ipHostEntry.AddressList[0];
IPEndPoint ipEndPoint = new IPEndPoint(localhost, port);

while (true) {
  try {
    server = new Socket(ipEndPoint.AddressFamily, SocketType.Stream,
      ProtocolType.Tcp);
    await server.ConnectAsync(ipEndPoint);
    Console.WriteLine($"connected {server.RemoteEndPoint}");

    byte[] buf = Encoding.UTF8.GetBytes("dummy client is connected");
    server.Send(buf);

    buf = new byte[1024];
    int recvBytes = server.Receive(buf);
    if (recvBytes > 0) {
      string recvContent = Encoding.UTF8.GetString(buf, 0, recvBytes);
      Console.WriteLine($"[{recvBytes}]received: {recvContent}");
    }

    server.Shutdown(SocketShutdown.Both);
    server.Close();

    Thread.Sleep(1000);
  }
  catch (Exception ex) {
    Console.WriteLine(ex.Message);
  }
}