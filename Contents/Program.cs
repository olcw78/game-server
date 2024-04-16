using System.Net;
using Contents;
using server;
using Shared;

ManualResetEventSlim preventExitEvent = new();

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Dotenv.Load(envPath);

string host = Dns.GetHostName();
IPHostEntry ipHostEntry = Dns.GetHostEntry(host);

int port = Dotenv.GetI("PORT") ?? 3000;
var ipEndPoint = new IPEndPoint(ipHostEntry.AddressList[0], port);

var listener = new Listener(ipEndPoint, () => new GameSession());

preventExitEvent.Wait();
return;