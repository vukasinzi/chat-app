using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace Klijent.Domen
{
    public class Korisnik : IObjekat
    {
        public int Id { get; set; }
        public string Korisnicko_ime { get; set; }
        public string Lozinka { get; set; }
        public string nazivTabele { get; set; } = "Korisnik";
        public object koloneNaziv { get; set; } = "korisnicko_ime,lozinka";
        public string vrednostiNaziv { get; set; }
        public string kljucPrimarni { get; set; } = "id";
        public string kljucSpoljni { get; set; } = "";
        public string kriterijumWhere { get; set; } = "";
        public Korisnik(string korisnicko_ime, string lozinka)
        {
 
            Korisnicko_ime = korisnicko_ime;
            Lozinka = lozinka;
        }
        public Korisnik()
        {

        }

        public List<IObjekat> vratiObjekte(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }

        public List<IObjekat> vratiObjekteJoin(SqlDataReader dr)
        {
            List<IObjekat> koki = new List<IObjekat>();
            while(dr.Read())
            {
                Korisnik kok1 = new Korisnik();
                kok1.Korisnicko_ime = (string)dr["korisnicko_ime"];
                koki.Add(kok1);
            }
            if (koki.Count == 0)
                return null;
            return koki;           
        }

        public IObjekat vratiObjekat(SqlDataReader dr)
        {

            Korisnik k = null;
            while (dr.Read())
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

        public IObjekat vratiObjekatJoin(SqlDataReader dr)
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
