using System.Net;
using System.Net.Sockets;

namespace gs.dummy;

public class Session
{
    readonly Socket _client;
    IPEndPoint _ipEndPoint;

    public Session(int port)
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
        IPAddress localhost = ipHostEntry.AddressList[0];

        _ipEndPoint = new IPEndPoint(localhost, port);
        _client = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _client.ConnectAsync(_ipEndPoint);

            Console.WriteLine($"connected {_client.RemoteEndPoint}");

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes("Message from dummy");
            await _client.SendAsync(sendBytes);

            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}