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
        internal async Task<bool> DodajPrijatelja(Prijateljstvo p)
        {
            SystemOperationsBase dod = new DodajPrijateljaSO(p);
            dod.Execute();
           return ((DodajPrijateljaSO)dod).Uspesno;

        }

        internal async Task<Odgovor> RegistrujSe(Korisnik korisnik)
        {
            SystemOperationsBase rog = new RegistrujSeSO(korisnik);
            rog.Execute();
            return ((RegistrujSeSO)rog).o;
        }

        internal async Task<List<Prijateljstvo>> VratiZahtevePrijatelja(int? id)
        {
            SystemOperationsBase vog = new VratiZahtevePrijateljaSO(id);
            vog.Execute();
            return ((VratiZahtevePrijateljaSO)vog).listaPrijatelja;

        }
        internal async Task<bool> PrihvatiPrijatelja(Prijateljstvo p)
        {
            SystemOperationsBase pog = new PrihvatiPrijateljaSO(p);
            pog.Execute();
            return ((PrihvatiPrijateljaSO)pog).Uspesno; 

        }

        internal async Task<bool> OdbijPrijatelja(Prijateljstvo prijateljstvo)
        {
            SystemOperationsBase oog = new OdbijPrijateljaSO(prijateljstvo);
            oog.Execute();
            return ((OdbijPrijateljaSO)oog).Uspesno;
        }

        internal async Task<List<Poruka>> UcitajSvePoruke(Tuple<int, int> tuple)
        {
            SystemOperationsBase uog = new UcitajSvePorukeSO(tuple);
            uog.Execute();
            return ((UcitajSvePorukeSO)uog).Lista;
        }
    }
}
