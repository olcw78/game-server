using System.Net;
using System.Net.Sockets;
using gs.shared;

namespace gs.server;

internal sealed class Listener
{
    Socket _listener;

    public Listener(int port)
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
        IPAddress mezelf = ipHostEntry.AddressList[0];
        IPEndPoint ipEndPoint = new(mezelf, port);

        _listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        _listener.Bind(ipEndPoint);
        _listener.Listen(backlog: 10);
    }

    public async Task ListenAsync()
    {
        try
        {
            do
            {
                Console.WriteLine($"Listening... at 3080");
                var accepted = await _listener.AcceptAsync();

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
    }
}
