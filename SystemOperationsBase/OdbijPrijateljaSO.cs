using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
        {
           
            p.kriterijumWhere = "status = @status and korisnik1_id = @korisnik1_id and korisnik2_id = @korisnik2_id";
            p.parametri = new Dictionary<string, object?>
            {
                { "@status", "ceka se" },
                { "@korisnik1_id", p.korisnik1_id },
                { "@korisnik2_id", p.korisnik2_id }
            };
            int a = await broker.DeleteCriteriaAsync(p, token);
            if (a > 0)
                Uspesno = true;
        }
    }
}
