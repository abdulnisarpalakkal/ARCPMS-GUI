using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;
namespace ARCPSGUI.DB
{
    class VLCDba
    {
        public string DisabledNotificationQuery = "select MACHINE_CODE, STATUS  FROM L2_VLC_MASTER";

        public event EventHandler disableMachineTriggered;
        public List<Model.VLCData> GetVLCList()
        {
            List<VLCData> lstVLCData = null;

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT VLC_ID, VLC_NAME,F_ROW,F_AISLE,MACHINE_CODE"
                                     + " ,STATUS, MACHINE_CHANNEL,VLC_DECK_CODE"
                                     + " FROM L2_VLC_MASTER";



                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                lstVLCData = new List<VLCData>();

                                while (reader.Read())
                                {

                                    Model.VLCData objVLCData = new Model.VLCData();

                                    objVLCData.vlcPkId = Int32.Parse(reader["VLC_ID"].ToString());
                                    objVLCData.vlcName = reader["VLC_NAME"].ToString();
                                    objVLCData.row = Int32.Parse(reader["F_ROW"].ToString());

                                    objVLCData.aisle = Int32.Parse(reader["F_AISLE"].ToString());
                                    objVLCData.machineCode = reader["MACHINE_CODE"].ToString();

                                    objVLCData.status = Int32.Parse(reader["STATUS"].ToString());
                                    objVLCData.machineChannel = reader["MACHINE_CHANNEL"].ToString();
                                    objVLCData.vlcDeckCode = reader["VLC_DECK_CODE"].ToString();


                                    lstVLCData.Add(objVLCData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            return lstVLCData;
        }
        public bool GetVLCBlockedStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_BLOCKED FROM L2_VLC_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetVLCBlockedStatus(string machineCode, bool blockStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_VLC_MASTER set IS_BLOCKED ='" + (blockStatus ? 1 : 0)
                        + "', block_q_id=0 where MACHINE_CODE = '" + machineCode + "' and IS_BLOCKED= '" + (blockStatus ? 0 : 1) + "'";
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
        public bool GetVLCEnabledStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT STATUS FROM L2_VLC_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetVLCEnabledStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_VLC_MASTER set STATUS ='" + (enableStatus ? 2 : 0)
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
        public decimal GetVLCQueueId(string machineCode)
        {
            decimal queueId = 0;
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT BLOCK_Q_ID FROM L2_VLC_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    queueId = Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                queueId = 0;
            }
            finally
            {
            }

            return queueId;
        }


        public void saveVLCMode(string machineName, int setValue)
        {

            string query = "UPDATE l2_vlc_master SET vlc_mode =" + setValue + " where VLC_NAME='" + machineName + "' and vlc_mode!=" + setValue;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
        public int getVLCMode(string machineName)
        {
            int vlcMode = 0;
            string query = "select vlc_mode from l2_vlc_master  where VLC_NAME='" + machineName + "'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    vlcMode = int.Parse(Convert.ToString(command.ExecuteScalar()));
                }
            }
            return vlcMode;
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
