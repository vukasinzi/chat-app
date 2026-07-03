using Klijent.Domen;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        public event Action<Zahtev>? PushPrimljen;
        
        //semafor
        private SemaphoreSlim requestLock = new SemaphoreSlim(1, 1);
        private const string ExpectedServerThumbprint = "48:E1:11:F8:31:9C:E2:E6:C6:06:4B:36:97:5C:6A:9F:F2:63:94:54";

        private Komunikacija()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ip = config.GetRequiredSection("ServerSettings").GetValue<string>("Ip")
                ?? throw new InvalidOperationException("ServerSettings:Ip nije podesen.");
    
            port = config.GetRequiredSection("ServerSettings").GetValue<int>("Port");
            pushPort = config.GetRequiredSection("ServerSettings").GetValue<int>("PushPort");
        }

        //provera sertifikata
        private static bool ValidateServerCertificate( object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
        {
            if (certificate == null)
                return false;

            var cert2 = new X509Certificate2(certificate);

            string expectedThumbprint = ExpectedServerThumbprint
                .Replace(":", "")
                .Replace(" ", "")
                .ToUpperInvariant();

            string actualThumbprint = cert2.Thumbprint
                .Replace(":", "")
                .Replace(" ", "")
                .ToUpperInvariant();

            return actualThumbprint == expectedThumbprint;

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

            await requestLock.WaitAsync(token);
            try
            {
                if (!isConnected())
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(ip, port, token);
                    var networkStream = new NetworkStream(socket, ownsSocket: false);
                    var sslStream = new SslStream(
                        networkStream,
                        leaveInnerStreamOpen: false,
                        userCertificateValidationCallback: (sender, certificate, chain, errors) => ValidateServerCertificate(sender, certificate, chain, errors)
                    );

                    await sslStream.AuthenticateAsClientAsync("localhost");

                    serializer = new JsonNetworkSerializer(sslStream);

                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                throw;
            }
            finally
            { 
                requestLock.Release();
            }
        }
        internal async Task<Odgovor> LogInAsync(Korisnik k, CancellationToken token = default)
        {
                await requestLock.WaitAsync(token);
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
            finally
            { 
                requestLock.Release();
            }
        }

        private async Task PosaljiUsername(string v, CancellationToken token)
        {
            if (pushSocket != null && pushSocket.Connected) return;

            pushSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await pushSocket.ConnectAsync(ip, pushPort, token);
            var networkStream = new NetworkStream(pushSocket, ownsSocket: false);
            var sslStream = new SslStream(
                networkStream,
                leaveInnerStreamOpen: false,
                userCertificateValidationCallback: (sender, certificate, chain, errors) => ValidateServerCertificate(sender, certificate, chain, errors)
            );

            await sslStream.AuthenticateAsClientAsync("localhost");

            pushSerializer = new JsonNetworkSerializer(sslStream);
            await pushSerializer.SendAsync(v, token);


            pushCts = new CancellationTokenSource();
            _ = Task.Run(() => HandlePush(pushCts.Token), pushCts.Token);
        }

        private async Task HandlePush(CancellationToken token)
        {
            while(!token.IsCancellationRequested && pushSocket != null&&  pushSocket.Connected)
            {
                Zahtev z = await pushSerializer.ReceiveAsync<Zahtev>(token);
                DeserijalizujPushObjekat(z);
                PushPrimljen?.Invoke(z);

            }
        }

        private void DeserijalizujPushObjekat(Zahtev z)
        {
            if (z.Objekat is not JsonElement element)
                return;

            switch (z.Operacija)
            {
                case Operacija.Posalji:
                    z.Objekat = pushSerializer.ReadType<Poruka>(element);
                    break;

                case Operacija.DodajPrijatelja:
                    z.Objekat = pushSerializer.ReadType<PrijateljstvoView>(element);
                    break;

                case Operacija.PrihvatiPrijatelja:
                    z.Objekat = pushSerializer.ReadType<Korisnik>(element);
                    break;

                case Operacija.ObrisiPrijateljstvo:
                    z.Objekat = pushSerializer.ReadType<Korisnik>(element);
                    break;

                case Operacija.OdbijPrijatelja:
                    z.Objekat = pushSerializer.ReadType<PrijateljstvoView>(element);
                    break;
            }
        }

        internal async Task<Odgovor> RegistrujSe(Korisnik k, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> Dostupan(Korisnik k, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> Pretrazi(string text, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }
        internal async Task<Odgovor> Pretrazi(int id, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> vratiSvePrijatelje(Korisnik id, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }

        }

        internal async Task<Odgovor> Posalji(string pt,int posiljalac,int primalac, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> DodajPrijatelja(int id, int id2, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> VratiZahtevePrijatelja(int id, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> PrihvatiPrijatelja(Prijateljstvo prijatelj, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> OdbijPrijatelja(Prijateljstvo prijatelj, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> UcitajSvePoruke(Korisnik primalac, Korisnik k, CancellationToken token = default)
        {
            
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }

        internal async Task<Odgovor> ObrisiPrijateljstvo(int id1, int id2, CancellationToken token = default)
        {
            await requestLock.WaitAsync(token);

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
            finally
            { 
                requestLock.Release();
            }
        }
    }
}
