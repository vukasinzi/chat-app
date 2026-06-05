using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Zajednicki.Domen
{
    public class Poruka: IObjekat
    {
        public int Id { get; set; }
        public int primalac_id { get; set; }
        public int posiljalac_id { get; set; }
        public string poruka_text { get; set; }
        [JsonIgnore]
        public string nazivTabele { get; set; } = "Poruka";
        [JsonIgnore]
        public object koloneNaziv { get; set; } = "posiljalac,primalac,poruka_text,datum";
        [JsonIgnore]
        public string vrednostiNaziv { get; set; }
        [JsonIgnore]
        public string kljucPrimarni { get; set; } = "id";
        [JsonIgnore]
        public string kljucSpoljni { get; set; }
        [JsonIgnore]
        public string kriterijumWhere { get; set; }
        [JsonIgnore]
        public Dictionary<string, object?> parametri { get; set; } = new Dictionary<string, object?>();

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

        public async Task<List<IObjekat>> vratiObjekteAsync(SqlDataReader dr, CancellationToken token = default)
        {
            List<IObjekat> popara = new List<IObjekat>();
            while (await dr.ReadAsync(token))
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

        public Task<List<IObjekat>> vratiObjekteJoinAsync(SqlDataReader dr, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<IObjekat> vratiObjekatAsync(SqlDataReader dr, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<IObjekat> vratiObjekatJoinAsync(SqlDataReader dr, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
