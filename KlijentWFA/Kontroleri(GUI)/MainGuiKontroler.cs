using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public List<Korisnik> kontakti = new List<Korisnik>();
        public List<Prijateljstvo> prijatelji = new List<Prijateljstvo>();
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
                if (o.Rezultat != null)
                {
                    kontakti = (List<Korisnik>)o.Rezultat;
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

        internal async Task ProveriNovePrijatelje(int id)
        {
            try
            {
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.ProveriNovePrijatelje(id);
                if (o != null)
                    if (o.Rezultat != null)
                    {
                        prijatelji = (List<Prijateljstvo>)o.Rezultat;
                    }
                    else
                         return;
                     
                     
            }
            catch
            {
                return ;
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
                List<Poruka> poruke = new List<Poruka>();
                if (o.Rezultat != null)
                {
                    poruke = (List<Poruka>)o.Rezultat;
                }
                return poruke;

            }
            catch
            {
                return null;
            }
        }

        internal async Task<bool> ObrisiPrijatelja(Prijateljstvo p)
        {
            try
            {
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.ObrisiPrijatelja(p);
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
    }
}
