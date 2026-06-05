using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zajednicki.Domen;

namespace SO
{
    public class VratiZahtevePrijateljaSO : SystemOperationsBase
    {
        int id;
        public List<Prijateljstvo> listaPrijatelja = new List<Prijateljstvo>();
        public VratiZahtevePrijateljaSO(int id)
        {
            this.id = id;
        }
        protected override async Task ExecuteConcreteOperationAsync(CancellationToken token = default)
        {
            Prijateljstvo p = new Prijateljstvo();
            p.korisnik2_id = id;
            p.kriterijumWhere = "korisnik2_id = @korisnik2_id and status = @status";
            p.parametri = new Dictionary<string, object?>
            {
                { "@korisnik2_id", p.korisnik2_id },
                { "@status", "ceka se" }
            };
            List<IObjekat> x = await broker.GetAllCriteriaAsync(p, token);
            if (x != null)
                listaPrijatelja = x.OfType<Prijateljstvo>().ToList();
            if (listaPrijatelja == null)
                Debug.WriteLine("Prazna lista sa prijateljima...");

        }
    }
}
