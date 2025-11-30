
using Klijent.Domen;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Zajednicki;

namespace Server
{
    public class ClientHandler
    {
        public Socket socket;
        private Server server;
        private JsonNetworkSerializer serializer;

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
                    Odgovor o = await ProcessRequest(z);
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
                server.RemoveClient(this);
            }
        }

        private async Task<Odgovor> ProcessRequest(Zahtev z)
        {
            Odgovor o = new Odgovor();
            try
            {
                switch(z.Operacija)
                {
                    case Operacija.LogIn:
                       o = await Kontroler.Instance.LogIn(serializer.ReadType<Korisnik>((JsonElement)z.Objekat));
                        break;
                }
            }
            catch { }
            if (o.Poruka == null && o.Rezultat == null)
                return null;
            return o;
        }
    }
}
