using System.Net;
using System.Net.Sockets;
using gs.shared;

string envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
Console.WriteLine(envPath);
Dotenv.Load(envPath);

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
IPAddress mezelf = ipHostEntry.AddressList[0];

int port = int.Parse(Dotenv.Get("PORT"));
IPEndPoint ipEndPoint = new(mezelf, port);

try
{
    Socket socket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    await socket.ConnectAsync(ipEndPoint);

    System.Console.WriteLine($"connected {socket.RemoteEndPoint}");

    byte[] buf = new byte[1024];

    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes("Message from dummy");
    await socket.SendAsync(sendBytes);

    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}
catch (Exception ex)
{
    System.Console.WriteLine(ex.Message);
}
