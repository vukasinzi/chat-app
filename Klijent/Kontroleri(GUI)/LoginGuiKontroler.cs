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
                if(!o.Uspesno)
                {
                    MessageBox.Show("Pogresni kredencijali. Pokusajte ponovo.");
                    return;
                }
                else
                {
                    MessageBox.Show("Uspesno logovanje. Dobrodosli - " + k.Korisnicko_ime + ".");
                    MainWindow window = new MainWindow(k);
                    window.ShowDialog();
                }
                
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }

        internal async Task RegistrujSe(string korisnicko_ime, string lozinka)
        {
            try
            {
                if(korisnicko_ime == "" || lozinka == "")
                {
                    MessageBox.Show("Morate uneti korisnicko ime i lozinku.");
                    return;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor dostupan = await Komunikacija.Instance.Dostupan(k);
                if (!dostupan.Uspesno)
                {
                    MessageBox.Show("Korisnicko ime je zauzeto.");
                    return;
                } 
                Odgovor o = await Komunikacija.Instance.RegistrujSe(k);
                if (o.Uspesno)
                {
                    MessageBox.Show("Uspesna registracija! Dobro dosli - " + k.Korisnicko_ime);
                    MainWindow mw = new MainWindow(k);
                }
                else
                    MessageBox.Show("Registracija nije uspela.");
            }
            catch(Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
    }
}
