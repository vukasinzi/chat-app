using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zajednicki;

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
        public List<Korisnik> prijatelji = new List<Korisnik>();

        internal async Task<Korisnik> Pretrazi(string msgtext)
        {
            try
            {
                if (msgtext.IsWhiteSpace())
                    throw new Exception("aaa");
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.Pretrazi(msgtext);
                if (!o.Uspesno)
                {
                    MessageBox.Show("Ne postoji korisnik sa tim korisnickim imenom.");
                    throw new Exception("aaa");
                }
                Korisnik l = (Korisnik)o.Rezultat;
                return l;

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
                    prijatelji = (List<Korisnik>)o.Rezultat;
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

        internal async Task DodajPrijatelja(int id, int id2)
        {
            try
            {
                Komunikacija.Instance.Connect();
                //Odgovor o = await Komunikacija.instance.DodajPrijatelja(id,id2);

            }
            catch (Exception x)
            {
                return;
            }
        }
    }
}
