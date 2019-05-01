using ARCPSGUI.Model;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ARCPSGUI.DB
{
    class ERPDba
    {
       
        public void DeleteTransaction(int queueid)
        {
            try
            {
                
                            //string query = "CONFIG_PACKAGE.delete_transaction";
                            string query = "UPDATE L2_EES_QUEUE SET CANCELREQTYPE =1 where ID =" + queueid; //,STATUS = 1
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
            catch (Exception errMsg)
            {

            }
        }

        public void CompleteTransaction(int queueid)
        {
            try
            {
               
                            string query = "UPDATE L2_EES_QUEUE SET CANCELREQTYPE =2 where ID =" + queueid;
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
            catch (Exception errMsg)
            {


            }
        }

        public bool AbortTransaction(int queueId)
        {
            bool success = false;
            try
            {

                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = con.CreateCommand())
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CONFIG_PACKAGE.abort_transaction";
                        command.Parameters.Add("V_QUEUE_ID", OracleDbType.Int64, queueId, ParameterDirection.Input);
                        command.ExecuteNonQuery();
                        success = true;
                    }
                }

            }
            catch (Exception errMsg)
            {

               

            }
           
            return success;
        }
        public void UpdateVLCDynamicEntryStatus(bool status  )
        {
            string query = "UPDATE l2_config_master SET value =" + (status?1:0 )+ " where module_name='PathSelection' and property_name='path_type'";
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
        public bool GetVLCDynamicEntryStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='PathSelection' and property_name='path_type'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }


        /// <summary>
        /// vlc Dynamic Exit
        /// </summary>
        /// <param name="status"></param>
        public void UpdateVLCDynamicExitStatus(bool status)
        {
            string query = "UPDATE l2_config_master SET value =" + (status ? 1 : 0) + " where module_name='PathSelection' and property_name='exit_path_type'";
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
        public bool GetVLCDynamicExitStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='PathSelection' and property_name='exit_path_type'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }
        /// <summary>
        /// Path Priority
        /// </summary>
        /// <param name="status"></param>
        public void SetPathPriorityStatus(bool status)
        {
            string query = "UPDATE l2_config_master SET value =" + (status ? 1 : 0) + " where module_name='PathSelection' and property_name='priority'";
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
        public bool GetPathPriorityStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='PathSelection' and property_name='priority'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }
        /// <summary>
        /// EES Dynamic Exit
        /// </summary>
        /// <param name="status"></param>
        public void SetEESDynamicExitStatus(bool status)
        {
            string query = "UPDATE l2_config_master SET value =" + (status ? 1 : 0) + " where module_name='PathSelection' and property_name='exit_ees_path_type'";
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
        public bool GetEESDynamicExitStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='PathSelection' and property_name='exit_ees_path_type'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }

        /// <summary>
        /// path lock
        /// </summary>
        /// <param name="status"></param>
        public void SetPathLockStatus(bool status)
        {
            string query = "UPDATE l2_config_master SET value =" + (status ? 1 : 0) + " where module_name='PathSelection' and property_name='path_lock'";
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
        public bool GetPathLockStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='PathSelection' and property_name='path_lock'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }

        public DateTime? GetCarOutsideTime(int eesNum)
        {
            DateTime? carOutTime = (DateTime?)null;
            string query = "select CAR_OUTSIDE_TIME from WAIT_CAR_FOR_EES_SNAP where EES_ID=" + eesNum;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    if (!string.IsNullOrEmpty(value))
                        carOutTime = DateTime.Parse(value);
                }
            }
            return carOutTime;
        }
        /// <summary>
        /// peak hour
        /// </summary>
        /// <returns></returns>
        public bool GetPeakHourEnabledStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='SlotSelection' and property_name='PeakHour'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }
        public bool SetPeakHourEnabledStatus(bool isEnabled)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + (isEnabled ? 1 : 0) +
                " where module_name='SlotSelection' and property_name='PeakHour'" +
                 " and value !=" + (isEnabled ? 1 : 0);

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
            return status;
        }

        /// <summary>
        /// Rotation
        /// </summary>
        /// <returns></returns>
        public bool GetEntryRotaionEnabledStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='LCM' and property_name='EntryRotation'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }
        public bool SetEntryRotaionEnabledStatus(bool isEnabled)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + (isEnabled ? 1 : 0) +
                " where module_name='LCM' and property_name='EntryRotation'" +
                 " and value !=" + (isEnabled ? 1 : 0);

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
            return status;
        }
        public DateTime? GetEESReadyTime(int eesNum)
        {
            DateTime? eesReadyTime = (DateTime?)null;
            string query = "select EES_READY_TIME from WAIT_CAR_FOR_EES_SNAP where EES_ID=" + eesNum;
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    if (!string.IsNullOrEmpty(value))
                        eesReadyTime = DateTime.Parse(value);
                }
            }
            return eesReadyTime;
        }
        public DataTable GetERPTasks(string query)
        {
           
            DataTable dt = null;
            try
            {


                dt = new DataTable();
                dt.TableName = "ERPTasks";
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

            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
                // lblErrorMsg.Text = errMsg.Message;
            }
            finally
            {

            }
            return dt;
        }

        public DataTable GetPMSTasks(string eesName)
        {
            string query = "";

            query = "select * from PMS_VIEW order by s_no";


            DataTable dt = new DataTable();
            dt.TableName = "PMSVIEW";
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

            //lblTotalEntryESSTrans.Content = "Current Parking : " + dt.Select("[PARK MODE] = 'ENTRY'").Count();
            //lblTotalExitESSTrans.Content = "Current Retrieval : " + dt.Select(@"[PARK MODE] = 'Exit'").Count();
            return dt;
        }

        public DataTable GetAbortedRecords(DateTime dtFrom, DateTime dtTo)
        {
            string query = "";


            query = "SELECT * FROM ABORTED_TRANSACTIONS_VIEW WHERE S_NO > 0 ";

            query += " AND " + @"""START TIME"" BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '"
                + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";



            DataTable dt = new DataTable();
          
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


        public bool GetHoldReqFlagStatus(int queueId)
        {
            bool holdStatus = false;
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    OracleCommand command = con.CreateCommand();
                    string sql = "select HOLD_REQ_FLAG from  L2_EES_QUEUE   where id=" + queueId + " and HOLD_FLAG !=" + (holdStatus ? 1 : 0);
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    
                    string value = Convert.ToString(command.ExecuteScalar());
                    holdStatus = (value == "1");

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
        public void SetHoldReqFlagStatus(int queueId, bool holdStatus)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_EES_QUEUE set HOLD_REQ_FLAG=" + (holdStatus ? 1 : 0) + "  where id=" + queueId ;
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
        /// Dynamic slot selection
        /// </summary>
        /// <returns></returns>
        public bool GetDynamicSlotEnabledStatus()
        {
            bool isEnabled = false;
            string query = "select VALUE from L2_CONFIG_MASTER where module_name='SlotSelection' and property_name='Dynamic'";
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    string value = Convert.ToString(command.ExecuteScalar());
                    isEnabled = (value == "1");
                }
            }
            return isEnabled;
        }
        public bool SetDynamicSlotEnabledStatus(bool isEnabled)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + (isEnabled ? 1 : 0) +
                " where module_name='SlotSelection' and property_name='Dynamic'" +
                 " and value !=" + (isEnabled ? 1 : 0);

            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public void SetHoldReqFlagStatusForAll( bool holdStatus)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    OracleCommand command = con.CreateCommand();
                    string sql = "update L2_EES_QUEUE set HOLD_REQ_FLAG=" + (holdStatus ? 1 : 0) 
                        + "  where HOLD_REQ_FLAG!=" + (holdStatus ? 1 : 0);
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

      
    }
}
