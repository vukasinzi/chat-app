using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
        {
            p.vrednostiNaziv = "status = @novi_status";
            p.kriterijumWhere = "status = @stari_status and korisnik1_id = @korisnik1_id and korisnik2_id = @korisnik2_id";
            p.parametri = new Dictionary<string, object?>
            {
                { "@novi_status", "prihvacen" },
                { "@stari_status", "ceka se" },
                { "@korisnik1_id", p.korisnik1_id },
                { "@korisnik2_id", p.korisnik2_id }
            };
            int a = await broker.UpdateCriteriaAsync(p, token);
            if (a > 0)
                Uspesno = true;
        }
    }
}
