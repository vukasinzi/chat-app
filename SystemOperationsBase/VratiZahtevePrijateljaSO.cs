using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class VratiZahtevePrijateljaSO : SystemOperationsBase
    {
        int id;
        public List<Prijateljstvo> listaPrijatelja = new List<Prijateljstvo>();
        public VratiZahtevePrijateljaSO(int?id)
        {
            this.id = (int)id;
        }
        protected override void ExecuteConcreteOperation()
        {
            Prijateljstvo p = new Prijateljstvo();
            p.korisnik2_id = id;
            p.kriterijumWhere = $"korisnik2_id = {p.korisnik2_id} and status = 'ceka se'";
            List<IObjekat> x = broker.GetAllCriteria(p);
             listaPrijatelja = x.OfType<Prijateljstvo>().ToList();
            if (listaPrijatelja == null)
                Debug.WriteLine("Prazna lista sa prijateljima...");

        }
    }
}
