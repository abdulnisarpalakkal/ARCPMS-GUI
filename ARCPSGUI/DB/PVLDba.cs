using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;

namespace ARCPSGUI.DB
{
    class PVLDba
    {
        public string DisabledNotificationQuery = "select MACHINE_CODE, STATUS  FROM L2_PVl_MASTER";

        public event EventHandler disableMachineTriggered;

        public List<Model.PVLData> GetPVLList()
        {
            List<Model.PVLData> lstPVLData = null;

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString)) // DA.Connection().getDBConnection())
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT PVL_ID, PVL_NAME,MACHINE_CODE,F_AISLE,F_ROW,STATUS, MACHINE_CHANNEL"
                                     + " FROM L2_PVl_MASTER";

                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                lstPVLData = new List<Model.PVLData>();

                                while (reader.Read())
                                {

                                    Model.PVLData objPVLData = new Model.PVLData();

                                    objPVLData.pvlPkId = Int32.Parse(reader["PVL_ID"].ToString());
                                    objPVLData.pvlName = reader["PVL_NAME"].ToString();
                                    objPVLData.machineCode = reader["MACHINE_CODE"].ToString();
                                    objPVLData.aisle = Int32.Parse(reader["F_AISLE"].ToString());

                                    objPVLData.row = Int32.Parse(reader["F_ROW"].ToString());
                                    objPVLData.status = Int32.Parse(reader["STATUS"].ToString());
                                    objPVLData.machineChannel = reader["MACHINE_CHANNEL"].ToString();

                                    lstPVLData.Add(objPVLData);
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
            return lstPVLData;
        }
        
        public bool GetPVLBlockedStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_BLOCKED FROM L2_PVL_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetPVLBlockedStatus(string machineCode, bool blockStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PVL_MASTER set IS_BLOCKED ='" + (blockStatus ? 1 : 0)
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
        public bool GetPVLEnabledStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT STATUS FROM L2_PVL_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetPVLEnabledStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PVL_MASTER set STATUS ='" + (enableStatus ? 2 : 0)
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
        /// <summary>
        /// GET PVL MIN ZONE RANGE FOR SLOT SELECTION
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public int GetPVLMinSlotRange(string machineCode)
        {
            int minRange = 0;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT CONFIG_START_AISLE FROM L2_PVL_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    minRange = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
            }

            return minRange;
        }
        /// <summary>
        /// SET PVL MIN ZONE RANGE FOR SLOT SELECTION
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public void SetPVLMinSlotRange(string machineCode, int minRange)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PVL_MASTER set CONFIG_START_AISLE =" + minRange
                        + " where MACHINE_CODE = '" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
        }
        /// <summary>
        /// GET PVL MAX ZONE RANGE FOR SLOT SELECTION
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public int GetPVLMaxSlotRange(string machineCode)
        {
            int maxRange = 0;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT CONFIG_END_AISLE FROM L2_PVL_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    maxRange = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
            }

            return maxRange;
        }
        /// <summary>
        /// SET PVL MAX ZONE RANGE FOR SLOT SELECTION
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public void SetPVLMaxSlotRange(string machineCode, int maxRange)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PVL_MASTER set CONFIG_END_AISLE =" + maxRange
                        + " where MACHINE_CODE = '" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
        }
        /// <summary>
        /// RESET CONFIGURATION
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public void ResetPVLZone()
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_PVL_MASTER set CONFIG_START_AISLE=START_AISLE, CONFIG_END_AISLE =END_AISLE";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bOk = command.ExecuteNonQuery() > 0;
                }

            }
            finally
            {

            }
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
