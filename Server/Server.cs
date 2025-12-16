using Klijent.Domen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Server
    {
        private Socket serverskiSocket;
        private Socket pushSocket;
        private CancellationTokenSource cts;
        private ConcurrentDictionary<string,ClientHandler> online;

        
        public Server()
        {
            online = new ConcurrentDictionary<string, ClientHandler>();

            serverskiSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pushSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          

        }
        public void Listen()
        {
            try
            {
                serverskiSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
                serverskiSocket.Listen();
                pushSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));
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
            try
            {
                while (!token.IsCancellationRequested)
                {

                    Socket klijentskiSocket = await serverskiSocket.AcceptAsync(token);
                    ClientHandler handler = new ClientHandler(klijentskiSocket, this);
                    await handler.HandleRequests(token);

                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
        public void Stop()
        {
            try
            {
                cts?.Cancel();
            }
            catch { }
            ConcurrentDictionary<string, ClientHandler> copy = new ConcurrentDictionary<string, ClientHandler>(online);
            foreach (var c in copy)
            {
                try
                {
                    c.Value.socket.Close();
                }
                catch { }
            }
            lock (_lock)
                online.Clear();
            serverskiSocket.Close();
            online = new ConcurrentDictionary<string, ClientHandler>();
        }


        object _lock = new object();
        internal void RemoveClient(ClientHandler clientHandler,string currentUser)
        {
            lock (_lock)
            {
                online.TryRemove(currentUser, out _);

            }
        }
        internal void AddClient(ClientHandler clienthandler,string currentUser)
        {
            lock(_lock)
            {
                online.TryAdd(currentUser, clienthandler);
            }
        }

        internal bool isOnline(Korisnik l, ClientHandler clientHandler)
        {
            foreach (var x in online)
                if (x.Key == l.Korisnicko_ime.ToString())
                    return true;
            return false;
        }
    }
   
}
