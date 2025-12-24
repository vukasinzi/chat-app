using Klijent.Domen;
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not PrijateljstvoView other) return false;
            if (Link == null || other.Link == null) return false;
            return Link.Equals(other.Link);
        }
        public override int GetHashCode()
        {
            return Link == null ? 0 : Link.GetHashCode();
        }
    }
}
