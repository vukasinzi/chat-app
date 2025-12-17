using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class DodajPrijateljaSO : SystemOperationsBase
    {
        Prijateljstvo p;
        public bool Uspesno = false;
        public DodajPrijateljaSO(Prijateljstvo p)
        {
            this.p = p;
        }
        protected override void ExecuteConcreteOperation()
        {
            p.vrednostiNaziv = $"{p.korisnik1_id},{p.korisnik2_id},'ceka se'";
            p.kriterijumWhere = $"({p.korisnik1_id} NOT LIKE korisnik1_id AND {p.korisnik2_id} NOT LIKE korisnik2_id) AND " +
                $"({p.korisnik2_id} NOT LIKE korisnik1_id AND {p.korisnik1_id} NOT LIKE korisnik2_id)";
            if (broker.getCriteria(p) == null)
            {

                int a = broker.Insert(p);

                if (a > 0)
                    Uspesno = true;
            }
        }
    }
}
