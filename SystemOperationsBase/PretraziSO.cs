using Klijent.Domen;
using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki;
using Zajednicki.Domen;

namespace SO
{
    public class PretraziSO : SystemOperationsBase
    {
        private string msgText;
        int primalac;
        public string prinm;
        Korisnik k = new Korisnik();
        public Odgovor o = new Odgovor();
        public PretraziSO(string msgText)
        {
            this.msgText = msgText;
            k.Korisnicko_ime = msgText;
            k.koloneNaziv = "id,korisnicko_ime";
            k.kriterijumWhere = $"korisnicko_ime = '{k.Korisnicko_ime}'";
        }
        public PretraziSO(int primalac)
        {
            this.primalac = primalac;
            k.koloneNaziv = "korisnicko_ime";
            k.kriterijumWhere = $"id = '{primalac}'";
        }
        protected override void ExecuteConcreteOperation()
        {
            o.Rezultat = (Korisnik)broker.getCriteria(k);
            if (o.Rezultat != null)
            {
                prinm = ((Korisnik)o.Rezultat).Korisnicko_ime;
                o.Uspesno = true;
            }
        }
    }
}
