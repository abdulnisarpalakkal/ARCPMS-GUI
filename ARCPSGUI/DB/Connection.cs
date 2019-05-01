using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.OPC;
using System.Configuration;
using ARCPSGUI.Model;
namespace ARCPSGUI.DB
{
    public class Connection
    {
        #region Member Variables

        static OracleConnection GlobalConn;


        public static string connectionString = null;
        public static OracleDependency SlotDependency { get; set; }
        public static OracleDependency CarWashDependency { get; set; }
        public static OracleDependency ConfigMasterDependency { get; set; }
 //       public static string connectionString = "Data Source=(DESCRIPTION="
 //+ "(ADDRESS=(PROTOCOL=TCP)(HOST=3.0.0.200)(PORT=1521))"
 //+ "(CONNECT_DATA=(SID=orcl)));"
 //+ "User Id=l2rpmsadmin;Password=fclrpms456;";

//        public static string connectionString = "Data Source=(DESCRIPTION="
//+ "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
//+ "(CONNECT_DATA=(SID=orcl)));"
//+ "User Id=l2rpmsadmin;Password=fclrpms456;";

//        public static string connectionString = "Data Source=(DESCRIPTION="
//+ "(ADDRESS=(PROTOCOL=TCP)(HOST=VisionServer)(PORT=1521))"
//+ "(CONNECT_DATA=(SID=orcl)));"
//+ "User Id=l2rpmsadmin;Password=fclrpms456;";
        #endregion

        public Connection()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }

          
       
        public DataTable GetSnapShot(int leveId)
        {
            string query = "SELECT * FROM L2_PROC_SNAPSHOT where F_LEVEL_ID = " + leveId + "  order by F_ROW,AISLE";

            DataTable dt = new DataTable();
            dt.TableName = "SNAPSHOT";
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

        public DataTable GetExecCommandsInFloor(int floor)
        {

            string query = "SELECT * FROM EXECUTING_COMMANDS_IN_FLOOR where FLOOR = " + floor + " order by TRANS_ID,EES";

            DataTable dt = new DataTable();
            dt.TableName = "VIEW";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public DataTable GetAllMachineTags()
        {
            string query = " select distinct channel, machine from l2_opc_tag_master where machine like 'UCM%' "
                             + " or machine like 'LCM%' order by machine";

            //    string query = " select distinct channel, machine from l2_opc_tag_master where (machine like 'UCM%' "
            //+" or machine like 'LCM%' or machine LIKE 'VLC_Drive_%' or machine like 'EES%' "
            //+ " ) order by machine ";

            DataTable dt = new DataTable();
            dt.TableName = "SNAPSHOT";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public DataTable GetAllMachineTagsForSynch()
        {
            string query = " select distinct channel, machine from l2_opc_tag_master where (machine like 'UCM%' "
        + " or machine like 'LCM%' or machine LIKE 'VLC_Drive_%' or machine like 'EES%' "
        + " ) order by machine ";

            DataTable dt = new DataTable();
            dt.TableName = "SNAPSHOT";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public DataTable GetMachineAlertTags()
        {
            string query = "SELECT * FROM MACHINE_ALERT order by machine";

            DataTable dt = new DataTable();
            dt.TableName = "MACHINEALERT";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public void UpdateMachineValues(string machine, int minWin, int maxWin, int currentPosition, int currentRow) //changed home aisle = current position.
        {

            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();
                //VIRTUAL_AISLE_MIN, VIRTUAL_AISLE_MAX,  CHANGED_HOME_AISLE
                string sql = "update l2_lcm_ucm_master set VIRTUAL_AISLE_MIN =" + minWin
                  + ", VIRTUAL_AISLE_MAX = " + maxWin + ", CHANGED_HOME_AISLE = " + currentPosition
                  + ", CHANGED_HOME_ROW =" + currentRow
                  + " where MACHINE_CODE = '" + machine + "'";

                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

        }

        public DataTable GetErrorDetails()
        {

            DataTable dt = new DataTable();

            try
            {
                TimeSpan currentTime = System.DateTime.Now.TimeOfDay;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select * FROM L2_TRIGGER_COMMANDS where  is_trigger =1";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    OracleDataAdapter da = new OracleDataAdapter(selectCommand);
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dt;

        }

        public DataTable GetErrorDetails(string machineCode)
        {

            DataTable dt = new DataTable();

            try
            {
                TimeSpan currentTime = System.DateTime.Now.TimeOfDay;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    //  string sql = "select * FROM L2_TRIGGER_COMMANDS where DONE =0 and is_trigger =1 and MACHINE ='" + machineCode + "'";
                    string sql = "select * FROM L2_TRIGGER_COMMANDS where  is_trigger =1 and MACHINE ='" + machineCode + "'";
                    // Create the Select command retrieving all data from the Dept table. 
                    OracleCommand selectCommand = new OracleCommand(sql, con);

                    OracleDataAdapter da = new OracleDataAdapter(selectCommand);
                    da. Fill(dt);


                }



                //using (OracleCommand command = con.CreateCommand())
                //{
                //    string sql = "select COMMAND,MACHINE FROM L2_TRIGGER_COMMANDS from l2_mode_master where DONE = 1";
                //    command.CommandText = sql;
                //    string a = Convert.ToString(command.ExecuteScalar());
                //    dep = new OracleDependency(command);
                //}
                // Set the port number for the listener to listen for the notification
                // request
                //   OracleDependency.Port = 1005;

                // Create an OracleDependency instance and bind it to an OracleCommand
                // instance.
                // When an OracleDependency instance is bound to an OracleCommand
                // instance, an OracleNotificationRequest is created and is set in the
                // OracleCommand's Notification property. This indicates subsequent 
                // execution of command will register the notification.
                // By default, the notification request is using the Database Change
                // Notification.


                // Add the event handler to handle the notification. The 
                // OnMyNotification method will be invoked when a notification message
                // is received from the database
                //   dep.OnChange += new OnChangeEventHandler(dep_OnChange);
                //new OnChangeEventHandler(MyNotificationSample.OnMyNotificaton);

                // The notification registration is created and the query result sets 
                // associated with the command can be invalidated when there is a 
                // change.  When the first notification registration occurs, the 
                // notification listener is started and the listener port number 
                // will be 1005.
                //  cmd.ExecuteNonQuery();

                // Updating emp table so that a notification can be received when
                // the emp table is updated.
                // Start a transaction to update emp table
                //OracleTransaction txn = con.BeginTransaction();
                //// Create a new command which will update emp table
                //string updateCmdText =
                //  "update emp set sal = sal + 10 where empno = 7782";
                //OracleCommand updateCmd = new OracleCommand(updateCmdText, con);
                //// Update the emp table
                //updateCmd.ExecuteNonQuery();
                ////When the transaction is committed, a notification will be sent from
                ////the database
                //txn.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dt;

        }
        public bool IsTriggerEnabledUsingQueueId(int queue_id)
        {
            int triggerValue = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select is_trigger FROM L2_TRIGGER_COMMANDS where  q_id ='" + queue_id + "'";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    int.TryParse(selectCommand.ExecuteScalar().ToString(), out triggerValue);

                }
            }
            catch (Exception errmsg)
            {
                Console.WriteLine(errmsg.Message);
            }
            return triggerValue == 1;
            
        }
        public string GetTriggerEnabledMachineUsingQueueId(int queue_id)
        {
            string machine = null;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select machine FROM L2_TRIGGER_COMMANDS where  is_trigger=1 and q_id ='" + queue_id + "'";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    machine=selectCommand.ExecuteScalar().ToString();

                }
            }
            catch (Exception errmsg)
            {
                Console.WriteLine(errmsg.Message);
            }
            return machine;

        }

        public void GetErrorCommand(string rowid,out string errorCommand,out string channel, out string machineName,out bool isTrigger,out bool isLock)
        {

            errorCommand = "";
            channel = "";
            machineName = "";
            isTrigger = false;
            isLock = false;
            int tmpTrigger = 0;
            int tmpLock = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select DISTINCT TC.COMMAND,TM.channel, TM.MACHINE,IS_TRIGGER,DONE FROM L2_TRIGGER_COMMANDS TC INNER JOIN " +
                                 " l2_opc_tag_master TM on TM.machine = TC.Machine where   TC.rowid ='" + rowid + "'"; //(TC.is_trigger =1 or TC.done = 0) and
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using(OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            while (dreader.Read())
                            {
                                errorCommand = Convert.ToString(dreader["COMMAND"]);
                                channel = Convert.ToString(dreader["channel"]);
                                machineName = Convert.ToString(dreader["MACHINE"]);
                                int.TryParse(Convert.ToString(dreader["IS_TRIGGER"]), out tmpTrigger);
                                int.TryParse(Convert.ToString(dreader["DONE"]), out tmpLock);
                                isTrigger = (tmpTrigger == 1);
                                isLock = (tmpLock == 0);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }

        public bool NeedToTrigger(string machineCode)
        {
            int needtotrigger = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    //  string sql = "select * FROM L2_TRIGGER_COMMANDS where DONE =0 and is_trigger =1 and MACHINE ='" + machineCode + "'";
                    string sql = "select count(*) FROM L2_TRIGGER_COMMANDS where DONE = 0 and is_trigger =1 and MACHINE ='" + machineCode + "'";
                    // Create the Select command retrieving all data from the Dept table. 
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    int.TryParse(selectCommand.ExecuteScalar().ToString(), out needtotrigger);

                }
            }
            catch (Exception errmsg)
            {

            }
            if (needtotrigger < 1)
                return false;
            else
                return true;
        }

        public void UpdateErrorDetails(string machineCode,int triggerAction)
        {
            try
            {




                TimeSpan currentTime = System.DateTime.Now.TimeOfDay;
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "update L2_TRIGGER_COMMANDS set Is_Trigger = 0,TRIGGER_ACTION=" + triggerAction 
                        + " where MACHINE ='" + machineCode + "'" +
                          " and Is_Trigger = 1 ";
                    // Create the Select command retrieving all data from the Dept table. 
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    selectCommand.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void UpdateOptimizePathStatus(string machineCode, int optimisePathvalue,int optimiseSlotValue)
        {
            try
            {




                TimeSpan currentTime = System.DateTime.Now.TimeOfDay;
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "update l2_ees_queue set NEED_OPTIMIZE_PATH=" + optimisePathvalue + ", NEED_OPTIMIZE_SLOT=" + optimiseSlotValue
                        + "  where id=(select tr.q_id from l2_trigger_commands tr where tr.machine='" + machineCode + "')"
                    + " and (NEED_OPTIMIZE_PATH!=" + optimisePathvalue + " or NEED_OPTIMIZE_SLOT!=" + optimiseSlotValue + ")";
                    // Create the Select command retrieving all data from the Dept table. 
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    selectCommand.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        void dependency_OnChange(object sender, OracleNotificationEventArgs args)
        {
            //DataTable dt = args.Details;

            //Console.WriteLine("The following database objects were changed:");
            //foreach (string resource in args.ResourceNames)
            //    Console.WriteLine(resource);

            //Console.WriteLine("\n Details:");
            //Console.Write(new string('*', 80));
            //for (int rows = 0; rows < dt.Rows.Count; rows++)
            //{
            //    Console.WriteLine("Resource name: " + dt.Rows[rows].ItemArray[0]);
            //    string type = Enum.GetName(typeof(OracleNotificationInfo), dt.Rows[rows].ItemArray[1]);
            //    Console.WriteLine("Change type: " + type);
            //    Console.Write(new string('*', 80));
            //} 
        }

        static void dep_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            DataTable changeDetails = eventArgs.Details;
            for (int rows = 0; rows < changeDetails.Rows.Count; rows++)
            {
                Console.WriteLine("Resource name: " + changeDetails.Rows[rows].ItemArray[0]);
                string type = Enum.GetName(typeof(OracleNotificationInfo), changeDetails.Rows[rows].ItemArray[1]);
                Console.WriteLine("Change type: " + type);
                Console.Write(new string('*', 80));
            }
            //modeValue = Convert.ToInt32(changeDetails.Rows[0]["ResourceName"]);
        }

        public DataTable GetSlotSummary(int floorId)
        {
            string query = "select 'Low Car' as CarType, count(slot_type) SlotCount from L2_PROC_SNAPSHOT where slot_status in (0,1,2) and slot_type =  1  and f_level_id =" + floorId;
            query += " union select 'High Car' as CarType, count(slot_status) SlotCount from L2_PROC_SNAPSHOT where slot_status in (0,1,2) and slot_type =  2  and f_level_id =" + floorId;
            //string query = "select 'Car Occupied' as CarType, count(slot_type) SlotCount from L2_e_SNAPSHOT where slot_status in (0,1,2) and slot_type =  1  and f_level_id =" + floorId;
            //query += " union select 'Low Car' as CarType, count(slot_status) SlotCount from L2_PROC_SNAPSHOT where slot_status = 1 and f_level_id =" + floorId;
            //query += " union select 'Pallet Bundle' as CarType, count(slot_status) SlotCount from L2_PROC_SNAPSHOT where  slot_status in (0,1,2) and slot_type =  2  and f_level_id =" + floorId;


            //string query = " select distinct channel, machine from l2_opc_tag_master where machine = 'UCM_FLR7_03'";

            DataTable dt = new DataTable();
            dt.TableName = "SLOTSUMMARY";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public DataTable GetSlotCountByLevel()
        {
            string query = "select * from rpmsadmin.vw_FloorScoreBoard where SLOTSTATUS in(2,3,4,5,6,7)"; // order by LEVELNO"; //vw_SlotCountByLevel";

            DataTable dt = new DataTable();
            dt.TableName = "SLOTSUMMARY";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
            }
            finally
            {

            }
            return dt;

        }

        public void GetTotalValidCapacity(out Int32 carOccupied, out Int32 totalValidCapacity)
        {
            carOccupied = 0;
            totalValidCapacity = 0;
            //string query = " select count(*) from l2_proc_snapshot where slot_status in(0,2)";
            string query = " select * from rpmsadmin.v_DisplayBoardMessage";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read()) // (dreader.HasRows)
                            {
                                if (Convert.ToInt32(dreader["v_Type"]) == 1) //'Car Occupied'
                                {
                                    carOccupied = Convert.ToInt32(dreader["v_count"]);
                                }
                                else if (Convert.ToInt32(dreader["v_Type"]) == 2) //'Total Valid Capacity'
                                {
                                    totalValidCapacity = Convert.ToInt32(dreader["v_count"]);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {

            }
        }

        public void GetMachineShortName(string machineCode, out string machineShortName, out string floor)
        {
            machineShortName = "";
            floor = "";
            string sql = "";
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();

                    if (machineCode.Contains("REM") == false)
                    {
                        if (machineCode.Contains("LCM") || machineCode.Contains("UCM"))
                            sql = "select LU_NAME,FLOOR FROM L2_LCM_UCM_MASTER where MACHINE_CODE ='" + machineCode + "'";
                        else if (machineCode.Contains("EES"))
                            sql = "select EES_NAME,0 FLOOR FROM L2_EES_MASTER where MACHINE_CODE ='" + machineCode + "'";
                        else if (machineCode.Contains("VLC"))
                            sql = "select VLC_NAME,0 FLOOR FROM L2_VLC_MASTER where MACHINE_CODE ='" + machineCode + "'";
                        else if (machineCode.Contains("PST_"))
                            sql = "select PST_NAME,0 FLOOR FROM L2_PST_MASTER where MACHINE_CODE ='" + machineCode + "'";
                        else if (machineCode.Contains("PS_"))
                            sql = "select PS_NAME,0 FLOOR FROM L2_PS_MASTER where MACHINE_CODE ='" + machineCode + "'";
                        else if (machineCode.Contains("PVL"))
                            sql = "select PVL_NAME,0 FLOOR FROM L2_PVL_MASTER where MACHINE_CODE ='" + machineCode + "'";

                        OracleCommand selectCommand = new OracleCommand(sql, con);
                        selectCommand.CommandType = CommandType.Text;
                        using (OracleDataReader dreader = selectCommand.ExecuteReader())
                        {
                            while (dreader.Read())
                            {
                                machineShortName = dreader[0].ToString();
                                floor = dreader[1].ToString();
                            }
                        }
                    }
                    else if (machineCode.Contains("REM"))
                    {
                        string tmp = GetMachineRelatedToREM(machineCode);
                        sql = "select LU_NAME,FLOOR FROM L2_LCM_UCM_MASTER where MACHINE_CODE ='" + tmp + "'";
                        OracleCommand selectCommand = new OracleCommand(sql, con);
                        selectCommand.CommandType = CommandType.Text;
                        using (OracleDataReader dreader = selectCommand.ExecuteReader())
                        {
                            while (dreader.Read())
                            {
                                machineShortName = dreader[0].ToString();
                                floor = dreader[1].ToString();
                            }
                        }
                        //machineShortName = machineShortName + "_" + "REM";
                    }
                }
            }
            catch (Exception errmsg)
            {

            }
        }

        public string GetMachineRelatedToREM(string machineCode)
        {
            string result = "";

            string sql = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    sql = "select DISTINCT opc1.machine from l2_opc_tag_master OPC INNER JOIN l2_opc_tag_master OPC1 ON OPC.CHANNEL = opc1.channel "
                          + " where OPC.machine ='" + machineCode + "' AND (OPC1.MACHINE LIKE 'UCM%' OR OPC1.MACHINE LIKE 'LCM%')";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    selectCommand.CommandType = CommandType.Text;
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        while (dreader.Read())
                        {
                            result = dreader[0].ToString();
                        }
                    }

                }
            }
            catch (Exception errmsg)
            {

            }

            return result;
        }

        public string GetMachineMaintenancePath()
        {
            string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='Maintenance' and PROPERTY_NAME = 'ExcelMapPath'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelPath = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelPath;
        }
        public void SaveMachineMaintenancePath(string excelPath)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_CONFIG_MASTER set value ='" + excelPath + "' where MODULE_NAME ='Maintenance' and PROPERTY_NAME = 'ExcelMapPath'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
            
        }

        public int GetExitEstimateTime()
        {
            int exitEstimateTime = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='EXIT_EST_TIME' and PROPERTY_NAME = 'EstimateTime'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    exitEstimateTime = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            finally
            { }
            return exitEstimateTime;
        }
        public void SaveExitEstmateTime(int exitEstTime)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_CONFIG_MASTER set value =" + exitEstTime + " where MODULE_NAME ='EXIT_EST_TIME' and PROPERTY_NAME = 'EstimateTime'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }

        public int GetMobExitEstimateTime()
        {
            int exitEstimateTime = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='Exit_Est_Time' and PROPERTY_NAME = 'MobExitEstimateTime'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    exitEstimateTime = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            finally
            { }
            return exitEstimateTime;
        }
        public void SaveMobExitEstmateTime(int exitEstTime)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_CONFIG_MASTER set value =" + exitEstTime + " where MODULE_NAME ='Exit_Est_Time' and PROPERTY_NAME = 'MobExitEstimateTime'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }


        public string GetDisplayXMLPath()
        {
            string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='Display_XML_Path' and PROPERTY_NAME = 'DISPXMLPATH'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelPath = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelPath;
        }
        public void SaveDisplayXMLPath(string displayxml)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update L2_CONFIG_MASTER set value ='" + displayxml + "' where MODULE_NAME ='Display_XML_Path' and PROPERTY_NAME = 'DISPXMLPATH'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
        }

        public string GetCarWashEnableIndicationXMLPath()
        {
            string xmlPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='CarWash' and PROPERTY_NAME = 'EnableXMLPath'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    xmlPath = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return xmlPath;
        }
        public string GetCarWashEnableIndication()
        {
            string isCarWashEnable = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='CarWash' and PROPERTY_NAME = 'IsEnable'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    isCarWashEnable = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return isCarWashEnable;
        }
        public void SaveCarWashEnableIndication(string isCarWashEnable)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update L2_CONFIG_MASTER set value ='" + isCarWashEnable + "' where MODULE_NAME ='CarWash' and PROPERTY_NAME = 'IsEnable'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
        }

        public string GetEESPhotoPath()
        {
            string path = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='EES_Photo_Path' and PROPERTY_NAME = 'EESPhotoPath'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    path = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return path;
        }
        public void SaveEESPhotoPath(string eesPhotoPath)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update L2_CONFIG_MASTER set value ='" + eesPhotoPath + "' where MODULE_NAME ='EES_Photo_Path' and PROPERTY_NAME = 'EESPhotoPath'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
        }

        public string GetEstCarWashTimeXMLPath()
        {
            string carWashEstTimePath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='CARWASH' and ITEM_NAME = 'EST_XML_PATH' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    carWashEstTimePath = Convert.ToString(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            finally
            { }
            return carWashEstTimePath;
        }
        public void SaveEstCarWashTimeXMLPath(string carWashEstXMLPath)
        {
            // string excelPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update L2_CONFIG_MASTER set value ='" + carWashEstXMLPath + "' where  MODULE_NAME ='CARWASH' and ITEM_NAME = 'EST_XML_PATH' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
        }

        public string GetCMMachineExcelName(int rptType,string machineCode)
        {
            string excelName = "";
            string sql = "";
            try
            {
                

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if (rptType == 1)//maintenance history
                        sql = "select MAINT_HISTORY_EXCEL from L2_LCM_UCM_MASTER where MACHINE_CODE ='"+ machineCode + "'";
                    else if (rptType == 2)//preventive
                        sql = "select PREVENTIVE_EXCEL from L2_LCM_UCM_MASTER where MACHINE_CODE ='" + machineCode + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelName = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelName;
        }

        public string GetVLCMachineExcelName(int rptType, string machineCode)
        {
            string excelName = "";
            string sql = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if (rptType == 1)//maintenance history
                        sql = "select MAINT_HISTORY_EXCEL from L2_VLC_MASTER where MACHINE_CODE ='" + machineCode + "'";
                    else if (rptType == 2)//preventive
                        sql = "select PREVENTIVE_EXCEL from L2_VLC_MASTER where MACHINE_CODE ='" + machineCode + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelName = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelName;
        }

        public string GetPSMachineExcelName(int rptType, string machineCode)
        {
            string excelName = "";
            string sql = "";
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    if (rptType == 1)//maintenance history
                        sql = "select MAINT_HISTORY_EXCEL from l2_ps_master where MACHINE_CODE ='" + machineCode + "'";
                    else if (rptType == 2)//preventive
                        sql = "select PREVENTIVE_EXCEL from l2_ps_master where MACHINE_CODE ='" + machineCode + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelName = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelName;
        }

        public string GetPVLMachineExcelName(int rptType, string machineCode)
        {
            string excelName = "";
            string sql = "";
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if (rptType == 1)//maintenance history
                        sql = "select MAINT_HISTORY_EXCEL from L2_PVL_MASTER where MACHINE_CODE ='" + machineCode + "'";
                    else if (rptType == 2)//preventive
                        sql = "select PREVENTIVE_EXCEL from L2_PVL_MASTER where MACHINE_CODE ='" + machineCode + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelName = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelName;
        }

        public string GetPSTMachineExcelName(int rptType, string machineCode)
        {
            string excelName = "";
            string sql = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if (rptType == 1)//maintenance history
                        sql = "select MAINT_HISTORY_EXCEL from L2_PST_MASTER where MACHINE_CODE ='" + machineCode + "'";
                    else if (rptType == 2)//preventive
                        sql = "select PREVENTIVE_EXCEL from L2_PST_MASTER where MACHINE_CODE ='" + machineCode + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    excelName = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return excelName;
        }

        public int HasCardIdExistInParking(string cardId)
        {
            string sql = "";
            int hasExist = 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    //sql = "select count(is_retrieved) from l2_customers where is_retrieved = 0 and card_id = '" + cardId + "'";
                    sql = "select count(slot_type) from l2_proc_snapshot where slot_status in(1,2) and slot_type in (1,2) and value = '" + cardId + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(command.ExecuteScalar().ToString(), out hasExist);
                }
            }
            catch (Exception errMsg)
            { 
            
            }
            finally
            { }
            return hasExist;
        }


        //created on July31,2014: for checking the given slot is valid for car parking
        public Boolean isVacantValidSlotForParking(int floor, int aisle, int row, int carType)
        {
            string sql = "";
            int slotStatus = 0;
            Boolean isValid = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select count(SLOT_STATUS) from l2_proc_snapshot"
                         + " where  F_LEVEL_ID = " + floor
                         + " and AISLE = " + aisle
                         + " and F_ROW = " + row
                         + " and SLOT_STATUS=" + slotStatus;
                         //+ " and MANUAL_BLOCK=0";
                    if (carType == 1)
                        sql += " and SLOT_TYPE in (1,2,3)" ;
                    else if (carType == 3)
                        sql += " and SLOT_TYPE in (2,3)";
                    else
                        sql += " and SLOT_TYPE=2" ;
                         
                         

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    isValid=Convert.ToInt32(command.ExecuteScalar().ToString()) == 0 ? false : true;
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return isValid;
        }

        //created on July31,2014: for checking the given slot is already selected for car washing
        public Boolean isWashingSlot(int floor, int aisle, int row)
        {
            string sql = "";
           
            Boolean isWash = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                   
                    sql = "select count(status) from l2_car_wash_queue"
                         + " where  car_floor = " + floor
                         + " and car_aisle = " + aisle
                         + " and car_row = " + row
                         + " and status!=1";
                        


                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    isWash = Convert.ToInt32(command.ExecuteScalar().ToString()) == 1 ? true : false;
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return isWash;
        }

        public int HasCarDetailsExist(string cardId)
        {
            string sql = "";
            int hasExist = 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select count(is_retrieved) from l2_customers where is_retrieved = 0 and card_id = '" + cardId + "'";
                    //sql = "select count(slot_type) from l2_proc_snapshot where slot_status in(1,2) and slot_type in (1,2) and value = '" + cardId + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(command.ExecuteScalar().ToString(), out hasExist);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return hasExist;
        }
        public int HasCardIdValidToSave(string cardId,int floor, int aisle, int row)
        {
            string sql = "";
            int hasValid = 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select count(ID) from l2_proc_snapshot where value = '" +  cardId +"' and f_level_id = " + floor + 
                        " and aisle = " + aisle + " and f_row = " + row;

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(command.ExecuteScalar().ToString(), out hasValid);
                }
            }
            finally
            { }
            return hasValid;
        }

        public int GetExitEESEstimateTime()
        {
            int extimateAddTime = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='EXIT_EST_TIME' and PROPERTY_NAME = 'EstimateTime'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    extimateAddTime = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            finally
            { }
            return extimateAddTime;
        }
        
        public void UpdateMachineValues(string machine, int minWin, int maxWin, int currentPosition, int currentRow,
           bool autoMode) //changed home aisle = current position.
        {

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    //VIRTUAL_AISLE_MIN, VIRTUAL_AISLE_MAX,  CHANGED_HOME_AISLE
                    string sql = "update l2_lcm_ucm_master set VIRTUAL_AISLE_MIN =" + minWin
                      + ", VIRTUAL_AISLE_MAX = " + maxWin + ", CHANGED_HOME_AISLE = " + currentPosition
                      + ", CHANGED_HOME_ROW =" + currentRow
                      + ", AUTO_MODE = " + (autoMode ? 1 : 0)
                      + " where MACHINE_CODE = '" + machine + "' AND AUTO_MODE != " + (autoMode ? 1 : 0);

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }

        }

        public void UpdateVLCAutoMode(bool isAutoMode, string machineName)
        {
            string qry = "";
            int autoMode = 0;
            autoMode = isAutoMode == true ? 1 : 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "UPDATE L2_VLC_MASTER SET IS_AUTOMODE = " + autoMode
                               + " WHERE machine_code ='" + machineName + "' AND IS_AUTOMODE != " + autoMode;

                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
                //   rpt.Notification("UPDATECMPOSITION" + errMsg.Message + ", " + machineName);
            }
        }

        public void UpdateEESAutoMode(bool isAutoMode, string machineName)
        {
            string qry = "";
            int autoMode = 0;
            autoMode = isAutoMode == true ? 1 : 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "UPDATE L2_EES_MASTER SET IS_AUTOMODE = " + autoMode
                               + " WHERE machine_code ='" + machineName + "' AND IS_AUTOMODE != " + autoMode;

                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
                //   rpt.Notification("UPDATECMPOSITION" + errMsg.Message + ", " + machineName);
            }
        }

        public void UpdateCMBlockedStatusForHomMove(string machineName, int isBlocked, int blockedForHome, Int16 position)
        {
            string qry = "";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        if (position > 0)
                        {
                            qry = "UPDATE L2_LCM_UCM_MASTER SET is_blocked = " + isBlocked
                                + ",BLOCKED_FOR_HOME = " + blockedForHome
                                + " ,changed_home_aisle = " + position
                                + " ,IN_DEMO_MODE = " + isBlocked +
                                " WHERE machine_code ='" + machineName + "'";
                        }
                        else
                        {
                            qry = "UPDATE L2_LCM_UCM_MASTER SET is_blocked = " + isBlocked
                                    + ",BLOCKED_FOR_HOME = " + blockedForHome
                                    + " ,IN_DEMO_MODE = " + isBlocked 
                                    + " WHERE machine_code ='" + machineName + "'";

                        }
                        command.CommandText = qry;
                        command.ExecuteNonQuery();

                    }
                }
            }
            finally
            {
            }
        }

        public void UpdateVLCBlockedStatus(string machineName, int isBlocked)
        {
            string qry = "";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "UPDATE l2_vlc_master SET is_blocked = " + isBlocked +
                                " ,IN_DEMO_MODE = " + isBlocked +
                                  " WHERE machine_code ='" + machineName + "'";
                        
                        command.CommandText = qry;
                        command.ExecuteNonQuery();

                    }
                }
            }
            finally
            {
            }
        }

        public void UpdateBlockStatusOnPalletShuttle(string palletShuttle, int status)
        {

            bool bOk = false;
            
            string methodname = ""; // e.g.
            try
            {
              
                    using (OracleConnection con = new OracleConnection( Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        OracleCommand command = con.CreateCommand();
                        string sql = "update L2_PS_MASTER set IS_BLOCKED =" +status
                               + " ,IN_DEMO_MODE = " + status 
                            + " where MACHINE_CODE = '" + palletShuttle + "'";
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                                             
                    }
            }
            catch (Exception errMsg)
            {
               
            }

        
        }

        public void IsCMMachineBlocked(string machine,out bool isBlocked, out bool isInDemoMode)
        {
            string sql = "";
            //int isSelectable = 0;
            isBlocked = true;
            isInDemoMode = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select is_blocked,IN_DEMO_MODE from L2_LCM_UCM_MASTER where machine_code = '" + machine + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    using (OracleDataReader dreader = command.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            dreader.Read();
                            isBlocked = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                            isInDemoMode = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                        }
                    }
                   // int.TryParse(command.ExecuteScalar().ToString(), out isSelectable);
                }
            }
            finally
            { }
        }

        public void IsVLCMachineBlocked(string machine, out bool isBlocked, out bool isInDemoMode)
        {
            string sql = "";
            isBlocked = true;
            isInDemoMode = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select is_blocked,IN_DEMO_MODE from l2_vlc_master where machine_code = '" + machine + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (dreader.HasRows)
                        {
                            dreader.Read();
                            isBlocked = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                            isInDemoMode = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                        }
                    }

                }
            }
            finally
            { }
         
        }

        public void IsPSMachineBlocked(string machine, out bool isBlocked, out bool isInDemoMode)
        {
            string sql = "";
            isBlocked = true;
            isInDemoMode = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select is_blocked,IN_DEMO_MODE  from L2_PS_MASTER where machine_code = '" + machine + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (dreader.HasRows)
                        {
                            dreader.Read();
                            isBlocked = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                            isInDemoMode = Convert.ToInt32(dreader[0]) == 0 ? false : true;
                        }
                    }
                }
            }
            finally
            { }
        
        }
        
        public int GetUpdatedFloorInSnapShot(string rowid)
        {
            string sql = "";
            int floor = 0;

            try
            {
                if (!string.IsNullOrEmpty(rowid))
                {
                    using (OracleConnection con = new OracleConnection( Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        OracleCommand command = con.CreateCommand();

                        sql = "select F_LEVEL_ID from l2_proc_snapshot where  rowid = '" + rowid + "'";

                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        int.TryParse(command.ExecuteScalar().ToString(), out floor);
                    }
                }
            }
            catch (Exception errMsg)
            { 
            
            }
            finally
            { }
            return floor;
        }

        public int GetMachineStatus(string machineName, string machineType)
        {
            string sql = "";
            int status = 0;

            try
            {
                if (!string.IsNullOrEmpty(machineName) && !string.IsNullOrEmpty(machineType))
                {
                    using (OracleConnection con = new OracleConnection( Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        OracleCommand command = con.CreateCommand();

                        if (machineType.Contains("LCM") || machineType.Contains("UCM"))
                            sql = "SELECT STATUS FROM l2_lcm_ucm_master WHERE MACHINE_CODE = '" + machineName + "'";
                        else if (machineType.Contains("VLC"))
                            sql = "select STATUS from L2_VLC_MASTER where  MACHINE_CODE = '" + machineName + "'";
                        else if (machineType.Contains("PST"))
                            sql = "select STATUS from L2_PST_MASTER where  MACHINE_CODE = '" + machineName + "'";
                        else if (machineType.Contains("PS"))
                            sql = "select STATUS from L2_PS_MASTER where  MACHINE_CODE = '" + machineName + "'";
                        else if (machineType.Contains("PVL"))
                            sql = "select STATUS from L2_PVL_MASTER where  MACHINE_CODE = '" + machineName + "'";
                        else if (machineType.Contains("EES"))
                            sql = "select STATUS from L2_EES_MASTER where  MACHINE_CODE = '" + machineName + "'";


                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        int.TryParse(command.ExecuteScalar().ToString(), out status);
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return status;
        }

        public static void GetEstimationTimeForWashing(out string inEnglishEstmationTime, out string inArabicEstmationTime)
        {
            string sql = "";
            int count = 0;
             inEnglishEstmationTime = "";
             inArabicEstmationTime = "";
            try
            {
                    using (OracleConnection con = new OracleConnection( Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        OracleCommand command = con.CreateCommand();

                        sql = "SELECT COUNT(WASH_Q_ID) FROM L2_CAR_WASH_QUEUE where STATUS in (0,2)";
                     
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        int.TryParse(command.ExecuteScalar().ToString(), out count);

                        if (count != 0)
                        {
                            var val1 = (((Convert.ToDecimal((count + 1)) * Convert.ToDecimal(30))) / Convert.ToDecimal(60)).ToString() +" hr";
                            var val2 = (((Convert.ToDecimal((count + 1)) * Convert.ToDecimal(30))) / Convert.ToDecimal(60)).ToString() +" ساعات";
                          
                            string[] engsplit = val1.Split('.');
                            string[] arabicsplit = val2.Split('.');

                            if (engsplit.Length > 1)
                                inEnglishEstmationTime = engsplit[0] + ":30 hr";
                            else
                                inEnglishEstmationTime = Convert.ToString(val1);

                            if (arabicsplit.Length > 1)
                                inArabicEstmationTime = arabicsplit[0] + ":30 ساعات";
                            else
                                inArabicEstmationTime = Convert.ToString(val2);
                        }
                        else
                        {
                            inEnglishEstmationTime = ("30 Min");
                            inArabicEstmationTime = ("30 دقيقة");
                        }
                    }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            
        }

        public static int GetCurrentMode()
        {
            string sql = "";
            int currentMode = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "SELECT MANUAL_MODE FROM VIEW1";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(command.ExecuteScalar().ToString(), out currentMode);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return currentMode;
        }

        public bool CarType(string cardId)
        {
            bool isHighCar = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString)) // new  Connection().getDBConnection())
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT car_type FROM l2_customers WHERE CARD_ID ='" + cardId + "'  and is_retrieved = 0";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());
                    if (bResult == 1)
                        isHighCar = false;
                    else
                        isHighCar = true;
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
            finally
            {
            }

            return isHighCar;
        }
        
        public DataTable GetMachines(int leveId)
        {
            string query = "SELECT * FROM L2_PROC_SNAPSHOT where F_LEVEL_ID = " + leveId + "  order by F_ROW,AISLE";

            DataTable dt = new DataTable();
            dt.TableName = "SNAPSHOT";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public DataTable GetCMRunTimeTable(int cmPage,bool isRem)
        {
            DataTable dt = new DataTable();
            string query = "";
            if (cmPage == 1)
            {
                if (!isRem)
                    query = "select * from vw_run_time_table  where (MACHINE like 'LCM%' or machine like 'UCM%') and (floor >= 1 and floor <= 4) order by floor,channel";
                else
                    query = "select * from vw_run_time_table  where MACHINE like 'REM%' and (floor >= 1 and floor <= 4) order by floor,channel";
            }
            else if (cmPage == 2)
            {
                if (!isRem)
                    query = "select * from vw_run_time_table  where (MACHINE like 'LCM%' or machine like 'UCM%') and ( floor >= 5 and floor <= 9) order by floor,channel";
                else
                    query = "select * from vw_run_time_table  where MACHINE like 'REM%' and ( floor >= 5 and floor <= 9) order by floor,channel";
            }
            else if (cmPage == 3)
            {
             query = "select * from vw_run_time_table  where (MACHINE like 'VLC%' or machine like 'PVL%')  order by floor,channel";
            }
            try
            {            
                dt.TableName = "RUNTIMETABLE";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
            }
            finally { }
            return dt;
        }

        //public UInt16 GetTurnTableMachineValues(string machineName)
        //{
        //    UInt16 result = 0;

        //}

        public UInt16 GetSetPoints(string machineName, int type)
        {
            string sql = "";
            UInt16 setPointsValue = 0;

            string filedName = "";
            if (type == 1)
                filedName = "SET_POINTS";
            else if (type == 2)
                filedName = "CABLEREEL_SETPOINTS";
            else if (type == 3)
                filedName = "TTNOROT_SETPOINTS";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "SELECT " + filedName + " FROM L2_RUN_TIME_TABLE WHERE MACHINE_NAME = '" + machineName + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                   UInt16.TryParse(command.ExecuteScalar().ToString(), out setPointsValue);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return setPointsValue;
        
        }

        public float GetSetRemPoints(string machineName, int type)
        {
            string sql = "";
            float setPointsValue = 0;

            string filedName = "";
            if (type == 1)
                filedName = "SET_POINTS";
            else if (type == 2)
                filedName = "CABLEREEL_SETPOINTS";
            else if (type == 3)
                filedName = "TTNOROT_SETPOINTS";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "SELECT " + filedName + " FROM L2_RUN_TIME_TABLE WHERE MACHINE_NAME = '" + machineName + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    float.TryParse(command.ExecuteScalar().ToString(), out setPointsValue);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return setPointsValue;

        }

        public bool SaveSetPoints(string machineName, UInt16 setPointsValue, int type) //1 = machine set pints, 2= cable reel set points, 3=ttrot set points
        {
            string sql = "";
            string filedName = "";
            if (type == 1)
                filedName = "SET_POINTS";
            else if (type == 2)
                filedName = "CABLEREEL_SETPOINTS";
            else if (type == 3)
                filedName = "TTNOROT_SETPOINTS";
            bool bOk = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    sql = "UPDATE L2_RUN_TIME_TABLE SET " + filedName + " = " + setPointsValue + " WHERE MACHINE_NAME = '" + machineName + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    bOk = true;
                }
            }
            catch (Exception errMsg)
            {
                bOk = false;
            }
            finally
            { }
            return bOk;
        }

        public bool SaveSetPoints(string machineName, float setPointsValue, int type) //1 = machine set pints, 2= cable reel set points, 3=ttrot set points
        {
            string sql = "";
            string filedName = "";
            if (type == 1)
                filedName = "SET_POINTS";
            else if (type == 2)
                filedName = "CABLEREEL_SETPOINTS";
            else if (type == 3)
                filedName = "TTNOROT_SETPOINTS";
            bool bOk = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    sql = "UPDATE L2_RUN_TIME_TABLE SET " + filedName + " = " + setPointsValue + " WHERE MACHINE_NAME = '" + machineName + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    bOk = true;
                }
            }
            catch (Exception errMsg)
            {
                bOk = false;
            }
            finally
            { }
            return bOk;
        }

        public void DeleteAlam(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "DELETE FROM L2_ALARM WHERE INITDATETIME BETWEEN '" + dateFrom.ToString("dd/MMM/yyyy hh:mm:ss tt")
                        + "' AND '" + dateTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally { }
        }

        public bool HasCMInL2Mode(string machineName)
        {
            string query = "";
            int status = 0;
            //channel, MACHINE_CODE,
            query = "select STATUS FROM L2_LCM_UCM_MASTER T ";//where  T.MACHINE_CODE = '" + machineName + "'";

            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader())
                    {
                        if (dreader.HasRows == true)
                        {
                            dreader.Read();
                            //instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                           // isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                            status = Convert.ToInt32(dreader[0]);
                            isInL2Mode = status != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }
        public bool HasVLCInL2Mode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_VLC_MASTER T INNER JOIN l2_opc_tag_master OPC "
                        + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";
            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dreader.HasRows == true)
                        {
                            dreader.Read();
                            instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                            isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }
        public bool HasPSInL2Mode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_PS_MASTER T INNER JOIN l2_opc_tag_master OPC "
                        + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";
            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dreader.HasRows == true)
                        {
                            instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                            isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }
        public bool HASPSTnL2Mode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_PST_MASTER T INNER JOIN l2_opc_tag_master OPC "
                        + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";
            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dreader.HasRows == true)
                        {
                            instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                            isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }
        public bool HasPVLTnL2Mode(string machineName)
        {
            string query = "select channel, MACHINE_CODE, STATUS FROM L2_PVL_MASTER T INNER JOIN l2_opc_tag_master OPC "
                          + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";
            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dreader.HasRows == true)
                        {
                            instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                            isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }
        public bool HasEESTnL2Mode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE, STATUS FROM L2_EES_MASTER T INNER JOIN l2_opc_tag_master OPC "
                          + " ON T.MACHINE_CODE = OPC.MACHINE  WHERE OPC.MACHINE = '" + machineName + "'";
            bool isInL2Mode = false;
            string instruction = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dreader.HasRows == true)
                        {
                            instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                            isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;

                        }
                    }
                }
            }
            return isInL2Mode;
        }

        /// <summary>
        /// The query is used to register notification creation from table.
        /// </summary>
        public void RegisterEventInOracleDB()
        {
            string qry = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "alter system set events '10867 trace name context forever, level 1'";
                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }

        public bool GetMachineLockStatus(string machineName, int type) //1=psh, 2=pst, 3=pvl,4=EES
        {
            string sql = "";
            bool isLocked = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if(type ==1)
                        sql = "SELECT COUNT(*) FROM L2_PS_MASTER   WHERE (IS_BLOCKED = 1 OR IS_HOME_MOVE = 1) AND MACHINE_CODE = '" + machineName + "'";
                    else if (type == 2)
                        sql = "SELECT COUNT(*) FROM L2_PST_MASTER  WHERE IS_BLOCKED = 1 AND MACHINE_CODE = '" + machineName + "'";
                    else if (type == 3)
                        sql = "SELECT COUNT(*) FROM L2_PVL_MASTER  WHERE IS_BLOCKED = 1 AND MACHINE_CODE = '" + machineName + "'";
                    else if (type == 4)
                        sql = "SELECT COUNT(*) FROM L2_EES_MASTER  WHERE IS_IN_PROCESS = 1 AND MACHINE_CODE = '" + machineName + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int count = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out count);
                    isLocked = (count > 0);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return isLocked;
        }

        public void UpdateMachineBlockStatus(string machineName, int type) //1=psh, 2=pst, 3=pvl
        {

            string sql = "";
          
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    if (type == 1)
                        sql = "begin UPDATE L2_PS_MASTER SET IS_BLOCKED = 0,IS_HOME_MOVE = 0  WHERE MACHINE_CODE = '" + machineName + "';"
                            + " UPDATE ps_locked_data SET LOCKED = 0,CLEARING = 0  WHERE PSNAME = '" + machineName + "'; end;";
                    else if (type == 2)
                        sql = "UPDATE L2_PST_MASTER SET IS_BLOCKED = 0 WHERE MACHINE_CODE = '" + machineName + "'";
                    else if (type == 3)
                        sql = "UPDATE L2_PVL_MASTER SET IS_BLOCKED = 0   WHERE MACHINE_CODE = '" + machineName + "'";
                    else if (type == 4)
                        sql = "UPDATE L2_EES_MASTER SET IS_IN_PROCESS = 0   WHERE MACHINE_CODE = '" + machineName + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {
                Console.Write(errMsg);
            }
        }

        public void UpdateCarWashFinishTrigger(int isCarWashFinish) // 0=vacant, 1=car wash finish signal,2=user trigger to get 
             // car from car wash slot.
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update L2_CONFIG_MASTER set VALUE = " + isCarWashFinish + "  where MODULE_NAME ='CARWASH' " +
                        " and ITEM_NAME = 'FINISH' and PROPERTY_NAME = 'IsTriggered'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }
        }

        public bool IsCarWashFinished() 
        {
            string sql = "";
            bool isFinished = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    sql = "SELECT VALUE FROM L2_CONFIG_MASTER  where MODULE_NAME ='CARWASH' and ITEM_NAME = 'FINISH' and PROPERTY_NAME = 'IsTriggered'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int finished = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out finished);
                    isFinished = (finished == 1);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return isFinished;
        }

        public bool IsCarWashReady()
        {
            string sql = "";
            bool isCarWashReady = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    sql = "SELECT VALUE FROM L2_CONFIG_MASTER  where MODULE_NAME ='CARWASH' and ITEM_NAME = 'FINISH' and PROPERTY_NAME = 'IsTriggered'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int ready = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out ready);
                    isCarWashReady = (ready == 0);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return isCarWashReady;
        }

        public bool GetCarPresentInCarWash()
        {
            string sql = "";
            bool hasCarPresent = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    sql = "SELECT slot_status FROM L2_PROC_SNAPSHOT  where F_LEVEL_ID =1 and AISLE = 22 and F_ROW = 2";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int carPresentStatus = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out carPresentStatus);
                    hasCarPresent = (carPresentStatus == 2);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return hasCarPresent;
        }

        public string GetConfigValue(string moduleName, string itemName, string propertyName)
        {
            string result = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();

                    using (OracleCommand command = con.CreateCommand())
                    {
                        string query = "";

                        if (!string.IsNullOrEmpty(itemName))
                        {
                            query = "select value from l2_config_master where module_name = '" + moduleName + "' and "
                                 + " Item_Name ='" + itemName + "' and property_name = '" + propertyName + "'";
                        }
                        else
                        {
                            query = "select value from l2_config_master where module_name = '" + moduleName + "'"
                                     + " and property_name = '" + propertyName + "'";
                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        result = Convert.ToString(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }
            return result;
        }


        public string GetEntryXMlReqPath()
        {
            string entryXmlReqPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='EntryXMLPath' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    entryXmlReqPath = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return entryXmlReqPath;
        }
        public void SaveEntryXMlReqPath(string entryXmlReqPath)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_CONFIG_MASTER set value =" + entryXmlReqPath + " where MODULE_NAME ='EntryXMLPath' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }
        
        public string GetExitXMlReqPath()
        {
            string exitXmlReqPath = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME ='ExitXMLPath' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    exitXmlReqPath = Convert.ToString(command.ExecuteScalar());
                }
            }
            finally
            { }
            return exitXmlReqPath;
        }
        public void SaveExitXMlReqPath(string exitXmlReqPath)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_CONFIG_MASTER set value =" + exitXmlReqPath + " where MODULE_NAME ='ExitXMLPath' and PROPERTY_NAME = 'Path'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }
        public bool IsPSPuttingEES(string eesCode)
        {
            bool isPutting = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select CONFIG_PACKAGE.is_ps_putting_ees('" + eesCode + "') from dual";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int res = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out res);
                    isPutting = (res == 1);
                }
            }
            finally
            { }
            return isPutting;

        }
        public bool IsLCMGettingEES(string eesCode)
        {
            bool isGetting = false;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select CONFIG_PACKAGE.is_lcm_getting_ees('" + eesCode + "') from dual";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int res = 0;
                    int.TryParse(command.ExecuteScalar().ToString(), out res);
                    isGetting = (res == 1);
                }
            }
            finally
            { }
            return isGetting;

        }

        public void GetEESDetails(string rowid, out string errorCommand, out string channel, out string machineName, out bool isTrigger, out bool isLock)
        {

            errorCommand = "";
            channel = "";
            machineName = "";
            isTrigger = false;
            isLock = false;
            int tmpTrigger = 0;
            int tmpLock = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select DISTINCT TC.COMMAND,TM.channel, TM.MACHINE,IS_TRIGGER,DONE FROM L2_TRIGGER_COMMANDS TC INNER JOIN " +
                                 " l2_opc_tag_master TM on TM.machine = TC.Machine where   TC.rowid ='" + rowid + "'"; //(TC.is_trigger =1 or TC.done = 0) and
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            while (dreader.Read())
                            {
                                errorCommand = Convert.ToString(dreader["COMMAND"]);
                                channel = Convert.ToString(dreader["channel"]);
                                machineName = Convert.ToString(dreader["MACHINE"]);
                                int.TryParse(Convert.ToString(dreader["IS_TRIGGER"]), out tmpTrigger);
                                int.TryParse(Convert.ToString(dreader["DONE"]), out tmpLock);
                                isTrigger = (tmpTrigger == 1);
                                isLock = (tmpLock == 0);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        public List<EESData> GetEESList()
        {
            List<EESData> lstEESData = null;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT EES_ID, EES_NAME,AISLE,F_ROW,START_AISLE,END_AISLE,MACHINE_CODE,COLLAPSE_START"
                                     + " ,COLLAPSE_END,IS_BLOCKED,MORNING_MODE,NORMAL_MODE,EVENING_MODE,STATUS,NORMAL_MIX_EES,IS_AUTOMODE,MACHINE_CHANNEL"
                                     + " FROM L2_EES_MASTER";
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
                Console.WriteLine(errMsg.Message);
            }
            return lstEESData;
        }
       /* public int getCardIdFromAbortedTransaction(string cardId)
        {
            string sql = "";
            int hasExist = 0;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "select count(is_retrieved) from l2_customers where is_retrieved = 0 and card_id = '" + cardId + "'";
                    //sql = "select count(slot_type) from l2_proc_snapshot where slot_status in(1,2) and slot_type in (1,2) and value = '" + cardId + "'";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(command.ExecuteScalar().ToString(), out hasExist);
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            { }
            return hasExist;
        }*/
        public int GetCMMinConfigWindow(string machineCode)
        {
            int minWindow = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select CONFIG_AISLE_MIN from L2_LCM_UCM_MASTER where MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(Convert.ToString(command.ExecuteScalar()), out minWindow);
                }
            }
            finally
            { }
            return minWindow;
        }
        public int GetCMMaxConfigWindow(string machineCode)
        {
            int maxWindow = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "select CONFIG_AISLE_MAX from L2_LCM_UCM_MASTER where MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    int.TryParse(Convert.ToString(command.ExecuteScalar()), out maxWindow);
                }
            }
            finally
            { }
            return maxWindow;
        }
        public void UpdateCMMinConfigWindow(string machineCode, int minWindow)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set CONFIG_AISLE_MIN =" + minWindow + " where MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }
        public void UpdateCMMaxConfigWindow(string machineCode,int maxWindow)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set CONFIG_AISLE_MAX =" + maxWindow + " where MACHINE_CODE ='" + machineCode + "'";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }
        public void ResetCMConfigWindow()
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_LCM_UCM_MASTER set CONFIG_AISLE_MIN=actual_aisle_min, CONFIG_AISLE_MAX=actual_aisle_max";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            finally
            { }

        }
    }
    }
