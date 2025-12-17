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
            p.kriterijumWhere = $"(korisnik1_id = {p.korisnik1_id} OR korisnik2_id = {p.korisnik1_id}) and status = 'prihvacen'";
        }
        protected override void ExecuteConcreteOperation()
        {
            List<IObjekat> x = broker.GetAllCriteria(p);
            List<Prijateljstvo> listaPrijatelja = x.OfType<Prijateljstvo>().ToList();

            if (x != null)
            {
               foreach(Prijateljstvo y in listaPrijatelja)
                {
                    if(y.korisnik1_id == p.korisnik1_id)
                    {
                        Korisnik privremeni = new Korisnik();
                        privremeni.Id = y.korisnik2_id;
                        privremeni.kriterijumWhere = $"id = {privremeni.Id}";
                        if (broker.getCriteria(privremeni) is Korisnik k) lista.Add(k);
                    }
                    else if (y.korisnik2_id == p.korisnik1_id)
                    {
                        Korisnik privremeni = new Korisnik();
                        privremeni.Id = y.korisnik1_id;
                        privremeni.kriterijumWhere = $"id = {privremeni.Id}";
                        if (broker.getCriteria(privremeni) is Korisnik k) lista.Add(k);
                    }
                }
            }
        }
    }
}
