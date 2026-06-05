using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class ObrisiPrijateljstvoSO : SystemOperationsBase
    {
        Prijateljstvo prijateljstvo;
        public bool Uspesno = false;
        public ObrisiPrijateljstvoSO(Prijateljstvo p)
        {
            prijateljstvo = p;
            prijateljstvo.kriterijumWhere = "(korisnik1_id = @korisnik1_id and korisnik2_id = @korisnik2_id)" +
                "or (korisnik1_id = @korisnik2_id and korisnik2_id = @korisnik1_id)";
            prijateljstvo.parametri = new Dictionary<string, object?>
            {
                { "@korisnik1_id", prijateljstvo.korisnik1_id },
                { "@korisnik2_id", prijateljstvo.korisnik2_id }
            };

        }
        protected override void ExecuteConcreteOperation()
        {
            int a = broker.DeleteCriteria(prijateljstvo);
            if (a > 0)
                Uspesno = true;

        }
    }
}
