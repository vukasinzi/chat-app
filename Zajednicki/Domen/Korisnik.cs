using System;
using System.Collections.Generic;
using System.Text;

namespace Klijent.Domen
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string Korisnicko_ime { get; set; }
        public string Lozinka { get; set; }
        public Korisnik(string korisnicko_ime, string lozinka)
        {
 
            Korisnicko_ime = korisnicko_ime;
            Lozinka = lozinka;
        }


        
    }
}
