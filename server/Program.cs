using System.Net;
using System.Net.Sockets;

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
IPAddress mezelf = ipHostEntry.AddressList[0];
IPEndPoint iPEndPoint = new(mezelf, 3080);

Socket socket = new(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

try
{
    socket.Bind(iPEndPoint);
    socket.Listen(backlog: 10);

    do
    {
        Console.WriteLine($"Listening... at 3080");
        var accepted = await socket.AcceptAsync();

        byte[] buf = new byte[1024];
        int recvBytes = await accepted.ReceiveAsync(buf);
        if (recvBytes > 0)
        {
            string msg = System.Text.Encoding.UTF8.GetString(buf, 0, recvBytes);
            Console.WriteLine($"Received: {msg}");

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(
                "Jij hebt wel op de ober geconnecteerd."
            );
            await accepted.SendAsync(sendBytes);
        }

        accepted.Shutdown(SocketShutdown.Both);
        accepted.Close(100);
    } while (true);
}
catch (Exception ex)
{
    System.Console.WriteLine(ex.Message);
}
