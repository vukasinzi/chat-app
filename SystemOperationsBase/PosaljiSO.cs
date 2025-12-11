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
            p.vrednostiNaziv = $"{p.primalac_id},{p.posiljalac_id},'{p.poruka_text}','{datum:yyyy-MM-dd HH:mm:ss}'";
        }
        protected override void ExecuteConcreteOperation()
        {
            int a = broker.Insert(p);
            if (a > 0)
                Uspesno = true;
        }
    }
}
