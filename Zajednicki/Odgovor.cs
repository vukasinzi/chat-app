using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Zajednicki
{
    public class Odgovor
    {
        public object? Rezultat { get; set; }
        public string Poruka { get; set; } = "";
        public bool Uspesno { get; set; }

      
    }
}
