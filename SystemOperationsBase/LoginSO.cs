using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki;

namespace SO
{
    public class LoginSO : SystemOperationsBase
    {
        private Korisnik k;
        public bool Uspesno = false;
        public LoginSO(Korisnik k)
        {
            
            this.k = k;
            this.k.kriterijumWhere = $"korisnicko_ime = '{k.Korisnicko_ime}' and lozinka = '{k.Lozinka}'";
        }
        protected override void ExecuteConcreteOperation()
        {
            if ((Korisnik)broker.getCriteria(k) != null)
                Uspesno = true;
            

            
        }
    }
}
