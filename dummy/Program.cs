using gs.dummy;
using gs.shared;

string envPath = Path.Join(Directory.GetCurrentDirectory(), ".env");
Console.WriteLine(envPath);
Dotenv.Load(envPath);

int port;
if (!int.TryParse(Dotenv.Get("PORT"), out port))
    port = 3000;

Session session = new(port);
await session.ConnectAsync();