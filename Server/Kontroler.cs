using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki;

namespace Server
{
    public class Kontroler
    {
        public static Kontroler instance;
        public static Kontroler Instance
        {
            get
            {
                if (instance == null)
                    instance = new Kontroler();
                return instance;
            }
        }

        internal async Task<Odgovor> LogIn(Korisnik korisnik)
        {
           
        }
    }
}
