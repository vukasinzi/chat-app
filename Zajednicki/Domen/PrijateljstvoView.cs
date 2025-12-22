using System;
using System.Collections.Generic;
using System.Text;

namespace Zajednicki.Domen
{
    public class PrijateljstvoView 
    {
        public PrijateljstvoView(string korisnicko_ime, Prijateljstvo link)
        {
            Korisnicko_ime = korisnicko_ime;
            Link = link;
        }

        public string Korisnicko_ime { get; set; }
        public Prijateljstvo Link { get; set; }

    }
}
