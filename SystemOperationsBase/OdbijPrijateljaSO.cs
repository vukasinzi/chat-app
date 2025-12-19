using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class OdbijPrijateljaSO : SystemOperationsBase
    {
        Prijateljstvo p;
        public bool Uspesno = false;
        public OdbijPrijateljaSO(Prijateljstvo p)
        {
            this.p = p;
        }
        protected override void ExecuteConcreteOperation()
        {
           
            p.kriterijumWhere = $"status = 'ceka se' and korisnik1_id = {p.korisnik1_id} and korisnik2_id = {p.korisnik2_id}";
            int a = broker.DeleteCriteria(p);
            if (a > 0)
                Uspesno = true;
        }
    }
}
