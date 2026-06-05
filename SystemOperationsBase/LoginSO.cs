using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zajednicki;

namespace SO
{
    public class LoginSO : SystemOperationsBase
    {
        private Korisnik k;
        public Odgovor o = new Odgovor();
        public LoginSO(Korisnik k)
        {
            
            this.k = k;
            this.k.kriterijumWhere = "korisnicko_ime = @korisnicko_ime and lozinka = @lozinka";
            this.k.parametri = new Dictionary<string, object?>
            {
                { "@korisnicko_ime", k.Korisnicko_ime },
                { "@lozinka", k.Lozinka }
            };
        }
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
        {
            o.Rezultat = (Korisnik)await broker.getCriteriaAsync(k, token);
            if (o.Rezultat != null)
                o.Uspesno = true;
            
            

            
        }
    }
}
