using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using Zajednicki;
using Zajednicki.Domen;

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
        private Socket pushSocket;
        private JsonNetworkSerializer pushSerializer;
        private CancellationTokenSource pushCts;
        public event Action<Poruka> PorukaPrimljena;

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
                if (odgovor.Rezultat is JsonElement je)
                {
                    Korisnik l = serializer.ReadType<Korisnik>(je);
                    odgovor.Rezultat = l;
                }
                if(odgovor.Uspesno)
                {
                    await PosaljiUsername(k.Korisnicko_ime.ToString(), token);
                }
                return odgovor;
            }
            catch (Exception ex)
            {
                Odgovor o = new Odgovor();
                o.Poruka = "greska";
                return o;
            }
        }

        private async Task PosaljiUsername(string v, CancellationToken token)
        {
            if (pushSocket != null && pushSocket.Connected) return;

            pushSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pushSocket.Connect("127.0.0.1", 10000);
            pushSerializer = new JsonNetworkSerializer(pushSocket);
            await pushSerializer.SendAsync(v, token);

            pushCts = new CancellationTokenSource();
            _ = Task.Run(() => HandlePush(pushCts.Token));
        }

        private async Task HandlePush(CancellationToken token)
        {
            while(!token.IsCancellationRequested && pushSocket != null&&  pushSocket.Connected)
            {
                Poruka p = await pushSerializer.ReceiveAsync<Poruka>(token);
                PorukaPrimljena.Invoke(p);
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

        internal async Task<Odgovor> Pretrazi(string text)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Pretraga, text);
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                if (o.Rezultat == null)
                    return null;
                Korisnik k = serializer.ReadType<Korisnik>((JsonElement)o.Rezultat);
                o.Rezultat = k;
                return o;
            }catch(Exception x)
            {
                Odgovor o = new Odgovor();
                o.Poruka = "greska";
                return o;
            }
        }
        internal async Task<Odgovor> Pretrazi(int id)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Pretraga2, id);
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                if (o.Rezultat is Korisnik)
                {
                    Korisnik k = serializer.ReadType<Korisnik>((JsonElement)o.Rezultat);
                    o.Rezultat = k;
                    return o;
                }
                else
                {
                    o.Rezultat = serializer.ReadType<string>((JsonElement)o.Rezultat);
                    return o;
                }
            }
            catch (Exception x)
            {
                Odgovor o = new Odgovor();
                o.Poruka = "greska";
                return o;
            }
        }

        internal async Task<Odgovor> vratiSvePrijatelje(Korisnik id)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Prijatelji,id);
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                if (o.Rezultat is JsonElement je)
                {
                    List<Korisnik> l = serializer.ReadType<List<Korisnik>>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch(Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }

        }

        internal async Task<Odgovor> Posalji(string pt,int posiljalac,int primalac)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.Posalji;
                Poruka p = new Poruka(primalac,posiljalac,pt);
                z.Objekat = p;
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                return o;
            }
            catch(Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }
        }

        internal async Task<Odgovor> DodajPrijatelja(int id, int id2)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.DodajPrijatelja;
                Prijateljstvo p = new Prijateljstvo();
                p.status = "ceka se";
                p.korisnik1_id = id;
                p.korisnik2_id = id2;
                z.Objekat = p;
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                return o;
            }
            catch (Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }
        }

        internal async Task<Odgovor> ProveriNovePrijatelje(int id)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.VratiZahtevePrijatelja;

                z.Objekat = id;
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                if (o.Rezultat is JsonElement je)
                {
                    List<Prijateljstvo> l = serializer.ReadType<List<Prijateljstvo>>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch (Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }
        }

        internal async Task<Odgovor> PrihvatiPrijatelja(Prijateljstvo prijatelj)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.PrihvatiPrijatelja;

                z.Objekat = prijatelj;
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                return o;
            }
            catch (Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }
        }

        internal async Task<Odgovor> OdbijPrijatelja(Prijateljstvo prijatelj)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.OdbijPrijatelja;

                z.Objekat = prijatelj;
                await serializer.SendAsync(z);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>();
                return o;
            }
            catch (Exception x)
            {
                Odgovor o = new Odgovor();
                return o;
            }
        }
    }
}
