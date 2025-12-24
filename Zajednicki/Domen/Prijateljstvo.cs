using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zajednicki.Domen
{
    public class Prijateljstvo : IObjekat
    {
        public Prijateljstvo(int id_prijateljstvo, int korisnik1_id, int korisnik2_id, string status)
        {
            this.id_prijateljstvo = id_prijateljstvo;
            this.korisnik1_id = korisnik1_id;
            this.korisnik2_id = korisnik2_id;
            this.status = status;
        }
        public Prijateljstvo()
        {

        }

        public int id_prijateljstvo { get; set; }
        public int korisnik1_id { get; set; }
        public int korisnik2_id { get; set; }
        public string status { get; set; }
        public string nazivTabele { get; set; } = "prijateljstvo";
        public object koloneNaziv { get; set; } = "id_prijateljstva,korisnik1_id,korisnik2_id,status";
        public string vrednostiNaziv { get; set; }
        public string kljucPrimarni { get; set; } = "id_prijateljstva";
        public string kljucSpoljni { get; set; } = "korisnik1_id";
        public string kljucSpoljni2 { get; set; } = "korisnik2_id";
        public string kriterijumWhere { get; set; }

        public IObjekat vratiObjekat(SqlDataReader dr)
        {

            Prijateljstvo p = null;
            while (dr.Read())
            {

                p = new Prijateljstvo();
                p.id_prijateljstvo = (int)dr["id_prijateljstva"];
                p.korisnik1_id = (int)dr["korisnik1_id"];
                p.korisnik2_id = (int)dr["korisnik2_id"];
                p.status = (string)dr["status"];
               
            }
            if (p == null)
                return null;
            return p;
        }

        public IObjekat vratiObjekatJoin(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }

        public List<IObjekat> vratiObjekte(SqlDataReader dr)
        {
            List<IObjekat> lista = new List<IObjekat>();
            while (dr.Read())
            {

                Prijateljstvo p = new Prijateljstvo();
                p.id_prijateljstvo = (int)dr["id_prijateljstva"];
                p.korisnik1_id = (int)dr["korisnik1_id"];
                p.korisnik2_id = (int)dr["korisnik2_id"];
                p.status = (string)dr["status"];
                lista.Add(p);
            }
            if (lista.Count == 0)
                return null;
            return lista;
        }

        public List<IObjekat> vratiObjekteJoin(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Prijateljstvo other) return false;

            return (korisnik1_id == other.korisnik1_id && korisnik2_id == other.korisnik2_id)
                || (korisnik1_id == other.korisnik2_id && korisnik2_id == other.korisnik1_id);
        }

        public override int GetHashCode()
        {
            int a = Math.Min(korisnik1_id, korisnik2_id);
            int b = Math.Max(korisnik1_id, korisnik2_id);
            return HashCode.Combine(a, b);
        }
    }
}
