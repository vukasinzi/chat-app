using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using Zajednicki;

namespace Klijent.Kontroleri_GUI_
{
    public class LoginGuiKontroler
    {
        public static LoginGuiKontroler instance;
        public static LoginGuiKontroler Instance
        {
            get
            {
                if (instance == null)
                    instance = new LoginGuiKontroler();
                return instance;
            }
        }
        internal async Task LogIn(string korisnicko_ime,string lozinka)
        {
            try
            {
                if (korisnicko_ime == "" || lozinka == "")
                {
                    MessageBox.Show("Morate uneti korisnicko ime i lozinku.");
                    return;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.LogInAsync(k);
                if (o.Poruka == "greska")
                {
                    MessageBox.Show("Greska pri logovanju.");
                    return;
                }
                if(o.Uspesno)
                    MessageBox.Show("Uspesno logovanje. Dobrodosli - " + k.Korisnicko_ime+".");
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }

    }
}
