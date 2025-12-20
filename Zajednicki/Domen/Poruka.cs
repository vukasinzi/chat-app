using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zajednicki.Domen
{
    public class Poruka: IObjekat
    {
        public int Id { get; set; }
        public int primalac_id { get; set; }
        public int posiljalac_id { get; set; }
        public string poruka_text { get; set; }
        public string nazivTabele { get; set; } = "Poruka";
        public object koloneNaziv { get; set; } = "posiljalac,primalac,poruka_text,datum";
        public string vrednostiNaziv { get; set; }
        public string kljucPrimarni { get; set; } = "id";
        public string kljucSpoljni { get; set; }
        public string kriterijumWhere { get; set; }

        public DateTime datum;
        public Poruka()
        {

        }
        public Poruka(int primalac_id, int posiljalac_id, string poruka_text)
        {
            this.primalac_id = primalac_id;
            this.posiljalac_id = posiljalac_id;
            this.poruka_text = poruka_text;
        }

        public List<IObjekat> vratiObjekte(SqlDataReader dr)
        {
            List<IObjekat> popara = new List<IObjekat>();
            while (dr.Read())
            {
                Poruka pop1 = new Poruka();
                pop1.primalac_id = (int)dr["primalac"];
                pop1.posiljalac_id = (int)dr["posiljalac"];
                pop1.poruka_text = (string)dr["poruka_text"];
                pop1.datum = (DateTime)dr["datum"];
                popara.Add(pop1);
            }
            if (popara.Count == 0)
                return null;
            return popara;
            
        }

        public List<IObjekat> vratiObjekteJoin(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }

        public IObjekat vratiObjekat(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }

        public IObjekat vratiObjekatJoin(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }
    }
}
