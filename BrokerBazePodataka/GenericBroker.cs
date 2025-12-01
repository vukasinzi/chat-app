using Klijent.Domen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Zajednicki;
using Zajednicki.Domen;

namespace BrokerBazePodataka
{
    public class GenericBroker
    {
        private const string connectionString = "Data Source=DESKTOP-K69I1LH\\SQLEXPRESS;Initial Catalog=chatapp_db;Integrated Security=True;Trust Server Certificate=True";
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
            if (tran == null)
                tran = con.BeginTransaction();
            using var cmd = new SqlCommand(sql, con, tran);
            return cmd;
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
    }
}
