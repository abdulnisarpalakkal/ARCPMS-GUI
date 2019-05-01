using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;
using System.Windows;

namespace ARCPSGUI.DB
{
    class CMDba
    {
        public string DisabledNotificationQuery = "select MACHINE_CODE, STATUS  FROM L2_LCM_UCM_MASTER";

        public event EventHandler disableMachineTriggered;
        public List<CMData> GetCMList()
        {
            List<CMData> lstCMData = null;

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString)) // DA.Connection().getDBConnection())
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT LU_ID, MACHINE,FLOOR,MACHINE_CODE,STATUS,CM_CHANNEL,REM_CODE"
                                     + " FROM L2_LCM_UCM_MASTER";
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                lstCMData = new List<CMData>();

                                while (reader.Read())
                                {

                                    Model.CMData objCMData = new Model.CMData();

                                    objCMData.cmPkId = Int32.Parse(reader["LU_ID"].ToString());
                                    objCMData.cmName = reader["MACHINE"].ToString();
                                    objCMData.floor = Int32.Parse(reader["FLOOR"].ToString());

                                    objCMData.cmChannel = reader["CM_CHANNEL"].ToString();
                                    objCMData.machineCode = reader["MACHINE_CODE"].ToString();

                                    objCMData.status = Convert.ToInt32(reader["STATUS"]);


                                    objCMData.remCode = Convert.ToString(reader["REM_CODE"]);



                                    lstCMData.Add(objCMData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            return lstCMData;
        }
        public bool GetCMBlockedStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_BLOCKED FROM L2_LCM_UCM_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetCMBlockedStatus(string machineCode, bool blockStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set IS_BLOCKED ='" + (blockStatus ? 1 : 0) 
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
        public bool GetCMEnabledStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT STATUS FROM L2_LCM_UCM_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetCMEnabledStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set STATUS ='" + (enableStatus ? 2 : 0)
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
        public decimal GetCMQueueId(string machineCode)
        {
            decimal queueId = 0;
            try
            {
               

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT BLOCK_Q_ID FROM L2_LCM_UCM_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool GetCMRotationStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT ROTATION_ALLOWED_STATUS FROM L2_LCM_UCM_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetCMRotationStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set ROTATION_ALLOWED_STATUS ='" + (enableStatus ? 1 : 0)
                        + "' where MACHINE_CODE = '" + machineCode + "' and ROTATION_ALLOWED_STATUS !='" + (enableStatus ? 1 : 0) + "'";
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
        //6Nov18
        public int GetCMMode(string machineCode)
        {
            int bResult = 0;
            try
            {


                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT CM_MODE FROM L2_LCM_UCM_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());

                }
            }
            catch (Exception errMsg)
            {
                bResult = 0;
            }
            finally
            {
            }

            return bResult;
        }
        public bool SetCMMode(string machineCode, int mode)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set CM_MODE =" + mode
                        + " where MACHINE_CODE = '" + machineCode + "'";
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
    }
}
