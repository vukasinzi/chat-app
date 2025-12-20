using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
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
        internal async Task<bool> LogIn(string korisnicko_ime,string lozinka)
        {
            try
            {
                if (korisnicko_ime == "" || lozinka == "")
                {
                    MessageBox.Show("Morate uneti korisnicko ime i lozinku.");
                    return false;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.LogInAsync(k);
                if(o.Poruka == "logovan")
                {
                    MessageBox.Show("Vec je ulogovan korisnik sa pomenutim kredencijalima.");
                    return false;
                }
                if(!o.Uspesno)
                {
                    MessageBox.Show("Pogresni kredencijali. Pokusajte ponovo.");
                    return false;
                }
                else
                {
                    MessageBox.Show("Uspesno logovanje. Dobrodosli - " + k.Korisnicko_ime + ".");
                    Korisnik l = (Korisnik)o.Rezultat;
                    MainWindow window = new MainWindow(l);
                    window.Show();
                    return true;
                }
                    
                
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
            return false;
        }

        internal async Task<bool> RegistrujSe(string korisnicko_ime, string lozinka)
        {
            try
            {
                if (korisnicko_ime == "" || lozinka == "")
                {
                    MessageBox.Show("Morate uneti korisnicko ime i lozinku.");
                    return false;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor dostupan = await Komunikacija.Instance.Dostupan(k);
                if (!dostupan.Uspesno)
                {
                    MessageBox.Show("Korisnicko ime je zauzeto.");
                    return false;
                }
                Odgovor o = await Komunikacija.Instance.RegistrujSe(k);
                if (o.Uspesno)
                {
                    Korisnik l = (Korisnik)o.Rezultat;
                    MainWindow window = new MainWindow(l);
                    window.Show();
                    return true;
                }
                else
                {
                    MessageBox.Show("Registracija nije uspela.");
                    return false;
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
                return false;
        }
    }
}
