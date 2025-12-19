using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class PrihvatiPrijateljaSO : SystemOperationsBase
    {
        Prijateljstvo p;
        public bool Uspesno = false;
        public PrihvatiPrijateljaSO(Prijateljstvo p)
        {
            this.p = p;
        }
        protected override void ExecuteConcreteOperation()
        {
            p.vrednostiNaziv = $"status = 'prihvacen'";
            p.kriterijumWhere = $"status = 'ceka se' and korisnik1_id = {p.korisnik1_id} and korisnik2_id = {p.korisnik2_id}";
            int a =broker.UpdateCriteria(p);
            if (a > 0)
                Uspesno = true;
        }
    }
}
