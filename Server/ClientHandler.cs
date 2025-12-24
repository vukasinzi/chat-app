
using Klijent.Domen;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Zajednicki;
using Zajednicki.Domen;

namespace Server
{
    public class ClientHandler
    {
        public Socket socket;
        private Server server;
        private string currentUser;
        private JsonNetworkSerializer serializer;
        private Socket pushSocket;
        private JsonNetworkSerializer pushSerializer;

        public ClientHandler(Socket socket,Server server)
        {
           
            this.socket = socket;
            this.server = server;
            serializer = new JsonNetworkSerializer(this.socket);
        }
        internal void babyConstructor(Socket s)
        {
            pushSocket = s;
            pushSerializer = new JsonNetworkSerializer(pushSocket);
        }
        public async Task HandleRequests(CancellationToken token)
        {
            try
            {
                while(socket.Connected && !token.IsCancellationRequested)
                {
                    Zahtev z = await serializer.ReceiveAsync<Zahtev>(token);
                    Odgovor o = await ProcessRequests(z);
                    await serializer.SendAsync(o,token);
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine("Error in HandleRequestsAsync: " + x.Message);
            }
            finally
            {
                try
                {
                    if (socket.Connected)
                        socket.Close();
                }
                catch { }
                server.RemoveClient(this,currentUser);
                
            }
        }

        public Task PushAsync(Zahtev z, CancellationToken token = default)
        {
            if (pushSocket == null || !pushSocket.Connected || pushSerializer == null)
                return Task.CompletedTask;

            return pushSerializer.SendAsync(z, token);
        }

        private async Task<Odgovor> ProcessRequests(Zahtev z)
        {
            Odgovor o = new Odgovor();
            try
            {
                switch(z.Operacija)
                {
                    case Operacija.LogIn:
                        var l = serializer.ReadType<Korisnik>((JsonElement)z.Objekat);
                        if (server.isOnline(l, this))
                        {
                            o.Poruka = "logovan";
                            break;
                        }
                        o = await Kontroler.Instance.LogIn(l);
                        if (o.Uspesno)
                        {
                            server.AddClient(this, l.Korisnicko_ime.ToString());
                            currentUser = l.Korisnicko_ime.ToString();
                           
                        }
                        
                        break;
                    case Operacija.Dostupnost:
                        o.Uspesno = await Kontroler.Instance.Dostupan(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.RegistrujSe:
                        o = await Kontroler.Instance.RegistrujSe(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Pretraga:
                        o = await Kontroler.Instance.Pretrazi(serializer.ReadType<string>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Pretraga2:
                        o.Rezultat = await Kontroler.Instance.Pretrazi((int)serializer.ReadTypeStruct<int>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Prijatelji:
                        o.Rezultat = await Kontroler.Instance.Prijatelji(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Posalji:
                        {
                            Poruka p = serializer.ReadType<Poruka>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.Posalji(p);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(p.primalac_id);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    await client.PushAsync(z);
                                }
                            }
                        }
                        break;
                    case Operacija.DodajPrijatelja:
                        {
                            Prijateljstvo x = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.DodajPrijatelja(x);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(x.korisnik2_id);
                                PrijateljstvoView zaSlanje = new PrijateljstvoView(primalacUsername, x);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.DodajPrijatelja;
                                    _z.Objekat = zaSlanje;
                                    await client.PushAsync(_z);
                                }
                            }
                        }
                        break;
                    case Operacija.VratiZahtevePrijatelja:
                        o.Rezultat = await Kontroler.Instance.VratiZahtevePrijatelja(serializer.ReadTypeStruct<int>((JsonElement)z.Objekat));
                        break;
                    case Operacija.PrihvatiPrijatelja:
                        {
                            o.Uspesno = await Kontroler.Instance.PrihvatiPrijatelja(serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat));
                            if (o.Uspesno)
                            {
                                Prijateljstvo _p = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);

                                string posiljalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik1_id);
                                string primalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik2_id);
                                Odgovor _o = await Kontroler.Instance.Pretrazi(primalacUsername);
                                Korisnik _k = (Korisnik)_o.Rezultat;
                                if (server.online.TryGetValue(posiljalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.PrihvatiPrijatelja;
                                    _z.Objekat = _k;
                                    await client.PushAsync(_z);
                                }
                                _o = await Kontroler.Instance.Pretrazi(posiljalacUsername);
                                _k = (Korisnik)_o.Rezultat;
                                if (server.online.TryGetValue(primalacUsername, out var client1))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.PrihvatiPrijatelja;
                                    _z.Objekat = _k;
                                    await client1.PushAsync(_z);
                                }
                            }
                        }
                        break;
                    case Operacija.OdbijPrijatelja:
                        {
                            Prijateljstvo x = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.OdbijPrijatelja(x);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(x.korisnik2_id);
                                PrijateljstvoView zaSlanje = new PrijateljstvoView(primalacUsername, x);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.OdbijPrijatelja;
                                    _z.Objekat = zaSlanje;
                                    await client.PushAsync(_z);
                                }
                            }
                        }
                        break;
                    case Operacija.UcitajSvePoruke:
                        o.Rezultat = await Kontroler.Instance.UcitajSvePoruke(serializer.ReadType<Tuple<int, int>>((JsonElement)z.Objekat));
                        break;
                    case Operacija.ObrisiPrijateljstvo:
                        {
                            o.Uspesno = await Kontroler.Instance.ObrisiPrijateljstvo(serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat));
                            Prijateljstvo _p = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);

                            string posiljalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik1_id);
                            string primalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik2_id);
                            Odgovor _o = await Kontroler.Instance.Pretrazi(primalacUsername);
                            Korisnik _k = (Korisnik)_o.Rezultat;
                            if (server.online.TryGetValue(posiljalacUsername, out var client))
                            {
                                Zahtev _z = new Zahtev();
                                _z.Operacija = Operacija.ObrisiPrijateljstvo;
                                _z.Objekat = _k;
                                await client.PushAsync(_z);
                            }
                            _o = await Kontroler.Instance.Pretrazi(posiljalacUsername);
                            _k = (Korisnik)_o.Rezultat;
                            if (server.online.TryGetValue(primalacUsername, out var client1))
                            {
                                Zahtev _z = new Zahtev();
                                _z.Operacija = Operacija.ObrisiPrijateljstvo;
                                _z.Objekat = _k;
                                await client1.PushAsync(_z);
                            }
                        }
                        break;
                }
                if (o.Poruka == "logovan")
                    throw new Exception("login exception");
            }
            catch  { }
            return o;
        }
    }
}
