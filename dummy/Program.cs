using System.Net;
using System.Net.Sockets;

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);

// foreach (var ad in ipHostEntry.AddressList)
//     System.Console.WriteLine(ad);
IPAddress mezelf = ipHostEntry.AddressList[0];
IPEndPoint ipEndPoint = new(mezelf, 3080);

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
