using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server? server = null;
            Task? serverTask = null;
            int i = 0;
            bool trenutly = false;
            Console.WriteLine("Startovanje servera - 1. Gasenje servera - 0.");
            while (true)
            {
                int.TryParse(Console.ReadLine(),out i);
                if (i == 0 && trenutly)
                {
                    trenutly = false;
                    server.Stop();
                    if (serverTask != null)
                        await serverTask;
                    server = null;
                    serverTask = null;
                    Console.WriteLine("Server zaustavljen.");
                    continue;
                }
                if (i == 1 && !trenutly) { 
                    server = new Server();
                    server.Listen();
                    serverTask = server.StartAsync();
                    Console.WriteLine("Server je upaljen.");
                    trenutly = true;
                }
            }
          

        }
    }
}