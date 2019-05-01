using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.Model;
using System.Windows;
using System.Collections.ObjectModel;
using ARCPSGUI.StaticGlobalClass;

namespace ARCPSGUI.DB
{
    class GeneralDba
    {
        public bool GetMachineTriggerStatus(string machineCode)
        {
            bool bOk = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_TRIGGER FROM L2_TRIGGER_COMMANDS WHERE MACHINE ='" + machineCode + "'";
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
        public bool SetTriggerStatus(string machineCode, bool triggerStatus,int triggerAction)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_TRIGGER_COMMANDS set Is_Trigger = '" + (triggerStatus ? 1 : 0)
                        + "',TRIGGER_ACTION=" + triggerAction
                        + " where MACHINE ='" + machineCode + "'" +
                          " and Is_Trigger = '" + (triggerStatus ? 0 : 1) + "' ";
                    
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
        public string GetMachineTriggerCommand(string machineCode)
        {
            string machineCommand = null;
            try
            {
                

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT command FROM L2_TRIGGER_COMMANDS WHERE MACHINE ='" + machineCode + "' and IS_TRIGGER=1";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    machineCommand = Convert.ToString(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(""+errMsg.Message);
            }
            finally
            {
            }

            return machineCommand;
        }
        public string GetCardIdFromQueue(decimal queueId)
        {
            string cardId = null;
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT CUSTOMER_ID FROM L2_EES_QUEUE WHERE ID =" + queueId ;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    cardId = Convert.ToString(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine("" + errMsg.Message);
            }
            finally
            {
            }

            return cardId;
        }
        public bool SetReallocateData(decimal queueId, string machineCode, int reallocateFlag)
        {
            bool bOk = false;


            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update l2_ees_queue set CANCELREQTYPE = '" + reallocateFlag
                        + "',PATH_START_MACHINE='" + machineCode
                        + "' where ID =" + queueId ;

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
        public string SaveChangeToTransferSlot(decimal queueId, int slotFloor, int slotAisle, int slotRow)
        {
         
            string retStatus = "";
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CLICK_TRANSFER_PACKAGE.save_change_transfer_slot";
                        command.Parameters.Add("to_floor", OracleDbType.Int64, slotFloor, ParameterDirection.Input);
                        command.Parameters.Add("to_aisle", OracleDbType.Int64, slotAisle, ParameterDirection.Input);
                        command.Parameters.Add("to_row", OracleDbType.Int64, slotRow, ParameterDirection.Input);
                        command.Parameters.Add("trans_queue_id", OracleDbType.Int64, queueId, ParameterDirection.Input);
                        command.Parameters.Add("ret_string", OracleDbType.Varchar2, 100,"", ParameterDirection.Output);

                        command.ExecuteNonQuery();
                        retStatus = Convert.ToString(command.Parameters["ret_string"].Value);
                    }
                }

            }
            catch (Exception errMsg)
            {
                retStatus = errMsg.Message;

            }
            return retStatus;
        }
        public int GetParkingCountOfDay(string countDate)
        {
            int carCount = 0;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "select count(customer_id) from l2_customers where ENTRY_TIME between DATE_TOSTARTOFDAY('" + countDate + "') and DATE_TOENDOFDAY('" + countDate + "')";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    carCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                carCount = 0;
            }
            finally
            {
            }

            return carCount;
        }
        public int GetRetrievalCountOfDay(string countDate)
        {
            int carCount = 0;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "select count(customer_id) from l2_customers where EXIT_TIME between DATE_TOSTARTOFDAY('" + countDate + "') and DATE_TOENDOFDAY('" + countDate + "')";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    carCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                carCount = 0;
            }
            finally
            {
            }

            return carCount;
        }

        public int GetCarVacantSlots(int carType)
        {
            int count = 0;
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "select count(aisle) " 
                                    +" from l2_proc_snapshot s "
                                    + " where slot_type=" + carType + "  and manual_block=0  and slot_status=0"
                                    +" and REHANDLE=0 "
                                    +" and f_level_id in (select floor_number from l2_floor_data where block_status=0)";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                count = 0;
            }
            finally
            {
            }

            return count;
        }
        public int GetRequestType(decimal queueId)
        {
            int requestType = 0;
            try
            {


                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT IS_ENTRY FROM L2_EES_QUEUE WHERE ID =" + queueId ;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    requestType = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception errMsg)
            {
                requestType = 0;
            }
            finally
            {
            }

            return requestType;
        }
        public void UnRegisterDBNotification()
        {

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {


                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    if ( Connection.SlotDependency != null &&  Connection.SlotDependency.IsEnabled)
                         Connection.SlotDependency.RemoveRegistration(con);
                    

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void UnRegisterGeneralDBNotification()
        {

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {


                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    if ( Connection.SlotDependency != null &&  Connection.SlotDependency.IsEnabled)
                         Connection.SlotDependency.RemoveRegistration(con);
                    if ( Connection.CarWashDependency != null &&  Connection.CarWashDependency.IsEnabled)
                         Connection.CarWashDependency.RemoveRegistration(con);
                    if ( Connection.ConfigMasterDependency != null &&  Connection.ConfigMasterDependency.IsEnabled)
                         Connection.ConfigMasterDependency.RemoveRegistration(con);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public bool InsertQueue(string cardId, int requestType)
        {
            var queueId = 0;
            bool success = false;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "INSERT_QUEUE";
                        command.Parameters.Add("V_QUEUE_ID", OracleDbType.Int64, queueId, ParameterDirection.Output);
                        command.Parameters.Add("cust_id", OracleDbType.Varchar2, 50, cardId, ParameterDirection.Input);
                        command.Parameters.Add("ARG_EES_ID", OracleDbType.Int64, 0, ParameterDirection.Input);
                        command.Parameters.Add("KIOSK_ID", OracleDbType.Varchar2, "", ParameterDirection.Input);
                        command.Parameters.Add("CAR_ID", OracleDbType.Varchar2, "", ParameterDirection.Input);
                        command.Parameters.Add("ARG_REQUEST_TYPE", OracleDbType.Int64, requestType, ParameterDirection.Input);
                        command.Parameters.Add("PRIORITY", OracleDbType.Int64, 1, ParameterDirection.Input); //TODO: NOTIFY
                        command.Parameters.Add("high_car", OracleDbType.Int64, 0, ParameterDirection.Input);
                        command.Parameters.Add("STAT", OracleDbType.Int64, 0, ParameterDirection.Input);
                        command.Parameters.Add("PATRON_NAME", OracleDbType.NVarchar2, 500, "", ParameterDirection.Input);
                        command.Parameters.Add("arg_need_wash", OracleDbType.Int64, 0, ParameterDirection.Input);
                        command.Parameters.Add("RETRIEVAL_TYPE", OracleDbType.Int64, 0, ParameterDirection.Input);
                        command.Parameters.Add("arg_rot_staus", OracleDbType.Char, 0, ParameterDirection.Input);

                        command.ExecuteNonQuery();
                        Int32.TryParse(Convert.ToString(command.Parameters["V_QUEUE_ID"].Value), out queueId);
                        success = queueId != 0;
                    }
                }

            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);

            }
            return success;

        }
        public void SetHoldFlagStatus(int queueId, bool holdStatus)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_EES_QUEUE set HOLD_FLAG=" + (holdStatus ? 1 : 0) + "  where id=" + queueId + " and HOLD_FLAG !=" + (holdStatus ? 1 : 0);
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }
        }

        /// <summary>
        /// get status of holding flag
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        public bool GetHoldFlagStatus(int queueId)
        {
            bool holdStatus = false;
            try
            {
                int bResult = 0;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                    string sql = "SELECT HOLD_FLAG FROM L2_EES_QUEUE WHERE id =" + queueId;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    bResult = Convert.ToInt32(command.ExecuteScalar());
                    holdStatus = bResult == 1;
                }
            }
            catch (Exception errMsg)
            {
            }
            finally
            {
            }
            return holdStatus;
        }
        public bool CompletePMSTask(decimal queueId)
        {

            bool success = false;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PVL_MANIPULATION.task_after_pvl_processing";
                        command.Parameters.Add("pathid", OracleDbType.Int64, queueId, ParameterDirection.Input);
                        command.ExecuteNonQuery();
                        success = true;
                    }
                }

            }
            catch (Exception errMsg)
            {
                success = false;

            }
            return success;
        }
        public MemberData GetMemberDetailsUsingCardId(string cardId)
        {
            string query = "";
            MemberData objMemberData = null;
            try
            {
                query = "SELECT CARD_ID,MEMBER_NAME,PLATE_NO "
                          + " FROM rpmsadmin.L2_MEMBERS WHERE  CARD_ID = '" + cardId + "' ";

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandText = query;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                objMemberData = new MemberData();

                                if (reader.Read())
                                {


                                    objMemberData.cardId = reader["CARD_ID"].ToString();
                                    objMemberData.memberName = reader["MEMBER_NAME"].ToString();
                                    objMemberData.PlateNo = reader["PLATE_NO"].ToString();


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
            finally
            {
            }
            return objMemberData;
        }
        public void InsertQueueForSimulation(QueueData objQueueData)
        {
            var queueId = 0;
            int seqNumber = 0;

            try
            {
                seqNumber = GetOracleSequenceValue();

                if (string.IsNullOrEmpty(objQueueData.patronName))
                    objQueueData.patronName = "Simulation_" + Math.Abs(seqNumber);
                if (string.IsNullOrEmpty(objQueueData.customerId))
                    objQueueData.customerId = "SM_" + seqNumber.ToString();

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                        if (con.State == System.Data.ConnectionState.Closed) con.Open();
                        using (OracleCommand command = con.CreateCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "INSERT_QUEUE";
                            command.Parameters.Add("V_QUEUE_ID", OracleDbType.Int64, queueId, ParameterDirection.Output);
                            command.Parameters.Add("cust_id", OracleDbType.Varchar2, 50, objQueueData.customerId, ParameterDirection.Input);
                            command.Parameters.Add("ARG_EES_ID", OracleDbType.Int64, objQueueData.eesNumber, ParameterDirection.Input);
                            command.Parameters.Add("KIOSK_ID", OracleDbType.Varchar2, objQueueData.kioskId, ParameterDirection.Input);
                            command.Parameters.Add("CAR_ID", OracleDbType.Varchar2, objQueueData.plateNumber, ParameterDirection.Input);
                            command.Parameters.Add("ARG_REQUEST_TYPE", OracleDbType.Int64, objQueueData.requestType, ParameterDirection.Input);
                            command.Parameters.Add("PRIORITY", OracleDbType.Int64, 1, ParameterDirection.Input); //TODO: NOTIFY
                            command.Parameters.Add("high_car", OracleDbType.Int64, objQueueData.carType, ParameterDirection.Input);
                            command.Parameters.Add("STAT", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("PATRON_NAME", OracleDbType.NVarchar2, 500, objQueueData.patronName, ParameterDirection.Input);
                            command.Parameters.Add("arg_need_wash", OracleDbType.Int64, (objQueueData.needWash ? 1 : 0), ParameterDirection.Input);
                            command.Parameters.Add("RETRIEVAL_TYPE", OracleDbType.Int64, objQueueData.retrievalType, ParameterDirection.Input);
                            command.Parameters.Add("arg_rot_staus", OracleDbType.Char, (objQueueData.isRotate ? 1 : 0), ParameterDirection.Input);

                            command.ExecuteNonQuery();
                            Int32.TryParse(Convert.ToString(command.Parameters["V_QUEUE_ID"].Value), out queueId);
                            objQueueData.queuePkId = queueId;
                        }
                    }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);

            }
        }
        int GetOracleSequenceValue()
        {
            int seqVal = 0;
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                using (OracleCommand command = con.CreateCommand())
                {
                    command.Connection = con;
                    command.CommandText = "select SIMULATION_CAR_SEQ.nextval from dual";
                    command.CommandType = CommandType.Text;
                    Int32.TryParse(command.ExecuteScalar().ToString(), out seqVal);
                }
            }

            return seqVal;
        }
        public List<DBLogData> GetDBLogData(decimal queueId)
        {
            string query = "";
            List<DBLogData> logDataList = null;
            try
            {


                query = " SELECT * FROM (SELECT QUEUEID,MESSAGE,TIME,TRACK_ID,PACKAGE_NAME,PROCEDURE_NAME "
                          + " FROM TRACKFLOW WHERE 1=1  ";
                
                if (queueId != 0)
                {
                    query += " and QUEUEID = '" + queueId + "' ";
                    
                }
                else
                {
                    query += " and time>sysdate - (30/1440) ";
                }
                query += " order by TRACK_ID desc)";
               // query += ")";
                if (queueId == 0)
                {
                    query += " WHERE rownum<=100";
                }

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandText = query;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                logDataList = new List<DBLogData>();

                                while (reader.Read())
                                {
                                    DBLogData objDBLogData = new DBLogData();

                                    objDBLogData.TrackId = Convert.ToDecimal(reader["TRACK_ID"].ToString());
                                    objDBLogData.QueueId = Convert.ToDecimal(reader["QUEUEID"].ToString());
                                    objDBLogData.Message = reader["MESSAGE"].ToString();

                                    objDBLogData.Time = Convert.ToDateTime(reader["TIME"].ToString());
                                    objDBLogData.PackageName = reader["PACKAGE_NAME"].ToString();
                                    objDBLogData.ProcedureName = reader["PROCEDURE_NAME"].ToString();

                                    logDataList.Add(objDBLogData);
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception errMsg)
            {

            }
            finally
            {
            }
            return logDataList;
        }


        //public ObservableCollection<DBLogData>  GetDBLogData(decimal queueId  )
        //{
        //    string query = "";
        //    ObservableCollection<DBLogData> logDataCollection=null;
        //    try
        //    {


        //        query = " SELECT * FROM (SELECT QUEUEID,MESSAGE,TIME,TRACK_ID,PACKAGE_NAME,PROCEDURE_NAME "
        //                  + " FROM TRACKFLOW WHERE 1=1 ";
        //        if (queueId != 0)
        //        {
        //            query += " and QUEUEID = '" + queueId + "' ";
        //        }
        //        query += " order by TRACK_ID desc)";
        //        query += " WHERE rownum<=200";

        //        using (OracleConnection con = new OracleConnection(Connection.connectionString))
        //        {
        //            if (con.State == System.Data.ConnectionState.Closed) con.Open();
        //            using (OracleCommand command = con.CreateCommand())
        //            {

        //                command.CommandText = query;
        //                using (OracleDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        logDataCollection = new ObservableCollection<DBLogData>();

        //                        while (reader.Read())
        //                        {
        //                            DBLogData objDBLogData = new DBLogData();

        //                            objDBLogData.TrackId = Convert.ToDecimal(reader["TRACK_ID"].ToString());
        //                            objDBLogData.QueueId = Convert.ToDecimal(reader["QUEUEID"].ToString());
        //                            objDBLogData.Message = reader["MESSAGE"].ToString();

        //                            objDBLogData.Time = Convert.ToDateTime(reader["TIME"].ToString());
        //                            objDBLogData.PackageName = reader["PACKAGE_NAME"].ToString();
        //                            objDBLogData.ProcedureName = reader["PROCEDURE_NAME"].ToString();

        //                            logDataCollection.Add(objDBLogData);
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //    finally
        //    {
        //    }
        //    return logDataCollection;
           
        //}


        public int GetPathlockId()
        {
            int lockId = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.Connection = con;
                        command.CommandText = "select LOCK_ID from L2_MODULE_LOCK_DATA where LOCK_STATUS=1 and MODULE_ID=1";
                        command.CommandType = CommandType.Text;
                        Int32.TryParse(command.ExecuteScalar().ToString(), out lockId);
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return lockId;
        }
        public void SaveModeMaster(long morningStartHour, long morningEndHour, long morningStartMin, long morningEndMin,
            long eveningStartHour, long eveningEndHour, long eveningStartMin, long eveningEndMin)
        {
           

                string query = "UPDATEMODEMASTER ";

              
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Parameters.Add("MorSTART_HOUR", morningStartHour);
                        command.Parameters.Add("MorEND_HOUR", morningEndHour);
                        command.Parameters.Add("EveSTART_HOUR", eveningStartHour);
                        command.Parameters.Add("EveEND_HOUR", eveningEndHour);
                        command.Parameters.Add("MorSTART_MIN", morningStartMin);
                        command.Parameters.Add("MorEND_MIN", morningEndMin);
                        command.Parameters.Add("EveSTART_MIN", eveningStartMin);
                        command.Parameters.Add("EveEND_MIN", eveningEndMin);

                        command.Connection = con;
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                }
           
        }
        public void BlockFloor(int f1, int f2, int f3, int f4, int f5, int f6, int f7, int f8, int f9)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "BLOCK_FLOOR_PROCEDURE";
                        command.CommandText = sql;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("F1", OracleDbType.Int64, f1, ParameterDirection.Input);
                        command.Parameters.Add("F2", OracleDbType.Int64, f2, ParameterDirection.Input);
                        command.Parameters.Add("F3", OracleDbType.Int64, f3, ParameterDirection.Input);

                        command.Parameters.Add("F4", OracleDbType.Int64, f4, ParameterDirection.Input);
                        command.Parameters.Add("F5", OracleDbType.Int64, f5, ParameterDirection.Input);
                        command.Parameters.Add("F6", OracleDbType.Int64, f6, ParameterDirection.Input);

                        command.Parameters.Add("F7", OracleDbType.Int64, f7, ParameterDirection.Input);
                        command.Parameters.Add("F8", OracleDbType.Int64, f8, ParameterDirection.Input);
                        command.Parameters.Add("F9", OracleDbType.Int64, f9, ParameterDirection.Input);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(" Error occured while saving floor block . " + errMsg.Message);
            }
        }
        public bool GetStatusOfL2AutoSchedule()
        {
            string query = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME= 'SetPoints' and ITEM_NAME = 'L2AutoSchedle'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                   return value == "1";
                }
            }
        }

        public void SaveStatusOfL2AutoSchedule(bool status)
        {
            try
            {
                string query = "update L2_CONFIG_MASTER  set VALUE  = " + (status ? "1" : "0") + " where MODULE_NAME= 'SetPoints' and ITEM_NAME = 'L2AutoSchedle'";
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.Connection = con;
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool GetParkFloorStatus(int floor)
        {
            bool isBlocked = false;
            /* 0 = blocked, > 0 unblocked*/
            string query = "";

            // query = "select count(*) from l2_proc_snapshot where F_LEVEL_ID= " + i + " and slot_type in (1,2)  and manual_block = 0";
            query = "select BLOCK_STATUS from L2_FLOOR_DATA where FLOOR_NUMBER = " + floor;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    int c = Convert.ToInt32(command.ExecuteScalar());
                    isBlocked = c == 1;
                }
            }

            return isBlocked;

        }
        public bool GetPMSFeedFloorStatus(int floor)
        {
            bool isBlocked = false;
            /* 0 = blocked, > 0 unblocked*/
            string query = "";

            // query = "select count(*) from l2_proc_snapshot where F_LEVEL_ID= " + i + " and slot_type in (1,2)  and manual_block = 0";
            query = "select PALLET_FEED_BLOCK from L2_FLOOR_DATA where FLOOR_NUMBER = " + floor;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    int c = Convert.ToInt32(command.ExecuteScalar());
                    isBlocked = c == 1;
                }
            }

            return isBlocked;

        }
        public void UpdateParkFloorStatus(int floor, bool isBlocked)
        {
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    string sql = "";

                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "update L2_FLOOR_DATA set BLOCK_STATUS = " + (isBlocked ? 1 : 0)
                            + " where FLOOR_NUMBER = " + floor;

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while saving normal ees mode. " + errMsg.Message);
            }
        }
        public void UpdatePMSFeedFloorStatus(int floor, bool isBlocked)
        {
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    string sql = "";

                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "update L2_FLOOR_DATA set PALLET_FEED_BLOCK = " + (isBlocked ? 1 : 0)
                            + " where FLOOR_NUMBER = " + floor;

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while saving normal ees mode. " + errMsg.Message);
            }
        }
        public bool GetPMSStoreFloorStatus(int floor)
        {
            bool isBlocked = false;
            /* 0 = blocked, > 0 unblocked*/
            string query = "";

            // query = "select count(*) from l2_proc_snapshot where F_LEVEL_ID= " + i + " and slot_type in (1,2)  and manual_block = 0";
            query = "select PALLET_STORE_BLOCK from L2_FLOOR_DATA where FLOOR_NUMBER = " + floor;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    int c = Convert.ToInt32(command.ExecuteScalar());
                    isBlocked = c == 1;
                }
            }

            return isBlocked;

        }
        public void UpdatePMSStoreFloorStatus(int floor, bool isBlocked)
        {
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    string sql = "";

                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "update L2_FLOOR_DATA set PALLET_STORE_BLOCK = " + (isBlocked ? 1 : 0)
                            + " where FLOOR_NUMBER = " + floor;

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while saving normal ees mode. " + errMsg.Message);
            }
        }

         public void SaveNotification(NotificationData objNotificationData)
        {
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    string sql = "";

                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    sql = "insert into  L2_NOTIFICATIONS(NOTIFY_MACHINE,NOTIFY_TYPE,NOTIFY_ERROR_CODE)" 
                            +" values(:NOTIFY_MACHINE,:NOTIFY_TYPE,:NOTIFY_ERROR_CODE)";
                    command.BindByName = true;
                    command.CommandText = sql;
                    command.Parameters.Add("NOTIFY_MACHINE", objNotificationData.MachineCode);
                    command.Parameters.Add("NOTIFY_TYPE", objNotificationData.category.ToString());
                    command.Parameters.Add("NOTIFY_ERROR_CODE", objNotificationData.ErrorCode);
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("SaveNotification. " + errMsg.Message);
            }
        }
         public List<NotificationData> GetNotifications(NotificationData filterNotificationData)
         {
             string query = "";
             List<NotificationData> notificationList = null;
             try
             {


                 query = "select notif.ID,notif.NOTIFY_MACHINE,notif.NOTIFY_TYPE,notif.NOTIFY_ERROR_CODE,notif.NOTIFY_TIME,er.ERROR_DESCRIPTION"
                        + " from l2_notifications notif left join l2_error_master er "
                        +" on notif.NOTIFY_ERROR_CODE=er.ERROR_CODE and er.ERROR_CODE!=0 "
                        +" and substr(notif.NOTIFY_MACHINE,1,instr(notif.NOTIFY_MACHINE,'_',1,1) -1)  = SUBSTR(er.MACHINE_MODEL, 1, 3) " 

                           + " WHERE 1=1  ";

                 //if (queueId != 0)
                 //{
                 //    query += " and QUEUEID = '" + queueId + "' ";

                 //}
                 //else
                 //{
                 //    query += " and time>sysdate - (30/1440) ";
                 //}
                 //query += " order by TRACK_ID desc)";
                 //// query += ")";
                 //if (queueId == 0)
                 //{
                 //    query += " WHERE rownum<=100";
                 //}

                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == System.Data.ConnectionState.Closed) con.Open();
                     using (OracleCommand command = con.CreateCommand())
                     {

                         if (!string.IsNullOrEmpty(filterNotificationData.MachineCode))
                         {
                             query += " and notif.NOTIFY_MACHINE = :NOTIFY_MACHINE ";
                             command.Parameters.Add("NOTIFY_MACHINE", filterNotificationData.MachineCode);
                         }
                         if (filterNotificationData.category != null && filterNotificationData.category!=NotificationData.errorCategory.NA)
                         {
                             query += " and notif.NOTIFY_TYPE = :NOTIFY_TYPE ";
                             command.Parameters.Add("NOTIFY_TYPE", filterNotificationData.category.ToString());
                         }
                         if (filterNotificationData.FilterNotifyStart != null && filterNotificationData.FilterNotifyStart != default(DateTime))
                         {
                             query += " and notif.NOTIFY_TIME >= :NOTIFY_START";
                             command.Parameters.Add("NOTIFY_START", filterNotificationData.FilterNotifyStart);
                         }
                         if (filterNotificationData.FilterNotifyEnd != null && filterNotificationData.FilterNotifyEnd != default(DateTime))
                         {
                             query += " and notif.NOTIFY_TIME <= :NOTIFY_END";
                             command.Parameters.Add("NOTIFY_END", filterNotificationData.FilterNotifyEnd);
                         }
                         command.CommandText = query;
                         command.BindByName = true;
                         using (OracleDataReader reader = command.ExecuteReader())
                         {
                             if (reader.HasRows)
                             {
                                 notificationList = new List<NotificationData>();

                                 while (reader.Read())
                                 {
                                     NotificationData objNotificationData = new NotificationData();

                                     objNotificationData.Id = Convert.ToInt32(reader["ID"].ToString());
                                     objNotificationData.MachineCode = reader["NOTIFY_MACHINE"].ToString();
                                     objNotificationData.category = (NotificationData.errorCategory)Enum.Parse(typeof(NotificationData.errorCategory), reader["NOTIFY_TYPE"].ToString());

                                     objNotificationData.NotifyTime = Convert.ToDateTime(reader["NOTIFY_TIME"].ToString());
                                     objNotificationData.ErrorDescription = reader["ERROR_DESCRIPTION"].ToString();

                                     notificationList.Add(objNotificationData);
                                 }
                             }
                         }
                     }
                 }


             }
             catch (Exception errMsg)
             {

             }
             finally
             {
             }
             return notificationList;
         }
         public List<string> GetAllMachines()
         {
             string query = "";
             List<string> machinesList = null;
             try
             {


                 query = "select machine_code from L2_LCM_UCM_MASTER"
                +" union all"
               +" select machine_code from L2_VLC_MASTER"
               +" union all"
               +" select machine_code from L2_EES_MASTER"
               +" union all"
               +" select machine_code from L2_PS_MASTER"
               +" union all"
               +" select machine_code from L2_PST_MASTER"
               +" union all"
               +" select machine_code from L2_PVL_MASTER";

               

                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == System.Data.ConnectionState.Closed) con.Open();
                     using (OracleCommand command = con.CreateCommand())
                     {

                         command.CommandText = query;
                         using (OracleDataReader reader = command.ExecuteReader())
                         {
                             if (reader.HasRows)
                             {
                                 machinesList = new List<string>();

                                 while (reader.Read())
                                 {


                                     machinesList.Add(reader["machine_code"].ToString());
                                 }
                             }
                         }
                     }
                 }


             }
             catch (Exception errMsg)
             {

             }
             finally
             {
             }
             return machinesList;
         }

         public void SaveExitDisplayMessage(string msg)
         {
             try
             {
                 string query = "update L2_CONFIG_MASTER  set VALUE  = '" + msg + "' where MODULE_NAME= 'EXIT_DISPALY' and PROPERTY_NAME = 'MESSAGE'";
                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == ConnectionState.Closed) con.Open();

                     using (OracleCommand command = new OracleCommand(query))
                     {
                         command.Connection = con;
                         command.CommandText = query;
                         command.CommandType = CommandType.Text;
                         command.ExecuteNonQuery();
                     }
                 }
             }
             catch (Exception errMsg)
             {
                 MessageBox.Show(errMsg.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Error);
             }
         }
         public string GetExitDisplayMessage()
         {
             string msg = null;
             string query = "";

             query = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME= 'EXIT_DISPALY' and PROPERTY_NAME = 'MESSAGE'";
             using (OracleConnection con = new OracleConnection(Connection.connectionString))
             {
                 if (con.State == ConnectionState.Closed) con.Open();

                 using (OracleCommand command = new OracleCommand(query))
                 {
                     command.CommandText = query;
                     command.Connection = con;
                     msg=command.ExecuteScalar().ToString();
                 }
             }

             return msg;

         }

         #region VLC Floor Configuration
         public bool GetFloorVLCStatus(int floor, string VLCCode)
         {
             bool isEnabled = false;
             string query = "";
             try
             {
                 query = "select STATUS from L2_FLOOR_VLC_MASTER where FLOOR = " + floor + " and VLC_CODE ='" + VLCCode + "'";
                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == ConnectionState.Closed) con.Open();

                     using (OracleCommand command = new OracleCommand(query))
                     {
                         command.CommandText = query;
                         command.Connection = con;
                         isEnabled = Convert.ToInt32(command.ExecuteScalar()) == 1;
                     }
                 }
             }
             catch (Exception errMsg)
             {
                 //MessageBox.Show("Error occured GetFloorVLCStatus. " + errMsg.Message);
             }

             return isEnabled;

         }
         public void UpdateFloorVLCStatus(int floor, string VLCCode, bool isBlocked)
         {
             try
             {

                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     string sql = "";

                     if (con.State == ConnectionState.Closed) con.Open();
                     OracleCommand command = con.CreateCommand();

                     sql = "update L2_FLOOR_VLC_MASTER set STATUS = " + (isBlocked ? 1 : 0)
                             + " where FLOOR = " + floor + " and VLC_CODE ='" + VLCCode + "'";

                     command.CommandText = sql;
                     command.CommandType = CommandType.Text;
                     command.ExecuteNonQuery();

                 }
             }
             catch (Exception errMsg)
             {
                 MessageBox.Show("Error occured UpdateFloorVLCStatus. " + errMsg.Message);
             }
         }
         #endregion
         #region Ramp Display

         public string GetRampDisplayMesaage(string propertyName)
         {
             string query = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME= 'RAMP_DISPALY' and PROPERTY_NAME = '" + propertyName + "'";
             using (OracleConnection con = new OracleConnection(Connection.connectionString))
             {
                 if (con.State == ConnectionState.Closed) con.Open();

                 using (OracleCommand command = new OracleCommand(query))
                 {
                     command.CommandText = query;
                     command.Connection = con;
                     string value = Convert.ToString(command.ExecuteScalar());
                     return value ;
                 }
             }
         }
         public void SetRampDisplayMesaage(string propertyName,string message)
         {
             try
             {
                 string query = "update L2_CONFIG_MASTER  set VALUE  = '" + message + "' where MODULE_NAME= 'RAMP_DISPALY' and PROPERTY_NAME = '" + propertyName + "'";
                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == ConnectionState.Closed) con.Open();

                     using (OracleCommand command = new OracleCommand(query))
                     {
                         command.Connection = con;
                         command.CommandText = query;
                         command.CommandType = CommandType.Text;
                         command.ExecuteNonQuery();
                     }
                 }
             }
             catch (Exception errMsg)
             {
                 MessageBox.Show(errMsg.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Error);
             }
         }
         public int GetRoboticParkingStatus()
         {
             string query = "select VALUE from L2_CONFIG_MASTER where MODULE_NAME= 'PARKING' and PROPERTY_NAME = 'STATUS'";
             using (OracleConnection con = new OracleConnection(Connection.connectionString))
             {
                 if (con.State == ConnectionState.Closed) con.Open();

                 using (OracleCommand command = new OracleCommand(query))
                 {
                     command.CommandText = query;
                     command.Connection = con;
                     return int.Parse(Convert.ToString(command.ExecuteScalar()));
                 }
             }
         }
         public void SetRoboticParkingStatus( int status)
         {
             try
             {
                 string query = "update L2_CONFIG_MASTER  set VALUE  = " + status + " where MODULE_NAME= 'PARKING' and PROPERTY_NAME = 'STATUS'";
                 using (OracleConnection con = new OracleConnection(Connection.connectionString))
                 {
                     if (con.State == ConnectionState.Closed) con.Open();

                     using (OracleCommand command = new OracleCommand(query))
                     {
                         command.Connection = con;
                         command.CommandText = query;
                         command.CommandType = CommandType.Text;
                         command.ExecuteNonQuery();
                     }
                 }
             }
             catch (Exception errMsg)
             {
                 MessageBox.Show(errMsg.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Error);
             }
         }
        
            #endregion
         
    }
}
