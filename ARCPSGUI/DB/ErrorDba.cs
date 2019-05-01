using ARCPSGUI.Model;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ARCPSGUI.DB
{
    class ErrorDba
    {
        public string TriggerNotificationQuery = "select TRIGGER_ID, MACHINE, IS_TRIGGER,TRIGGER_TYPE,N_VALUE  FROM L2_TRIGGER_COMMANDS   ";

        public event EventHandler triggerMachineTriggered;
        public string RetrieveTriggerDetails(string machineName)
        {
            string machineCommand = "";
            string query = " select COMMAND from L2_TRIGGER_COMMANDS where  done = 0 and MACHINE = '" + machineName + "'";

            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    machineCommand = Convert.ToString(command.ExecuteScalar());
                    //queueId = Int64.TryParse(,);
                }
            }
            return machineCommand;
        }
        public int RetrievePathStage(string machineName)
        {
            int pathStage = 0;
            string query = " select path_stage from l2_slot_path where queue_id=(select tr.q_id from l2_trigger_commands tr where tr.machine= '" + machineName + "')";

            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    pathStage = int.Parse(Convert.ToString(command.ExecuteScalar()));
                    //queueId = Int64.TryParse(,);
                }
            }
            return pathStage;
        }

        public void UpdateLockStatus(string machineName)
        {
            string query = " UPDATE L2_TRIGGER_COMMANDS SET DONE = 1 WHERE MACHINE = '" + machineName + "'";

            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    command.ExecuteNonQuery();
                }
            }

        }
        public bool RegisterTriggerNotification()
        {
            bool flag = false;
            try
            {
                using (OracleConnection conn = new OracleConnection(Connection.connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(this.TriggerNotificationQuery, conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        Connection.ConfigMasterDependency = new OracleDependency(cmd);
                        cmd.Notification.IsNotifiedOnce = false;
                        Connection.ConfigMasterDependency.OnChange += new OnChangeEventHandler(this.TriggerNotificatonListener);
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

        public NotificationData GetTriggerDataUsingNotificationQuery(string query)
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
                                NotificationData.errorCategory category = NotificationData.errorCategory.TRIGGER;
                                Enum.TryParse(Convert.ToString(oracleDataReader["TRIGGER_TYPE"]), out category);
                                notificationData.category = category;
                                if (notificationData.category == NotificationData.errorCategory.NA)
                                    notificationData.category = NotificationData.errorCategory.TRIGGER;
                                if (notificationData.category==NotificationData.errorCategory.ERROR)
                                {
                                    notificationData.ErrorCode = Convert.ToString(oracleDataReader["N_VALUE"]);
                                }
                                notificationData.MachineCode = Convert.ToString(oracleDataReader["MACHINE"]);
                                //notificationData.TriggerStatus = int.Parse(Convert.ToString(oracleDataReader["IS_TRIGGER"])) == 1;
                                notificationData.IsCleared = !(int.Parse(Convert.ToString(oracleDataReader["IS_TRIGGER"])) == 1);
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

        private void TriggerNotificatonListener(object sender, OracleNotificationEventArgs args)
        {


            string query = null;
            for (int index = 0; index < args.Details.Rows.Count; ++index)
            {

                query = this.TriggerNotificationQuery + " where rowid = '" + args.Details.Rows[index]["Rowid"].ToString() + "'";
                this.triggerMachineTriggered((object)this.GetTriggerDataUsingNotificationQuery(query), new EventArgs());
            }

        }
    }
}
