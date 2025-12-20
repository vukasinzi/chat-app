using System;
using System.Collections.Generic;
using System.Text;
using Zajednicki.Domen;

namespace SO
{
    public class UcitajSvePorukeSO : SystemOperationsBase
    {
        readonly int primalac;
        readonly int posiljalac;
        public List<Poruka> Lista;
        public UcitajSvePorukeSO(Tuple<int,int> zahtev)
        {
            primalac = zahtev.Item1;
            posiljalac = zahtev.Item2;
            Lista = new List<Poruka>();
        }
        protected override void ExecuteConcreteOperation()
        {
            Poruka p = new Poruka();
            p.kriterijumWhere = $"(primalac = {primalac} and posiljalac = {posiljalac}) OR (posiljalac = {primalac} and primalac = {posiljalac})";
            List<IObjekat> obj = broker.GetAllCriteria(p);
            Lista = obj.OfType<Poruka>().ToList();
            
        }
    }
}
