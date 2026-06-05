using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Zajednicki.Domen
{
    public interface IObjekat
    {
        
        string nazivTabele { get; set; }
        object koloneNaziv { get; set; }
        string vrednostiNaziv { get; set; }
        string kljucPrimarni { get; set; }
        string kljucSpoljni { get; set; }
        string kriterijumWhere { get; set; }
        Dictionary<string, object?> parametri { get; set; }

        List<IObjekat> vratiObjekte(SqlDataReader dr);
        List<IObjekat> vratiObjekteJoin(SqlDataReader dr);
        IObjekat vratiObjekat(SqlDataReader dr);
        IObjekat vratiObjekatJoin(SqlDataReader dr);
    }
}
