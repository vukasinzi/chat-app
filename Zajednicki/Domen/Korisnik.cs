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
            throw new NotImplementedException();
        }

        public IObjekat vratiObjekat(SqlDataReader dr)
        {
            if (!dr.Read())
                return null;
            Korisnik k = new Korisnik();
            while (dr.Read())
            {
                k.Id = (int)dr["Id"];
                k.Korisnicko_ime = (string)dr["Korisnicko_ime"];
                k.Lozinka = (string)dr["Lozinka"];
            }
            return k;
        }

        public IObjekat vratiObjekatJoin(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }
    }
}
