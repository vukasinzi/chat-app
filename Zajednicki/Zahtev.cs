using System;
using System.Collections.Generic;
using System.Text;

namespace Zajednicki
{
    public class Zahtev
    {
        public Operacija Operacija { get; set; }
        public object Objekat { get; set; }
        public Zahtev()
        {

        }
        public Zahtev(Operacija operacija, object obj)
        {
            Operacija = operacija;
            Objekat = obj;
        }

    }
}
