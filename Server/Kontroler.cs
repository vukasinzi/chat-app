using Klijent.Domen;
using SO;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using Zajednicki;

namespace Server
{
    public class Kontroler
    {
        public static Kontroler instance;
        public static Kontroler Instance
        {
            get
            {
                if (instance == null)
                    instance = new Kontroler();
                return instance;
            }
        }

        internal async Task<bool> Dostupan(Korisnik korisnik)
        {
            SystemOperationsBase dos = new DostupnostSO(korisnik);
            dos.Execute();
            return ((DostupnostSO)dos).Uspesno;
        }

        internal async Task<bool> LogIn(Korisnik korisnik)
        {
            SystemOperationsBase log = new LoginSO(korisnik);
            log.Execute();
            return ((LoginSO)log).Uspesno;

        }
        internal async Task<bool> RegistrujSe(Korisnik korisnik)
        {
            SystemOperationsBase rog = new RegistrujSeSO(korisnik);
            rog.Execute();
            return ((RegistrujSeSO)rog).Uspesno;
        }
    }
}
