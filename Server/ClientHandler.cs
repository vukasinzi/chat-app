
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
        private string currentUser = "";
        private JsonNetworkSerializer serializer;
        private Socket? pushSocket;
        private JsonNetworkSerializer? pushSerializer;

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
                    Console.WriteLine((z.Operacija));
                    Odgovor o = await ProcessRequests(z, token);
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
                catch (Exception x)
                {
                    Debug.WriteLine(x.Message);
                }
                server.RemoveClient(this,currentUser);
                
            }
        }

        public Task PushAsync(Zahtev z, CancellationToken token = default)
        {
            if (pushSocket == null || !pushSocket.Connected || pushSerializer == null)
                return Task.CompletedTask;

            return pushSerializer.SendAsync(z, token);
        }

        private async Task<Odgovor> ProcessRequests(Zahtev z, CancellationToken token)
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
                        o = await Kontroler.Instance.LogIn(l, token);
                        if (o.Uspesno)
                        {
                            server.AddClient(this, l.Korisnicko_ime.ToString());
                            currentUser = l.Korisnicko_ime.ToString();
                           
                        }
                        
                        break;
                    case Operacija.Dostupnost:
                        o.Uspesno = await Kontroler.Instance.Dostupan(serializer.ReadType<Korisnik>((JsonElement)z.Objekat), token);
                        break;
                    case Operacija.RegistrujSe:
                        o = await Kontroler.Instance.RegistrujSe(serializer.ReadType<Korisnik>((JsonElement)z.Objekat), token);
                        break;
                    case Operacija.Pretraga:
                        o = await Kontroler.Instance.Pretrazi(serializer.ReadType<string>((JsonElement)z.Objekat), token);
                        break;
                    case Operacija.Pretraga2:
                        int? id = serializer.ReadTypeStruct<int>((JsonElement)z.Objekat);
                        if (id != null)
                        {
                            string username = await Kontroler.Instance.Pretrazi(id.Value, token);
                            if (!string.IsNullOrWhiteSpace(username))
                            {
                                o.Rezultat = username;
                                o.Uspesno = true;
                            }
                            else
                            {
                                o.Poruka = "Korisnik nije pronađen.";
                            }
                        }
                        else
                            o.Poruka = "Neispravan id za pretragu.";
                        break;
                    case Operacija.Prijatelji:
                        o.Rezultat = await Kontroler.Instance.Prijatelji(serializer.ReadType<Korisnik>((JsonElement)z.Objekat), token);
                        o.Uspesno = true;
                        break;
                    case Operacija.Posalji:
                        {
                            Poruka p = serializer.ReadType<Poruka>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.Posalji(p, token);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(p.primalac_id, token);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    await client.PushAsync(z, token);
                                }
                            }
                        }
                        break;
                    case Operacija.DodajPrijatelja:
                        {
                            Prijateljstvo x = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.DodajPrijatelja(x, token);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(x.korisnik2_id, token);
                                PrijateljstvoView zaSlanje = new PrijateljstvoView(primalacUsername, x);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.DodajPrijatelja;
                                    _z.Objekat = zaSlanje;
                                    await client.PushAsync(_z, token);
                                }
                            }
                        }
                        break;
                    case Operacija.VratiZahtevePrijatelja:
                        int? korisnikId = serializer.ReadTypeStruct<int>((JsonElement)z.Objekat);
                        if (korisnikId != null)
                        {
                            o.Rezultat = await Kontroler.Instance.VratiZahtevePrijatelja(korisnikId.Value, token);
                            o.Uspesno = true;
                        }
                        else
                            o.Poruka = "Neispravan id korisnika.";
                        break;
                    case Operacija.PrihvatiPrijatelja:
                        {
                            o.Uspesno = await Kontroler.Instance.PrihvatiPrijatelja(serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat), token);
                            if (o.Uspesno)
                            {
                                Prijateljstvo _p = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);

                                string posiljalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik1_id, token);
                                string primalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik2_id, token);
                                Odgovor _o = await Kontroler.Instance.Pretrazi(primalacUsername, token);
                                if (_o.Rezultat is Korisnik _k && server.online.TryGetValue(posiljalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.PrihvatiPrijatelja;
                                    _z.Objekat = _k;
                                    await client.PushAsync(_z, token);
                                }
                                _o = await Kontroler.Instance.Pretrazi(posiljalacUsername, token);
                                if (_o.Rezultat is Korisnik _k1 && server.online.TryGetValue(primalacUsername, out var client1))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.PrihvatiPrijatelja;
                                    _z.Objekat = _k1;
                                    await client1.PushAsync(_z, token);
                                }
                            }
                        }
                        break;
                    case Operacija.OdbijPrijatelja:
                        {
                            Prijateljstvo x = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);
                            o.Uspesno = await Kontroler.Instance.OdbijPrijatelja(x, token);
                            if (o.Uspesno)
                            {
                                string primalacUsername = await Kontroler.Instance.Pretrazi(x.korisnik2_id, token);
                                PrijateljstvoView zaSlanje = new PrijateljstvoView(primalacUsername, x);
                                if (server.online.TryGetValue(primalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.OdbijPrijatelja;
                                    _z.Objekat = zaSlanje;
                                    await client.PushAsync(_z, token);
                                }
                            }
                        }
                        break;
                    case Operacija.UcitajSvePoruke:
                        o.Rezultat = await Kontroler.Instance.UcitajSvePoruke(serializer.ReadType<Tuple<int, int>>((JsonElement)z.Objekat), token);
                        o.Uspesno = true;
                        break;
                    case Operacija.ObrisiPrijateljstvo:
                        {
                            o.Uspesno = await Kontroler.Instance.ObrisiPrijateljstvo(serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat), token);
                            Prijateljstvo _p = serializer.ReadType<Prijateljstvo>((JsonElement)z.Objekat);

                            if (o.Uspesno)
                            {
                                string posiljalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik1_id, token);
                                string primalacUsername = await Kontroler.Instance.Pretrazi(_p.korisnik2_id, token);
                                Odgovor _o = await Kontroler.Instance.Pretrazi(primalacUsername, token);
                                if (_o.Rezultat is Korisnik _k && server.online.TryGetValue(posiljalacUsername, out var client))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.ObrisiPrijateljstvo;
                                    _z.Objekat = _k;
                                    await client.PushAsync(_z, token);
                                }
                                _o = await Kontroler.Instance.Pretrazi(posiljalacUsername, token);
                                if (_o.Rezultat is Korisnik _k1 && server.online.TryGetValue(primalacUsername, out var client1))
                                {
                                    Zahtev _z = new Zahtev();
                                    _z.Operacija = Operacija.ObrisiPrijateljstvo;
                                    _z.Objekat = _k1;
                                    await client1.PushAsync(_z, token);
                                }
                            }
                        }
                        break;
                    default:
                        o.Poruka = "Nepoznata operacija.";
                        break;
                }
                if (string.IsNullOrWhiteSpace(o.Poruka) && !o.Uspesno && o.Rezultat == null)
                    o.Poruka = "Operacija nije uspela.";
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                o.Uspesno = false;
                o.Poruka = x.Message;
            }
            return o;
        }
    }
}
