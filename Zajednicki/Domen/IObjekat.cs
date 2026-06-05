using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        Task<List<IObjekat>> vratiObjekteAsync(SqlDataReader dr, CancellationToken token = default);
        Task<List<IObjekat>> vratiObjekteJoinAsync(SqlDataReader dr, CancellationToken token = default);
        Task<IObjekat> vratiObjekatAsync(SqlDataReader dr, CancellationToken token = default);
        Task<IObjekat> vratiObjekatJoinAsync(SqlDataReader dr, CancellationToken token = default);
    }
}
