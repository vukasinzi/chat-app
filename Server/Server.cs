using System;
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
        private CancellationTokenSource cts;
        private List<ClientHandler> handleri;
        public Server()
        {
            serverskiSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            handleri = new List<ClientHandler>();

        }
        public void Listen()
        {
            try
            {
                serverskiSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
                serverskiSocket.Listen();
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
                    lock (_lock)
                        handleri.Add(handler);
                    handler.HandleRequests(token);

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
            List<ClientHandler> copy = new List<ClientHandler>(handleri);
            foreach (var c in copy)
            {
                try
                {
                    c.socket.Close();
                }
                catch { }
            }
            lock (_lock)
                handleri.Clear();
            serverskiSocket.Close();    
        }


        object _lock = new object();
        internal void RemoveClient(ClientHandler clientHandler)
        {
            lock(_lock)
                handleri.Remove(clientHandler);
        }
    }
   
}
