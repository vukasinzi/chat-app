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
            this.k.kriterijumWhere = $"korisnicko_ime = '{k.Korisnicko_ime}' and lozinka = '{k.Lozinka}'"


        }
        protected override void ExecuteConcreteOperation()
        {
            o.Rezultat = (Korisnik)broker.getCriteria(k);
            if (o.Rezultat != null)
                o.Uspesno = true;
        }
    }
}
