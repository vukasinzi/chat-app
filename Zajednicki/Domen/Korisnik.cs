using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Zajednicki.Domen;

namespace Klijent.Domen
{
    public class Korisnik : IObjekat
    {
        public int Id { get; set; }
        public string Korisnicko_ime { get; set; }
        public string Lozinka { get; set; }
        [JsonIgnore]
        public string nazivTabele { get; set; } = "Korisnik";
        [JsonIgnore]
        public object koloneNaziv { get; set; } = "korisnicko_ime,lozinka";
        [JsonIgnore]
        public string vrednostiNaziv { get; set; }
        [JsonIgnore]
        public string kljucPrimarni { get; set; } = "id";
        [JsonIgnore]
        public string kljucSpoljni { get; set; } = "";
        [JsonIgnore]
        public string kriterijumWhere { get; set; } = "";
        [JsonIgnore]
        public Dictionary<string, object?> parametri { get; set; } = new Dictionary<string, object?>();
        public Korisnik(string korisnicko_ime, string lozinka)
        {
 
            Korisnicko_ime = korisnicko_ime;
            Lozinka = lozinka;
        }
        public Korisnik()
        {

        }

        public Task<List<IObjekat>> vratiObjekteAsync(SqlDataReader dr, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IObjekat>> vratiObjekteJoinAsync(SqlDataReader dr, CancellationToken token = default)
        {
            List<IObjekat> koki = new List<IObjekat>();
            while(await dr.ReadAsync(token))
            {
                Korisnik kok1 = new Korisnik();
                kok1.Korisnicko_ime = (string)dr["korisnicko_ime"];
                koki.Add(kok1);
            }
            return koki;           
        }

        public async Task<IObjekat> vratiObjekatAsync(SqlDataReader dr, CancellationToken token = default)
        {

            Korisnik k = null;
            while (await dr.ReadAsync(token))
            {
                k = new Korisnik();
                k.Id = (int)dr["Id"];
                k.Korisnicko_ime = (string)dr["Korisnicko_ime"];
                k.Lozinka = (string)dr["Lozinka"];
            }
            if (k == null)
                return null;
            return k;
        }

        public Task<IObjekat> vratiObjekatJoinAsync(SqlDataReader dr, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Korisnik other) return false;
            return Id == other.Id;
        }
        public override int GetHashCode() => Id.GetHashCode();

    }
}
