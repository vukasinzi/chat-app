using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Zajednicki;

namespace Klijent
{
    public class Komunikacija
    {
        public static Komunikacija instance;
        public static Komunikacija Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Komunikacija();
                }
                return instance;
            }
        }
        public JsonNetworkSerializer serializer;
        Socket socket;
        bool isConnected()
        {
            if (socket != null && socket.Connected)
                return true;
            return false;
        }
        internal void Connect()
        {
            try
            {
                if (!isConnected())
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect("127.0.0.1", 9999);
                    serializer = new JsonNetworkSerializer(socket);
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
        internal async Task<Odgovor> LogInAsync(Korisnik k, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.LogIn,k);
                await serializer.SendAsync(z, token);
                Odgovor odgovor = await serializer.ReceiveAsync<Odgovor>(token);

                return odgovor;
            }
            catch (Exception ex)
            {
                Odgovor o = new Odgovor();
                o.Poruka = "greska";
                return o;
            }
        }

    }
}
