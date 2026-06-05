using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class PosaljiSO : SystemOperationsBase
    {
        Poruka p;
        public bool Uspesno = false;
        public PosaljiSO(Poruka p)
        {
            this.p = p;
            DateTime datum = DateTime.Now;
            p.vrednostiNaziv = "@primalac,@posiljalac,@poruka_text,@datum";
            p.parametri = new Dictionary<string, object?>
            {
                { "@primalac", p.primalac_id },
                { "@posiljalac", p.posiljalac_id },
                { "@poruka_text", p.poruka_text },
                { "@datum", datum }
            };
        }
        protected override void ExecuteConcreteOperation()
        {
            int a = broker.Insert(p);
            if (a > 0)
                Uspesno = true;
        }
    }
}
