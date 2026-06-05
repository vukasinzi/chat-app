using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class VratiPrijateljeSO : SystemOperationsBase
    {
        private Korisnik id;
        public List<Korisnik> lista = new List<Korisnik>();
        Prijateljstvo p = new Prijateljstvo();
        public VratiPrijateljeSO(Korisnik k)
        {
            id = k;
            p.korisnik1_id = k.Id;
            p.kriterijumWhere = "(korisnik1_id = @korisnik_id OR korisnik2_id = @korisnik_id) and status = @status";
            p.parametri = new Dictionary<string, object?>
            {
                { "@korisnik_id", p.korisnik1_id },
                { "@status", "prihvacen" }
            };
        }
        protected override void ExecuteConcreteOperation()
        {
            List<IObjekat> x = broker.GetAllCriteria(p);

            if (x != null)
            {
                List<Prijateljstvo> listaPrijatelja = x.OfType<Prijateljstvo>().ToList();
               foreach(Prijateljstvo y in listaPrijatelja)
                {
                    if(y.korisnik1_id == p.korisnik1_id)
                    {
                        Korisnik privremeni = new Korisnik();
                        privremeni.Id = y.korisnik2_id;
                        privremeni.kriterijumWhere = "id = @id";
                        privremeni.parametri = new Dictionary<string, object?>
                        {
                            { "@id", privremeni.Id }
                        };
                        if (broker.getCriteria(privremeni) is Korisnik k) lista.Add(k);
                    }
                    else if (y.korisnik2_id == p.korisnik1_id)
                    {
                        Korisnik privremeni = new Korisnik();
                        privremeni.Id = y.korisnik1_id;
                        privremeni.kriterijumWhere = "id = @id";
                        privremeni.parametri = new Dictionary<string, object?>
                        {
                            { "@id", privremeni.Id }
                        };
                        if (broker.getCriteria(privremeni) is Korisnik k) lista.Add(k);
                    }
                }
            }
        }
    }
}
