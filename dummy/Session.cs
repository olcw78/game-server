using System.Net;
using System.Net.Sockets;
using System.Text;

namespace gs.dummy;

public class Session
{
    readonly Socket _server;
    IPEndPoint _ipEndPoint;

    public Session(int port)
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
        IPAddress localhost = ipHostEntry.AddressList[0];

        _ipEndPoint = new IPEndPoint(localhost, port);
        _server = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _server.ConnectAsync(_ipEndPoint);
            Console.WriteLine($"connected {_server.RemoteEndPoint}");

            while (true)
            {
                byte[] buf = Encoding.UTF8.GetBytes("Message from dummy");
                _server.Send(buf);

                buf = new byte[1024];
                int recvBytes = _server.Receive(buf);
                Console.WriteLine("recvBytes: " + recvBytes);
                if (recvBytes > 0)
                {
                    string recvContent = Encoding.UTF8.GetString(buf, 0, recvBytes);
                    Console.WriteLine(recvContent);
                }

                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}