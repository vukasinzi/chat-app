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
            p.kriterijumWhere = $"(korisnik1_id LIKE {p.korisnik1_id} AND korisnik2_id LIKE {p.korisnik2_id}) AND " +
                $"(korisnik1_id LIKE {p.korisnik2_id} AND korisnik2_id LIKE {p.korisnik1_id})";
            if (broker.getCriteria(p) == null)
            {

                int a = broker.Insert(p);

                if (a > 0)
                    Uspesno = true;
            }
        }
    }
}
