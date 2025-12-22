using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        internal async Task<Korisnik> Pretrazi(string msgtext)
        {
            try
            {
                if (msgtext.IsWhiteSpace())
                    throw new Exception("aaa");
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.Pretrazi(msgtext);
                if (o == null || !o.Uspesno)
                {
                    MessageBox.Show("Ne postoji korisnik sa tim korisnickim imenom.");
                    throw new Exception("aaa");
                    return null;
                }
                Korisnik l = (Korisnik)o.Rezultat;
                return l;

            }
            catch (Exception x)
            {
                return null;
            }
        }
        internal async Task<string> Pretrazi(int id)
        {
            try
            {
               
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.Pretrazi(id);
                if (o == null)
                    return null;
                return (string)o.Rezultat;
               

            }
            catch (Exception x)
            {
                return null;
            }
        }
        internal async Task vratiSvePrijatelje(Korisnik id)
        {
            try
            {
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.vratiSvePrijatelje(id);
                Kontakti.Clear();

                if (o != null && o.Rezultat != null)
                {
                    var lista = (List<Korisnik>)o.Rezultat;
                    foreach (var u in lista)
                        Kontakti.Add(u);
                }
            }
            catch(Exception x)
            {
                return;
            }
        }

        internal async Task Posalji(string poruka_text, int posiljalac,int primalac)
        {
            try
            {
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.Posalji(poruka_text, posiljalac,primalac);
                if (!o.Uspesno)
                    MessageBox.Show("Nemoguce poslati poruku");

                
            }
            catch(Exception x)
            {
                return;
            }
        }

        internal async Task<bool> DodajPrijatelja(int id, int id2)
        {
            try
            {
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.DodajPrijatelja(id,id2);
                if (o.Uspesno)
                    return true;
                else
                    throw new Exception("umri");
            }
            catch (Exception x)
            {
                return false;
            }
        }

        internal async Task VratiZahtevePrijatelja(int id)
        {
            try
            {
                Komunikacija.Instance.Connect();
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
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.PrihvatiPrijatelja(prijatelj);
                if (o.Uspesno)
                    return true;
                else
                    throw new Exception("umri");

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
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.OdbijPrijatelja(prijatelj);
                if (o.Uspesno)
                    return true;
                else
                    throw new Exception("umri");

            }
            catch
            {
                return false;
            }
        }

        internal async Task<List<Poruka>> ucitajSvePoruke(Korisnik primalac, Korisnik k)
        {
            try
            {
                Komunikacija.Instance.Connect();
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
                Komunikacija.Instance.Connect();
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
