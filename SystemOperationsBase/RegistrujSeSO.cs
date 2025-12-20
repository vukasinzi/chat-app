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
        public Odgovor o = new Odgovor();
        public RegistrujSeSO(Korisnik k)
        {
            this.k = k;
            this.k.kriterijumWhere = $"korisnicko_ime = '{k.Korisnicko_ime}' and lozinka = '{k.Lozinka}'";
            this.k.vrednostiNaziv = $"'{k.Korisnicko_ime}','{k.Lozinka}'";

        }
        protected override void ExecuteConcreteOperation()
        {
            int a = broker.Insert(k);
            if (a > 0)
            {
                o.Rezultat = (Korisnik)broker.getCriteria(k);
                if (o.Rezultat != null)
                    o.Uspesno = true;
            }
                
           
        }
    }
}
