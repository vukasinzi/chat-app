using Klijent.Domen;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
                    await DialogService.ShowMessageAsync("Morate uneti korisnicko ime i lozinku.");
                    return false;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor o = await Komunikacija.Instance.LogInAsync(k);
                if(o.Poruka == "logovan")
                {
                    await DialogService.ShowMessageAsync("Već je ulogovan korisnik sa pomenutim kredencijalima.");
                    return false;
                }
                if(!o.Uspesno)
                {
                    await DialogService.ShowMessageAsync("Pogrešni kredencijali. Pokušajte ponovo.");
                    return false;
                }
                else
                {
                    await DialogService.ShowMessageAsync("Uspešno logovanje. Dobro došli, " + k.Korisnicko_ime + ".");
                    Korisnik l = (Korisnik)o.Rezultat;
                    MainWindow window = new MainWindow(l);
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        desktop.MainWindow = window;
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
                    await DialogService.ShowMessageAsync("Morate uneti korisnicko ime i lozinku.");
                    return false;
                }
                Korisnik k = new Korisnik(korisnicko_ime, lozinka);
                Komunikacija.Instance.Connect();
                Odgovor dostupan = await Komunikacija.Instance.Dostupan(k);
                if (!dostupan.Uspesno)
                {
                    await DialogService.ShowMessageAsync("Korisničko ime je zauzeto.");
                    return false;
                }
                Odgovor o = await Komunikacija.Instance.RegistrujSe(k);
                if (o.Uspesno)
                {
                    await DialogService.ShowMessageAsync("Uspešna registracija. Dobro došli, " + k.Korisnicko_ime + ".");
                    Korisnik l = (Korisnik)o.Rezultat;
                    MainWindow window = new MainWindow(l);
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        desktop.MainWindow = window;
                    window.Show();
                    return true;
                }
                else
                {
                    await DialogService.ShowMessageAsync("Registracija nije uspela.");
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
