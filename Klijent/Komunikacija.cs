using Klijent.Domen;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
        private readonly string ip;
        private readonly int port;
        private readonly int pushPort;
        public event Action<Poruka> PorukaPrimljena;
        public event Action<PrijateljstvoView> PrijateljaDodaj;
        public event Action<Korisnik> PrijateljaPrihvati;
        public event Action<Korisnik> PrijateljaObrisi;
        public event Action<PrijateljstvoView> PrijateljaOdbij;

        private Komunikacija()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ip = config.GetRequiredSection("ServerSettings").GetValue<string>("Ip");
    
            port = config.GetRequiredSection("ServerSettings").GetValue<int>("Port");
            pushPort = config.GetRequiredSection("ServerSettings").GetValue<int>("PushPort");
        }

        bool isConnected()
        {
            if (socket != null && socket.Connected)
                return true;
            return false;
        }
        private Odgovor Greska(Exception x)
        {
            Debug.WriteLine(x.Message);
            Odgovor o = new Odgovor();
            o.Uspesno = false;
            o.Poruka = x.Message;
            return o;
        }
        private Odgovor Greska(string poruka)
        {
            Odgovor o = new Odgovor();
            o.Uspesno = false;
            o.Poruka = poruka;
            return o;
        }
        internal async Task ConnectAsync(CancellationToken token = default)
        {
            try
            {
                if (!isConnected())
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(ip, port, token);
                    serializer = new JsonNetworkSerializer(socket);
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                throw;
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
                return Greska(ex);
            }
        }

        private async Task PosaljiUsername(string v, CancellationToken token)
        {
            if (pushSocket != null && pushSocket.Connected) return;

            pushSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await pushSocket.ConnectAsync(ip, pushPort, token);
            pushSerializer = new JsonNetworkSerializer(pushSocket);
            await pushSerializer.SendAsync(v, token);

            pushCts = new CancellationTokenSource();
            _ = Task.Run(() => HandlePush(pushCts.Token), pushCts.Token);
        }

        private async Task HandlePush(CancellationToken token)
        {
            while(!token.IsCancellationRequested && pushSocket != null&&  pushSocket.Connected)
            {
                Zahtev z = await pushSerializer.ReceiveAsync<Zahtev>(token);
                switch (z.Operacija)
                {
                    case Operacija.Posalji:
                        PorukaPrimljena.Invoke(serializer.ReadType<Poruka>((JsonElement)z.Objekat));
                        break;
                    case Operacija.DodajPrijatelja:
                        PrijateljaDodaj.Invoke(serializer.ReadType<PrijateljstvoView>((JsonElement)z.Objekat));
                        break;
                    case Operacija.PrihvatiPrijatelja:
                        PrijateljaPrihvati.Invoke(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.ObrisiPrijateljstvo:
                        PrijateljaObrisi.Invoke(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.OdbijPrijatelja:
                        PrijateljaOdbij.Invoke(serializer.ReadType<PrijateljstvoView>((JsonElement)z.Objekat));
                        break;

                }

            }
        }

        internal async Task<Odgovor> RegistrujSe(Korisnik k, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.RegistrujSe, k);
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (o.Rezultat is JsonElement je)
                {
                    Korisnik l = serializer.ReadType<Korisnik>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch(Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> Dostupan(Korisnik k, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Dostupnost, k);
                await serializer.SendAsync(z, token);
                Odgovor odgovor = await serializer.ReceiveAsync<Odgovor>(token);

                return odgovor;
            }
            catch (Exception ex)
            {
                return Greska(ex);
            }
        }

        internal async Task<Odgovor> Pretrazi(string text, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Pretraga, text);
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (!o.Uspesno || o.Rezultat == null)
                    return o;
                Korisnik k = serializer.ReadType<Korisnik>((JsonElement)o.Rezultat);
                o.Rezultat = k;
                return o;
            }catch(Exception x)
            {
                return Greska(x);
            }
        }
        internal async Task<Odgovor> Pretrazi(int id, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Pretraga2, id);
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (o.Rezultat == null)
                    return o;
                if (o.Rezultat is JsonElement je)
                    o.Rezultat = serializer.ReadType<string>(je);
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> vratiSvePrijatelje(Korisnik id, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev(Operacija.Prijatelji,id);
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (o.Rezultat is JsonElement je)
                {
                    List<Korisnik> l = serializer.ReadType<List<Korisnik>>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch(Exception x)
            {
                return Greska(x);
            }

        }

        internal async Task<Odgovor> Posalji(string pt,int posiljalac,int primalac, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.Posalji;
                Poruka p = new Poruka(primalac,posiljalac,pt);
                z.Objekat = p;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                return o;
            }
            catch(Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> DodajPrijatelja(int id, int id2, CancellationToken token = default)
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
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> VratiZahtevePrijatelja(int id, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.VratiZahtevePrijatelja;

                z.Objekat = id;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (o.Rezultat is JsonElement je)
                {
                    List<Prijateljstvo> l = serializer.ReadType<List<Prijateljstvo>>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> PrihvatiPrijatelja(Prijateljstvo prijatelj, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.PrihvatiPrijatelja;

                z.Objekat = prijatelj;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> OdbijPrijatelja(Prijateljstvo prijatelj, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.OdbijPrijatelja;

                z.Objekat = prijatelj;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> UcitajSvePoruke(Korisnik primalac, Korisnik k, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.UcitajSvePoruke;
                Tuple<int, int> tupl = new Tuple<int, int>(primalac.Id, k.Id);
                z.Objekat = tupl;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                if (o.Rezultat is JsonElement je)
                {
                    List<Poruka> l = serializer.ReadType<List<Poruka>>(je);
                    o.Rezultat = l;
                }
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }

        internal async Task<Odgovor> ObrisiPrijateljstvo(int id1, int id2, CancellationToken token = default)
        {
            try
            {
                Zahtev z = new Zahtev();
                z.Operacija = Operacija.ObrisiPrijateljstvo;
                Prijateljstvo p = new Prijateljstvo();
                p.korisnik1_id = id1;
                p.korisnik2_id = id2;
                z.Objekat = p;
                await serializer.SendAsync(z, token);
                Odgovor o = await serializer.ReceiveAsync<Odgovor>(token);
                return o;
            }
            catch (Exception x)
            {
                return Greska(x);
            }
        }
    }
}
