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
            p.vrednostiNaziv = "@korisnik1_id,@korisnik2_id,@status";
            p.kriterijumWhere = "(korisnik1_id = @korisnik1_id AND korisnik2_id = @korisnik2_id) OR " +
                "(korisnik1_id = @korisnik2_id AND korisnik2_id = @korisnik1_id)";
            p.parametri = new Dictionary<string, object?>
            {
                { "@korisnik1_id", p.korisnik1_id },
                { "@korisnik2_id", p.korisnik2_id },
                { "@status", "ceka se" }
            };
            if (broker.getCriteria(p) == null)
            {

                int a = broker.Insert(p);

                if (a > 0)
                    Uspesno = true;
            }
        }
    }
}
