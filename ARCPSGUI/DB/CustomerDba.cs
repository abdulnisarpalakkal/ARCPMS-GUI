using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ARCPSGUI.DB
{
    class CustomerDba
    {
        public void SaveNote(string note, int customerid)
        {
           
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        OracleCommand command = con.CreateCommand();

                        string sql = "update l2_customers set Note = '" + note + "' where customer_id = " + customerid;
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    
               
            
        }
        public void SaveNoteToParkHistory(string note, int parkId)
        {

            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();

                string sql = "update L2_PARK_HISTORY set Note = '" + note + "' where PARK_ID = " + parkId;
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }



        }

        public string GetNote(int customerid)
        {
            string note = null;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();
                string sql = "select Note from l2_customers where customer_id = " + customerid;
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                note = Convert.ToString(command.ExecuteScalar());
            }
            return note;

        }
        public string GetNoteFromParkHistory(int parkId)
        {
            string note = null;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();
                string sql = "select Note from L2_PARK_HISTORY where PARK_ID = " + parkId;
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                note = Convert.ToString(command.ExecuteScalar());
            }
            return note;

        }

        public void GetPhotoPath(int customerPrimaryKeyID, out string northImgPath, out string southImgPath)
        {
            northImgPath = "";
            southImgPath = "";
            string query = "SELECT * FROM L2_CUSTOMERS WHERE CUSTOMER_ID =" + customerPrimaryKeyID;
           
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            northImgPath = Convert.ToString(dreader["ENTRY_NORTH_IMG"]);
                            southImgPath = Convert.ToString(dreader["ENTRY_SOUTH_IMG"]);
                        }
                    }
                }
            }
        }

        public void RevertRetrievedStatus(int customerId, string startTimeString)
        {
            string sql = null;
            sql = " UPDATE L2_CUSTOMERS SET is_retrieved = 0 ,entry_time = '" + startTimeString + "'"
               + " where CUSTOMER_ID=" + customerId;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == System.Data.ConnectionState.Closed) con.Open();

                // Create the Select command retrieving all data from the Dept table. 
                OracleCommand selectCommand = new OracleCommand(sql, con);
                selectCommand.CommandType = CommandType.Text;
                selectCommand.ExecuteNonQuery();
            }
        }
    }
}
