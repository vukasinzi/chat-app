using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string ip = config.GetValue<string>("ServerSettings:Ip") ?? "127.0.0.1";
            int port = config.GetValue<int>("ServerSettings:Port");
            int pushPort = config.GetValue<int>("ServerSettings:PushPort");

            Server? server = null;
            Task? serverTask = null;
            int i = 0;
            bool trenutly = false;
            Console.WriteLine("Startovanje servera - 1. Gasenje servera - 0.");
            while (true)
            {
                string? unos = await Console.In.ReadLineAsync();
                int.TryParse(unos,out i);
                if (i == 0 && trenutly)
                {
                    trenutly = false;
                    server?.Stop();
                    if (serverTask != null)
                        await serverTask;
                    server = null;
                    serverTask = null;
                    Console.WriteLine("Server zaustavljen.");
                    continue;
                }
                if (i == 1 && !trenutly) { 
                    server = new Server(ip, port, pushPort);
                    server.Listen();
                    serverTask = server.StartAsync();
                    Console.WriteLine("Server je upaljen.");
                    trenutly = true;
                    
                }
            }
          

        }
    }
}
