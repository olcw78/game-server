using System.Net.Sockets;
using System.Text;
using server;
using server.Session;
using Shared;

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
return;

void OnAccepted(Socket? acceptedSocket) {
  if (acceptedSocket == null)
    return;

  try {
    GameSession gameSession = new();
    gameSession.Start(acceptedSocket);

    byte[] sendBuf = Encoding.UTF8.GetBytes("Welcome to MMORPG server!");
    gameSession.Send(sendBuf);
  }
  catch (Exception ex) {
    Console.WriteLine(ex.Message);
  }

  Thread.Sleep(1000);
}