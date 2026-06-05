using Klijent.Domen;
using SO;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using Zajednicki;
using Zajednicki.Domen;

namespace Server
{
    public class Kontroler
    {
        public static Kontroler? instance;
        public static Kontroler Instance
        {
            get
            {
                if (instance == null)
                    instance = new Kontroler();
                return instance;
            }
        }

        internal async Task<bool> Dostupan(Korisnik korisnik, CancellationToken token = default)
        {
            SystemOperationsBase dos = new DostupnostSO(korisnik);
            await dos.ExecuteAsync(token);
            return ((DostupnostSO)dos).Uspesno;
        }

        internal async Task<Odgovor> LogIn(Korisnik korisnik, CancellationToken token = default)
        {
            SystemOperationsBase log = new LoginSO(korisnik);
            await log.ExecuteAsync(token);
            return ((LoginSO)log).o;

        }

        internal async Task<bool> Posalji(Poruka poruka, CancellationToken token = default)
        {
            SystemOperationsBase pos = new PosaljiSO(poruka);
            await pos.ExecuteAsync(token);
            return ((PosaljiSO)pos).Uspesno;
        }

        internal async Task<Odgovor> Pretrazi(string v, CancellationToken token = default)
        {
            SystemOperationsBase pob = new PretraziSO(v);
            await pob.ExecuteAsync(token);
            return ((PretraziSO)pob).o;
        }

        internal async Task<string> Pretrazi(int v, CancellationToken token = default)
        {
            SystemOperationsBase pob = new PretraziSO(v);
            await pob.ExecuteAsync(token);
            return ((PretraziSO)pob).prinm;
        }


        internal async Task<List<Korisnik>> Prijatelji(Korisnik id, CancellationToken token = default)
        {
            SystemOperationsBase pob = new VratiPrijateljeSO(id);
            await pob.ExecuteAsync(token);
            return ((VratiPrijateljeSO)pob).lista;

        }
        internal async Task<bool> DodajPrijatelja(Prijateljstvo p, CancellationToken token = default)
        {
            SystemOperationsBase dod = new DodajPrijateljaSO(p);
            await dod.ExecuteAsync(token);
           return ((DodajPrijateljaSO)dod).Uspesno;

        }

        internal async Task<Odgovor> RegistrujSe(Korisnik korisnik, CancellationToken token = default)
        {
            SystemOperationsBase rog = new RegistrujSeSO(korisnik);
            await rog.ExecuteAsync(token);
            return ((RegistrujSeSO)rog).o;
        }

        internal async Task<List<Prijateljstvo>> VratiZahtevePrijatelja(int id, CancellationToken token = default)
        {
            SystemOperationsBase vog = new VratiZahtevePrijateljaSO(id);
            await vog.ExecuteAsync(token);
            return ((VratiZahtevePrijateljaSO)vog).listaPrijatelja;

        }
        internal async Task<bool> PrihvatiPrijatelja(Prijateljstvo p, CancellationToken token = default)
        {
            SystemOperationsBase pog = new PrihvatiPrijateljaSO(p);
            await pog.ExecuteAsync(token);
            return ((PrihvatiPrijateljaSO)pog).Uspesno; 

        }

        internal async Task<bool> OdbijPrijatelja(Prijateljstvo prijateljstvo, CancellationToken token = default)
        {
            SystemOperationsBase oog = new OdbijPrijateljaSO(prijateljstvo);
            await oog.ExecuteAsync(token);
            return ((OdbijPrijateljaSO)oog).Uspesno;
        }

        internal async Task<List<Poruka>> UcitajSvePoruke(Tuple<int, int> tuple, CancellationToken token = default)
        {
            SystemOperationsBase uog = new UcitajSvePorukeSO(tuple);
            await uog.ExecuteAsync(token);
            return ((UcitajSvePorukeSO)uog).Lista;
        }

        internal async Task<bool> ObrisiPrijateljstvo(Prijateljstvo prijateljstvo, CancellationToken token = default)
        {
            SystemOperationsBase oog = new ObrisiPrijateljstvoSO(prijateljstvo);
            await oog.ExecuteAsync(token);
            return ((ObrisiPrijateljstvoSO)oog).Uspesno;
        }
    }
}
