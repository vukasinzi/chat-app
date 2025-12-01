using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

      
        internal async Task<Odgovor> RegistrujSe(Korisnik k)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.RegistrujSe, k);
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                return o;
            }
            catch(Exception x)
            {
                Odgovor o = new Odgovor();
                o.Poruka = "greska";
                return o;
            }
        }

        internal async Task<Odgovor> Dostupan(Korisnik k)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Dostupnost, k);
                await serializer.SendAsync(z);
                Odgovor odgovor = await serializer.ReceiveAsync<Odgovor>();

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
