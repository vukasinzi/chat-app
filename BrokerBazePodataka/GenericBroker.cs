using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zajednicki;
using Zajednicki.Domen;

namespace BrokerBazePodataka
{
    public class GenericBroker
    {
        private static readonly string password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
            ?? throw new InvalidOperationException("MSSQL_SA_PASSWORD nije podešen.");
        private static readonly string connectionString = $"Server=localhost,1433;Database=chatapp_db;User Id=sa;Password={password};TrustServerCertificate=True;";
        private SqlConnection con;
        private SqlTransaction? tran;
        public GenericBroker()
        {
            con = new SqlConnection(connectionString);
        }
        public async Task OpenAsync(CancellationToken token = default)
        {
           if(con.State != ConnectionState.Open)
                await con.OpenAsync(token);
        }
        public async Task<SqlCommand> CreateCmdAsync(string sql, CancellationToken token = default)
        {
            await OpenAsync(token);
            if (tran == null)
                tran = (SqlTransaction)await con.BeginTransactionAsync(token);

            return new SqlCommand(sql, con, tran);
        }
        private void AddParams(SqlCommand cmd, IObjekat obj)
        {
            foreach (var p in obj.parametri)
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }

        public async Task CloseAsync()
        {
            if (tran != null)
                await RollbackAsync();
            if (con.State != ConnectionState.Closed)
                await con.CloseAsync();
            con.Dispose();
        }
        public async Task CommitAsync(CancellationToken token = default)
        {
            if (tran != null)
            {
                await tran.CommitAsync(token);
                tran.Dispose();
                tran = null;
            }
        }
        public async Task RollbackAsync(CancellationToken token = default)
        {
            if (tran != null)
            {
                await tran.RollbackAsync(token);
                tran.Dispose();
                tran = null;
            }
        }

        //broker
      
        public async Task<IObjekat> getCriteriaAsync(IObjekat obj, CancellationToken token = default)
        {

            IObjekat result;
            string sql = $"select * from {obj.nazivTabele} where {obj.kriterijumWhere}";
            using SqlCommand cmd = await CreateCmdAsync(sql, token);
            AddParams(cmd, obj);

            using SqlDataReader dr = await cmd.ExecuteReaderAsync(token);
            result = await obj.vratiObjekatAsync(dr, token);
            return result;
        }

        public async Task<int> InsertAsync(IObjekat k, CancellationToken token = default)
        {
             
            string sql = $"insert into {k.nazivTabele} values({k.vrednostiNaziv})";
            using SqlCommand cmd = await CreateCmdAsync(sql, token);
            AddParams(cmd, k);

            int affectedRows = await cmd.ExecuteNonQueryAsync(token);
            return affectedRows;
        }
        public async Task<List<IObjekat>> GetAllCriteriaAsync(IObjekat obj, CancellationToken token = default)
        {
            
            List<IObjekat> result;
            string sql = $"select {obj.koloneNaziv} from {obj.nazivTabele} where {obj.kriterijumWhere} ";
            using SqlCommand cmd = await CreateCmdAsync(sql, token);
            AddParams(cmd, obj);
   
            using SqlDataReader dr = await cmd.ExecuteReaderAsync(token);
            result = await obj.vratiObjekteAsync(dr, token);
            return result;
        }

        public async Task<int> UpdateCriteriaAsync(IObjekat obj, CancellationToken token = default)
        {
            string sql = $"update {obj.nazivTabele} set {obj.vrednostiNaziv} where {obj.kriterijumWhere}";
            using SqlCommand cmd = await CreateCmdAsync(sql, token);
            AddParams(cmd, obj);

            int affectedRows = await cmd.ExecuteNonQueryAsync(token);
            return affectedRows;
        }

        public async Task<int> DeleteCriteriaAsync(IObjekat obj, CancellationToken token = default)
        {
            string sql = $"delete from {obj.nazivTabele} where {obj.kriterijumWhere}";
            using SqlCommand cmd = await CreateCmdAsync(sql, token);
            AddParams(cmd, obj);

            int affectedRows = await cmd.ExecuteNonQueryAsync(token);
            return affectedRows;
        }
    }
}
