using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki;

namespace SO
{
    public class RegistrujSeSO : SystemOperationsBase
    {
        private Korisnik k;
        public bool Uspesno;
        public RegistrujSeSO(Korisnik k)
        {
            this.k = k;
            k.vrednostiNaziv = $"'{k.Korisnicko_ime}','{k.Lozinka}'";
            
        }
        protected override void ExecuteConcreteOperation()
        {
            int a = broker.Insert(k);
            if (a > 0)
                Uspesno = true;
        }
    }
}
