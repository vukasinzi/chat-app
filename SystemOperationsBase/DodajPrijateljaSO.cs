using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
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
            if (await broker.getCriteriaAsync(p, token) == null)
            {

                int a = await broker.InsertAsync(p, token);

                if (a > 0)
                    Uspesno = true;
            }
        }
    }
}
