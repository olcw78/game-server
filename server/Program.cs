using System.Net;
using System.Net.Sockets;
using gs.server;
using gs.shared;

string envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
System.Console.WriteLine(envPath);
Dotenv.Load(envPath);

int port = int.Parse(Dotenv.Get("PORT"));
Listener listener = new Listener(port);

await listener.ListenAsync();
