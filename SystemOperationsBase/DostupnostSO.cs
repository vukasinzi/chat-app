using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;

namespace SO
{
    public class DostupnostSO : SystemOperationsBase
    {
        private Korisnik k;
        public bool Uspesno = false;
        public DostupnostSO(Korisnik k)
        {
            this.k = k;
            k.kriterijumWhere = $"korisnicko_ime = '{k.Korisnicko_ime}'";
        }
        protected override void ExecuteConcreteOperation()
        {
            broker.getCriteria(k);
            if ((Korisnik)broker.getCriteria(k) == null)
                Uspesno = true;
        }
    }
}
