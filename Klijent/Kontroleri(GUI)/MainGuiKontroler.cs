using Klijent.Domen;
using System.Collections.ObjectModel;
using Zajednicki;
using Zajednicki.Domen;

namespace Klijent.Kontroleri_GUI_
{
    public class MainGuiKontroler
    {
        public static MainGuiKontroler instance;

        public static MainGuiKontroler Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainGuiKontroler();
                return instance;
            }
        }
        public ObservableCollection<Korisnik> Kontakti { get; set; } = new ObservableCollection<Korisnik>();
        public ObservableCollection<PrijateljstvoView> Prijateljstva { get; set; } = new ObservableCollection<PrijateljstvoView>();

        internal async Task<Korisnik?> Pretrazi(string msgtext)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(msgtext))
                    return null;

                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.Pretrazi(msgtext);
                if (o == null || !o.Uspesno)
                {
                    await DialogService.ShowMessageAsync("Ne postoji korisnik sa tim korisnickim imenom.");
                    return null;
                }

                Korisnik l = (Korisnik)o.Rezultat;
                return l;
            }
            catch
            {
                return null;
            }
        }

        internal async Task<string?> Pretrazi(int id)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.Pretrazi(id);
                if (o == null)
                    return null;
                return (string)o.Rezultat;
            }
            catch
            {
                return null;
            }
        }

        internal async Task VratiSvePrijatelje(Korisnik id)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.vratiSvePrijatelje(id);
                Kontakti.Clear();

                if (o != null && o.Rezultat != null)
                {
                    var lista = (List<Korisnik>)o.Rezultat;
                    foreach (var u in lista)
                        Kontakti.Add(u);
                }
            }
            catch
            {
                return;
            }
        }

        internal async Task<bool> Posalji(string poruka_text, int posiljalac,int primalac)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.Posalji(poruka_text, posiljalac,primalac);
                if (!o.Uspesno)
                {
                    await DialogService.ShowMessageAsync("Nemoguće poslati poruku");
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> DodajPrijatelja(int id, int id2)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.DodajPrijatelja(id,id2);
                if (o.Uspesno)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        internal async Task VratiZahtevePrijatelja(int id)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.VratiZahtevePrijatelja(id);
                Prijateljstva.Clear();
                if (o == null || o.Rezultat == null)
                    return;

                var lista = (List<Prijateljstvo>)o.Rezultat;
                foreach (var x in lista)
                {
                    Odgovor l;
                    if (id == x.korisnik1_id)
                        l = await Komunikacija.Instance.Pretrazi(x.korisnik2_id);
                    else
                        l = await Komunikacija.Instance.Pretrazi(x.korisnik1_id);
                    if (l != null && l.Rezultat != null && l.Rezultat is string)
                    {
                        PrijateljstvoView p = new PrijateljstvoView((string)l.Rezultat, x);
                        Prijateljstva.Add(p);
                    }
                }


            }
            catch
            {
                return;
            }
        }

        internal async Task<bool> PrihvatiPrijatelja(Prijateljstvo prijatelj)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.PrihvatiPrijatelja(prijatelj);
                if (o.Uspesno)
                    return true;

                return false;

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> OdbijPrijatelja(Prijateljstvo prijatelj)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.OdbijPrijatelja(prijatelj);
                if (o.Uspesno)
                    return true;

                return false;

            }
            catch
            {
                return false;
            }
        }

        internal async Task<List<Poruka>?> UcitajSvePoruke(Korisnik primalac, Korisnik k)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.UcitajSvePoruke(primalac,k);
               
                if (o.Rezultat != null)
                {
                    return (List<Poruka>)o.Rezultat;
                }
                return new List<Poruka>();

            }
            catch
            {
                return null;
            }
        }

        internal async Task<bool> ObrisiPrijateljstvo(int id1, int id2)
        {
            try
            {
                await Komunikacija.Instance.ConnectAsync();
                Odgovor o = await Komunikacija.Instance.ObrisiPrijateljstvo(id1, id2);

                return o.Uspesno;

            }
            catch
            {
                return false;
            }
        }
    }
}
