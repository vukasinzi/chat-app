using Klijent.Domen;
using SO;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using Zajednicki;
using Zajednicki.Domen;

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

        internal async Task<Odgovor> LogIn(Korisnik korisnik)
        {
            SystemOperationsBase log = new LoginSO(korisnik);
            log.Execute();
            return ((LoginSO)log).o;

        }

        internal async Task<bool> Posalji(Poruka poruka)
        {
            SystemOperationsBase pos = new PosaljiSO(poruka);
            pos.Execute();
            return ((PosaljiSO)pos).Uspesno;
        }

        internal async Task<Odgovor> Pretrazi(string v)
        {
            SystemOperationsBase pob = new PretraziSO(v);
            pob.Execute();
            return ((PretraziSO)pob).o;
        }
        internal async Task<string> Pretrazi(int v)
        {
            SystemOperationsBase pob = new PretraziSO(v);
            pob.Execute();
            return ((PretraziSO)pob).prinm;
        }


        internal async Task<List<Korisnik>> Prijatelji(Korisnik id)
        {
            SystemOperationsBase pob = new VratiPrijateljeSO(id);
            pob.Execute();
            return ((VratiPrijateljeSO)pob).lista;

        }

        internal async Task<bool> RegistrujSe(Korisnik korisnik)
        {
            SystemOperationsBase rog = new RegistrujSeSO(korisnik);
            rog.Execute();
            return ((RegistrujSeSO)rog).Uspesno;
        }
    }
}
