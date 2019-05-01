using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;

namespace ARCPSGUI.DB
{
    class EESDba
    {
        public string DisabledNotificationQuery = "select MACHINE_CODE, STATUS  FROM L2_EES_MASTER";

        public event EventHandler disableMachineTriggered;

        public List<Model.EESData> GetEESList()
        {
            List<EESData> lstEESData = null;

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = " SELECT EES_ID, EES_NAME,AISLE,F_ROW,MACHINE_CODE "
                                     + " , STATUS, MACHINE_CHANNEL "
                                     + " FROM L2_EES_MASTER ";
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                lstEESData = new List<EESData>();

                                while (reader.Read())
                                {

                                    EESData objEESData = new EESData();

                                    objEESData.eesPkId = Int32.Parse(reader["EES_ID"].ToString());
                                    objEESData.eesName = reader["EES_NAME"].ToString();
                                    objEESData.aisle = Int32.Parse(reader["AISLE"].ToString());
                                    objEESData.row = Int32.Parse(reader["F_ROW"].ToString());


                                    objEESData.machineCode = reader["MACHINE_CODE"].ToString();
                                    objEESData.status = Int32.Parse(reader["STATUS"].ToString());
                                    objEESData.machineChannel = reader["MACHINE_CHANNEL"].ToString();


                                    lstEESData.Add(objEESData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            return lstEESData;
        }
        public bool GetEESBlockedStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_BLOCKED FROM L2_EES_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetEESBlockedStatus(string machineCode, bool blockStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_EES_MASTER set IS_BLOCKED ='" + (blockStatus ? 1 : 0)
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
        public bool GetEESEnabledStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT STATUS FROM L2_EES_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public bool SetEESEnabledStatus(string machineCode, bool enableStatus)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_EES_MASTER set STATUS ='" + (enableStatus ? 2 : 0)
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
        public decimal GetEESQueueId(string machineCode)
        {
            decimal queueId = 0;
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT BLOCK_Q_ID FROM L2_EES_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
        public void getEESParameters(int eesNumber, out string eesChannel, out string eesCode)
        {
            eesChannel = null;
            eesCode = null;

            switch (eesNumber)
            {

                case 1:
                    eesChannel = "CH431";
                    eesCode = "EES_FLR4_01";
                    break;
                case 2:
                    eesChannel = "CH432";
                    eesCode = "EES_FLR4_02";
                    break;
                case 3:
                    eesChannel = "CH433";
                    eesCode = "EES_FLR4_03";
                    break;
                case 4:
                    eesChannel = "CH434";
                    eesCode = "EES_FLR4_04";
                    break;
                case 5:
                    eesChannel = "CH435";
                    eesCode = "EES_FLR4_05";
                    break;
                case 6:
                    eesChannel = "CH436";
                    eesCode = "EES_FLR4_06";
                    break;
                case 7:
                    eesChannel = "CH437";
                    eesCode = "EES_FLR4_07";
                    break;
                case 8:
                    eesChannel = "CH438";
                    eesCode = "EES_FLR4_08";
                    break;
                case 9:
                    eesChannel = "CH439";
                    eesCode = "EES_FLR4_09";
                    break;
                default:
                    eesChannel = "";
                    eesCode = "";
                    break;
            }








        }


        public void SaveEveningModeEntryEES(string entryEESs)
        {
            string sql = null;
            if (entryEESs != null)
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {

                    sql = "";
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "update L2_EES_MASTER set EVENING_MODE = 1 where EES_ID in " + entryEESs + " and EVENING_MODE != 1";
                    if (!string.IsNullOrEmpty(sql))
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }


            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (entryEESs != null)
                    sql = "update l2_ees_master set evening_mode=-1  where evening_mode!=-1 and EES_ID not in " + entryEESs;
                else
                    sql = "update l2_ees_master set evening_mode=-1  where evening_mode!=-1 ";
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();

                if (!string.IsNullOrEmpty(sql))
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
        public void SaveMorningModeExitEES(string exitEESs)
        {
            string sql = null;

            if (exitEESs != null)
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {

                    sql = "";
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "update L2_EES_MASTER set MORNING_MODE = -1 where EES_ID in " + exitEESs + " and MORNING_MODE != -1";
                    if (!string.IsNullOrEmpty(sql))
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }


            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (exitEESs != null)
                    sql = "update l2_ees_master set MORNING_MODE=1  where MORNING_MODE!=1 and EES_ID not in " + exitEESs;
                else
                    sql = "update l2_ees_master set MORNING_MODE=1  where MORNING_MODE!=1 ";
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();

                if (!string.IsNullOrEmpty(sql))
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }



        }
        public DataTable RetrieveNormalEES()
        {
            string query = "SELECT EES_ID,EES_NAME, NORMAL_MODE,NORMAL_MIX_EES FROM L2_EES_MASTER";

            DataTable dt = new DataTable();
            dt.TableName = "EESMASTER";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    OracleDataAdapter dadapter = new OracleDataAdapter(command);
                    dadapter.Fill(dt);
                }
            }
            return dt;
        }

        public void SaveNormalModeEES(string eesName, int eesMode)
        {



            string sql = "";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {



                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();

                if (eesMode != 0)
                {
                    sql = "update L2_EES_MASTER set NORMAL_MODE = " + eesMode + ",NORMAL_MIX_EES = 0"
                    + " where EES_NAME = '" + eesName + "'" + " and (NORMAL_MODE!=" + eesMode + " or NORMAL_MIX_EES != 0)";

                }
                else if (eesMode == 0)
                {
                    sql = "update L2_EES_MASTER set NORMAL_MIX_EES = " + 1
                    + " where EES_NAME = '" + eesName + "'" + " and NORMAL_MIX_EES != 1";

                }
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

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
        public int GetGateEntryStatus(string machineCode)
        {
            int bResult = 0;
            try
            {
               

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT ENTRY_STATUS FROM L2_EES_MASTER WHERE MACHINE_CODE ='" + machineCode + "'";
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
    }
}
