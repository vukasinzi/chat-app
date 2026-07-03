using Klijent.Domen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Zajednicki;

namespace Server
{
    public class Server
    {
        string certPassword = Environment.GetEnvironmentVariable("CHATAPP_CERT_PASSWORD")
                              ?? throw new InvalidOperationException("CHATAPP_CERT_PASSWORD nije podesen.");

        private readonly X509Certificate2 certificate;
        
        private Socket serverskiSocket;
        private Socket pushSocket;
        private CancellationTokenSource? cts;
        internal ConcurrentDictionary<string,ClientHandler> online;
        private readonly string ip;
        private readonly int port;
        private readonly int pushPort;

        
        public Server(string ip, int port, int pushPort)
        {
            certificate = new X509Certificate2("server.pfx", certPassword);
            this.ip = ip;
            this.port = port;
            this.pushPort = pushPort;
            online = new ConcurrentDictionary<string, ClientHandler>();

            serverskiSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pushSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          

        }
        public void Listen()
        {
            try
            {
                serverskiSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                serverskiSocket.Listen();
                pushSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), pushPort));
                pushSocket.Listen();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
        public async Task StartAsync()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            _ = Task.Run(() => AcceptPushLoopAsync(token), token);
            try
            {
                while (!token.IsCancellationRequested)
                {

                    Socket klijentskiSocket = await serverskiSocket.AcceptAsync(token);

                    var networkStream = new NetworkStream(klijentskiSocket, ownsSocket: false);
                    var sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false);

                    await sslStream.AuthenticateAsServerAsync(certificate);

                    ClientHandler handler = new ClientHandler(klijentskiSocket, this, sslStream);

                    _ = Task.Run(() => handler.HandleRequests(token), token);

                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }

        private async Task AcceptPushLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Socket s = await pushSocket.AcceptAsync(token);
                
                var networkStream = new NetworkStream(s, ownsSocket: false);
                var sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false);
                await sslStream.AuthenticateAsServerAsync(certificate);
                var ser = new JsonNetworkSerializer(sslStream);
                
                try
                {
                    string username = await ser.ReceiveAsync<string>(token);
                    if (online.TryGetValue(username, out var handler))
                        await handler.babyConstructor(s,ser); 
                    else
                        s.Close(); 
                }
                catch
                {
                    s.Close();
                }
            }
        }

        public void Stop()
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
            ConcurrentDictionary<string, ClientHandler> copy = new ConcurrentDictionary<string, ClientHandler>(online);
            foreach (var c in copy)
            {
                try
                {
                    c.Value.socket.Close();
                }
                catch (Exception x)
                {
                    Debug.WriteLine(x.Message);
                }
            }
            lock (_lock)
                online.Clear();
            serverskiSocket.Close();
            pushSocket.Close();
            online = new ConcurrentDictionary<string, ClientHandler>();
        }


        object _lock = new object();
        internal void RemoveClient(ClientHandler clientHandler,string currentUser)
        {
            lock (_lock)
            {
                try
                { 
                    online.TryRemove(currentUser, out _);
                }
                catch(Exception x)
                {
                    Debug.WriteLine(x.Message);
                }
            }
        }
        internal bool AddClient(ClientHandler clienthandler,string currentUser)
        {
            lock(_lock)
            {
                return online.TryAdd(currentUser, clienthandler);
            }
        }

    
    }
   
}
