using System;
using System.Collections.Generic;
using System.Text;

namespace Zajednicki
{
    public class Zahtev
    {
        public Zahtev(Operacija operacija, object objekat)
        {
            Operacija = operacija;
            Objekat = objekat;
        }

        public Operacija Operacija { get; set; }
        public object Objekat { get; set; }
    }
}
