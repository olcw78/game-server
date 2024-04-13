using System.Net;
using dummy;
using Shared;

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Console.WriteLine(envPath);
Dotenv.Load(envPath);

int port = Dotenv.GetI("PORT") ?? 3000;

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
IPAddress localhost = ipHostEntry.AddressList[0];
IPEndPoint ipEndPoint = new IPEndPoint(localhost, port);

var connector = new Connector(ipEndPoint, () => new GameSession());

try {
}
catch (Exception ex) {
  Console.WriteLine(ex.Message);
}

Thread.Sleep(3000);