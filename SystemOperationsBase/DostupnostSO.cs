using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO
{
    public class DostupnostSO : SystemOperationsBase
    {
        private Korisnik k;
        public bool Uspesno = false;
        public DostupnostSO(Korisnik k)
        {
            this.k = k;
            k.kriterijumWhere = "korisnicko_ime = @korisnicko_ime";
            k.parametri = new Dictionary<string, object?>
            {
                { "@korisnicko_ime", k.Korisnicko_ime }
            };
        }
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
        {
            if ((Korisnik)await broker.getCriteriaAsync(k, token) == null)
                Uspesno = true;
        }
    }
}
