using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;

namespace ARCPSGUI.DB
{
    class PSDba
    {
        public string DisabledNotificationQuery = "select MACHINE_CODE, STATUS  FROM L2_PS_MASTER";

        public event EventHandler disableMachineTriggered;
        public List<Model.PSData> GetPSList()
        {
            List<Model.PSData> lstPSData = null;

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString)) // DA.Connection().getDBConnection())
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT PS_ID, PS_NAME,MACHINE_CODE,STATUS,MACHINE_CHANNEL"
                                     + " FROM L2_PS_MASTER";
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                lstPSData = new List<Model.PSData>();

                                while (reader.Read())
                                {

                                    Model.PSData objPSData = new Model.PSData();

                                    objPSData.psPkId = Int32.Parse(reader["PS_ID"].ToString());
                                    objPSData.psName = reader["PS_NAME"].ToString();
                                    objPSData.machineCode = reader["MACHINE_CODE"].ToString();

                                    objPSData.status = Int32.Parse(reader["STATUS"].ToString());
                                    objPSData.machineChannel = reader["MACHINE_CHANNEL"].ToString();


                                    lstPSData.Add(objPSData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
            return lstPSData;
        }
        public bool GetPSBlockedStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_BLOCKED FROM L2_PS_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());
                    bOk = bResult == 1;
                }
            }
            catch (Exception errMsg)
            {
                bOk = true;
            }
            finally
            {
            }

            return bOk;
        }
        public bool SetPSBlockedStatus(string machineCode, bool blockStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PS_MASTER set IS_BLOCKED ='" + (blockStatus ? 1 : 0)
                        + "' where MACHINE_CODE = '" + machineCode + "' and IS_BLOCKED= '" + (blockStatus ? 0 : 1) + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
            return bOk;
        }
        public bool GetPSEnabledStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT STATUS FROM L2_PS_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());
                    bOk = bResult == 2;
                }
            }
            catch (Exception errMsg)
            {
                bOk = true;
            }
            finally
            {
            }

            return bOk;
        }
        public bool SetPSEnabledStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PS_MASTER set STATUS ='" + (enableStatus ? 2 : 0)
                        + "' where MACHINE_CODE = '" + machineCode + "' and STATUS !='" + (enableStatus ? 2 : 0) + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
            return bOk;
        }
        public bool GetPSSwitchOffStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_SWITCH_OFF FROM L2_PS_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());
                    bOk = bResult == 1;
                }
            }
            catch (Exception errMsg)
            {
                bOk = true;
            }
            finally
            {
            }

            return bOk;
        }
        public bool SetPSSwitchOffStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PS_MASTER set IS_SWITCH_OFF ='" + (enableStatus ? 1 : 0)
                        + "' where MACHINE_CODE = '" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
            return bOk;
        }
        public bool RegisterDisabledNotification()
        {
            bool flag = false;
            try
            {
                using (OracleConnection conn = new OracleConnection(Connection.connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(this.DisabledNotificationQuery, conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        Connection.SlotDependency = new OracleDependency(cmd);
                        cmd.Notification.IsNotifiedOnce = false;
                        Connection.SlotDependency.OnChange += new OnChangeEventHandler(this.DisablesNotificatonListener);
                        cmd.AddRowid = true;
                        flag = cmd.ExecuteNonQuery() == -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }

        public NotificationData GetDisabledDataUsingNotificationQuery(string query)
        {
            NotificationData notificationData = (NotificationData)null;
            try
            {
                using (OracleConnection conn = new OracleConnection(Connection.connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    using (OracleDataReader oracleDataReader = new OracleCommand(query, conn).ExecuteReader())
                    {
                        if (oracleDataReader.HasRows)
                        {
                            if (oracleDataReader.Read())
                            {
                                notificationData = new NotificationData();
                                notificationData.category = NotificationData.errorCategory.DISABLE;
                                notificationData.MachineCode = Convert.ToString(oracleDataReader["MACHINE_CODE"]);
                                // notificationData. = int.Parse(Convert.ToString(oracleDataReader["STATUS"])) == 2;
                                notificationData.IsCleared = int.Parse(Convert.ToString(oracleDataReader["STATUS"])) == 2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return notificationData;
        }

        private void DisablesNotificatonListener(object sender, OracleNotificationEventArgs args)
        {
            string query = null;
            for (int index = 0; index < args.Details.Rows.Count; ++index)
            {

                query = this.DisabledNotificationQuery + " where rowid = '" + args.Details.Rows[index]["Rowid"].ToString() + "'";
                this.disableMachineTriggered((object)this.GetDisabledDataUsingNotificationQuery(query), new EventArgs());
            }

        }
    }
}
