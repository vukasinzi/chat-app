
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
                            server.onlineUsers.Add(l.Korisnicko_ime.ToString());
                        currentUser = l.Korisnicko_ime.ToString();

                        break;
                    case Operacija.Dostupnost:
                        o.Uspesno = await Kontroler.Instance.Dostupan(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.RegistrujSe:
                        o.Uspesno = await Kontroler.Instance.RegistrujSe(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Pretraga:
                        o = await Kontroler.Instance.Pretrazi(serializer.ReadType<string>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Prijatelji:
                        o.Rezultat = await Kontroler.Instance.Prijatelji(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                    case Operacija.Posalji:
                        o.Uspesno = await Kontroler.Instance.Posalji(serializer.ReadType<Poruka>((JsonElement)z.Objekat));
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
