using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using Zajednicki;
using Zajednicki.Domen;

namespace BrokerBazePodataka
{
    public class GenericBroker
    {
        private static readonly string connectionString = LoadConnectionString();
        private SqlConnection con;
        private SqlTransaction tran;
        public GenericBroker()
        {
            con = new SqlConnection(connectionString);
        }
        public void Open()
        {
           if(con.State != ConnectionState.Open)
                con.Open();
        }
        public SqlCommand CreateCmd(string sql)
        {
            Open();
            tran ??= con.BeginTransaction();

            return new SqlCommand(sql, con, tran);
        }

        public void Close()
        {
            tran?.Rollback();
            tran?.Dispose();
            tran = null;
            if (con.State != ConnectionState.Closed)
                con.Close();
            con.Dispose();
        }
        public void Commit()
        {
            tran?.Commit();
            tran?.Dispose();
            tran = null;
        }
        public void Rollback()
        {
            tran?.Rollback();
            tran?.Dispose();
            tran = null;
        }

        private static string LoadConnectionString()
        {
            string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(appSettingsPath))
                throw new FileNotFoundException($"Nedostaje konfiguracija: {appSettingsPath}");

            using FileStream stream = File.OpenRead(appSettingsPath);
            using JsonDocument document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings) &&
                connectionStrings.TryGetProperty("ChatAppDb", out JsonElement connectionStringElement))
            {
                string? value = connectionStringElement.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            throw new InvalidOperationException("ConnectionStrings:ChatAppDb nije podešen u appsettings.json.");
        }


        //broker
      
        public IObjekat getCriteria(IObjekat obj)
        {

            IObjekat result;
            string sql = $"select * from {obj.nazivTabele} where {obj.kriterijumWhere}";
            SqlCommand cmd = CreateCmd(sql);

            SqlDataReader dr = cmd.ExecuteReader();
            result = obj.vratiObjekat(dr);
            dr.Close();
            return result;
        }

        public int Insert(IObjekat k)
        {
             
            string sql = $"insert into {k.nazivTabele} values({k.vrednostiNaziv})";
            SqlCommand cmd = CreateCmd(sql);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }
        public List<IObjekat> GetAllCriteria(IObjekat obj)
        {
            
            List<IObjekat> result;
            string sql = $"select {obj.koloneNaziv} from {obj.nazivTabele} where {obj.kriterijumWhere} ";
            SqlCommand cmd = CreateCmd(sql);
   
            SqlDataReader dr = cmd.ExecuteReader();
            result = obj.vratiObjekte(dr);
            dr.Close();
            return result;
        }

        public int UpdateCriteria(IObjekat obj)
        {
            string sql = $"update {obj.nazivTabele} set {obj.vrednostiNaziv} where {obj.kriterijumWhere}";
            SqlCommand cmd = CreateCmd(sql);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }

        public int DeleteCriteria(IObjekat obj)
        {
            string sql = $"delete from {obj.nazivTabele} where {obj.kriterijumWhere}";
            SqlCommand cmd = CreateCmd(sql);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }
    }
}
