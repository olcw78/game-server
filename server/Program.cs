using System.Net.Sockets;
using System.Text;
using gs.server;
using gs.shared;

ManualResetEventSlim preventExitEvent = new();

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Dotenv.Load(envPath);

var listener = new Listener(OnAccepted);
int port = Dotenv.GetI("PORT") ?? 3000;
listener.Listen(port,
  () => {
    //
    Console.WriteLine($"Server Listening at ${port}");
  }
);

preventExitEvent.Wait();

void OnAccepted(Socket? socket) {
  if (socket == null)
    return;

  byte[] buf = new byte[1024];

  try {
    int recvBytes = socket.Receive(buf);
    System.Console.WriteLine("recvBytes: " + recvBytes);

    if (recvBytes > 0) {
      string msg = Encoding.UTF8.GetString(buf, 0, recvBytes);
      Console.WriteLine($"Received: {msg}");

      buf = Encoding.UTF8.GetBytes("You are connected");
      int sendByte = socket.Send(buf);
    }
  }
  catch (Exception ex) {
    Console.WriteLine(ex.Message);
  }
  finally {
    socket.Shutdown(SocketShutdown.Both);
    socket.Close(100);
  }
}