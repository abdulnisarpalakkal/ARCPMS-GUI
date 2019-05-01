using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ARCPSGUI.Model;
using Oracle.DataAccess.Client;
using System.Collections;

namespace ARCPSGUI.DB
{
    class SlotDba
    {
        public string FloorNotificationQuery = "select F_LEVEL_ID,AISLE, F_ROW,SLOT_TYPE,SLOT_STATUS,VALUE,MANUAL_BLOCK,REHANDLE,PREV_SLOT_STATUS "
                                            + " FROM L2_PROC_SNAPSHOT  ";
        public event EventHandler triggerSlotUpdate;                  
        public int GetInsertQueueClickTransferId(Int64 fromFloor, Int64 fromAisle, Int64 fromRow, int toFloor, int toAisle, int toRow)
        {
            int clickTransferQueueId = 0;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    using (OracleCommand command = con.CreateCommand())
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CLICK_TRANSFER_PACKAGE.insert_queue_for_transfer";

                        command.Parameters.Add("from_floor", OracleDbType.Int32, fromFloor, ParameterDirection.Input);
                        command.Parameters.Add("from_aisle", OracleDbType.Int32, fromAisle, ParameterDirection.Input);
                        command.Parameters.Add("from_row", OracleDbType.Int32, fromRow, ParameterDirection.Input);

                        command.Parameters.Add("to_floor", OracleDbType.Int32, toFloor, ParameterDirection.Input);
                        command.Parameters.Add("to_aisle", OracleDbType.Int32, toAisle, ParameterDirection.Input);
                        command.Parameters.Add("to_row", OracleDbType.Int32, toRow, ParameterDirection.Input);

                        command.Parameters.Add("transfer_q_id", OracleDbType.Int32, clickTransferQueueId, ParameterDirection.Output);

                        command.ExecuteNonQuery();
                        Int32.TryParse(command.Parameters["transfer_q_id"].Value.ToString(), out clickTransferQueueId);
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

            return clickTransferQueueId;
        }
        public List<EESZoneData> GetEESDefaultZoneList()
        {
            List<EESZoneData> defaultZoneList = null;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "select EES_ID,EES_NAME,START_AISLE,END_AISLE,NON_BASE_ZONE_START,NON_BASE_ZONE_END,DEFAULT_BASE_REF_AISLE,DEFAULT_NONBASE_REF_AISLE"

                        + " from L2_EES_MASTER ";
                       
                       

                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                defaultZoneList = new List<EESZoneData>();

                                while (reader.Read())
                                {

                                    EESZoneData objEESZoneData = new EESZoneData();

                                    objEESZoneData.EESId = Int32.Parse(reader["EES_ID"].ToString());
                                    objEESZoneData.EESName = reader["EES_NAME"].ToString();

                                    objEESZoneData.BaseZoneStart = Int32.Parse(reader["START_AISLE"].ToString());
                                    objEESZoneData.BaseZoneEnd = Int32.Parse(reader["END_AISLE"].ToString());
                                    objEESZoneData.NonBaseZoneStart = Int32.Parse(reader["NON_BASE_ZONE_START"].ToString());
                                    objEESZoneData.NonBaseZoneEnd = Int32.Parse(reader["NON_BASE_ZONE_END"].ToString());
                                    objEESZoneData.BaseRefAisle = Int32.Parse(reader["DEFAULT_BASE_REF_AISLE"].ToString());
                                    objEESZoneData.NonBaseRefAisle = Int32.Parse(reader["DEFAULT_NONBASE_REF_AISLE"].ToString());
                                    defaultZoneList.Add(objEESZoneData);
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
            return defaultZoneList;
        }
        public List<EESZoneData> GetEESPeakHourZoneList()
        {
            List<EESZoneData> peakZoneList = null;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "select EES_ID,EES_NAME,PEAK_BASE_ZONE_START,PEAK_BASE_ZONE_END,PEAK_NON_BASE_ZONE_START"
                            +" ,PEAK_NON_BASE_ZONE_END,PEAK_BASE_REF_AISLE,PEAK_NONBASE_REF_AISLE"

                        + " from L2_EES_MASTER ";



                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                peakZoneList = new List<EESZoneData>();

                                while (reader.Read())
                                {

                                    EESZoneData objEESZoneData = new EESZoneData();

                                    objEESZoneData.EESId = Int32.Parse(reader["EES_ID"].ToString());
                                    objEESZoneData.EESName = reader["EES_NAME"].ToString();

                                    objEESZoneData.BaseZoneStart = Int32.Parse(reader["PEAK_BASE_ZONE_START"].ToString());
                                    objEESZoneData.BaseZoneEnd = Int32.Parse(reader["PEAK_BASE_ZONE_END"].ToString());
                                    objEESZoneData.NonBaseZoneStart = Int32.Parse(reader["PEAK_NON_BASE_ZONE_START"].ToString());
                                    objEESZoneData.NonBaseZoneEnd = Int32.Parse(reader["PEAK_NON_BASE_ZONE_END"].ToString());
                                    objEESZoneData.BaseRefAisle = Int32.Parse(reader["PEAK_BASE_REF_AISLE"].ToString());
                                    objEESZoneData.NonBaseRefAisle = Int32.Parse(reader["PEAK_NONBASE_REF_AISLE"].ToString());
                                    peakZoneList.Add(objEESZoneData);
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
            return peakZoneList;
        }
        public List<EESZoneData> GetEESCustomizedZoneList()
        {
            List<EESZoneData> custZoneList = null;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "select EES_ID,EES_NAME,CUST_BASE_ZONE_START,CUST_BASE_ZONE_END,CUST_NON_BASE_ZONE_START,CUST_NON_BASE_ZONE_END,CUST_BASE_REF_AISLE,CUST_NONBASE_REF_AISLE"

                        + " from L2_EES_MASTER ";



                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                custZoneList = new List<EESZoneData>();

                                while (reader.Read())
                                {

                                    EESZoneData objEESZoneData = new EESZoneData();

                                    objEESZoneData.EESId = Int32.Parse(reader["EES_ID"].ToString());
                                    objEESZoneData.EESName = reader["EES_NAME"].ToString();

                                    objEESZoneData.BaseZoneStart = Int32.Parse(reader["CUST_BASE_ZONE_START"].ToString());
                                    objEESZoneData.BaseZoneEnd = Int32.Parse(reader["CUST_BASE_ZONE_END"].ToString());
                                    objEESZoneData.NonBaseZoneStart = Int32.Parse(reader["CUST_NON_BASE_ZONE_START"].ToString());
                                    objEESZoneData.NonBaseZoneEnd = Int32.Parse(reader["CUST_NON_BASE_ZONE_END"].ToString());
                                    objEESZoneData.BaseRefAisle = Int32.Parse(reader["CUST_BASE_REF_AISLE"].ToString());
                                    objEESZoneData.NonBaseRefAisle = Int32.Parse(reader["CUST_NONBASE_REF_AISLE"].ToString());

                                    custZoneList.Add(objEESZoneData);
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
            return custZoneList;
        }
        public bool GetZoneExpandStatus()
        {
            bool isEnabled = false;
            try
            {
                string query = "select VALUE from L2_CONFIG_MASTER where module_name='SlotSelection' and property_name='NeedExpandZone'";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
            }
            catch (Exception ex)
            { 
            
            }
            return isEnabled;
        }
        public bool SetZoneExpandStatus(bool isEnabled)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + (isEnabled ? 1 : 0) +
                " where module_name='SlotSelection' and property_name='NeedExpandZone'" +
                 " and value !=" + (isEnabled ? 1 : 0);

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
        public int GetZoneExpandNumber()
        {
            int number = 0;
            try
            {
                string query = "select VALUE from L2_CONFIG_MASTER where module_name='SlotSelection' and property_name='ExpandNumber'";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        number = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return number;
        }
        public bool SetZoneExpandNumber(int number)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + number +
                " where module_name='SlotSelection' and property_name='ExpandNumber'" +
                 " and value !=" + number;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
        public int GetSlotSelectionType()
        {
            int type = 0;
            try
            {
                string query = "select VALUE from L2_CONFIG_MASTER where module_name='SlotSelection' and property_name='SlotSelectionType'";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        type = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return type;
        }
        public bool SetSlotSelectionType(int type)
        {
            bool status = false;
            string query = "UPDATE l2_config_master SET value =" + type +
                " where module_name='SlotSelection' and property_name='SlotSelectionType'" +
                 " and value !=" + type;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public int GetCustomizedBaseZoneStart(string eesName)
        {
            int zoneStart = 0;

            string query = "select CUST_BASE_ZONE_START from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        zoneStart=Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return zoneStart;
        }
        public bool SetCustomizedBaseZoneStart(string eesName, int zoneStart)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_BASE_ZONE_START =" + zoneStart
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_BASE_ZONE_START !=" + zoneStart;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
        public int GetCustomizedBaseZoneEnd(string eesName)
        {
            int zoneStart = 0;

            string query = "select CUST_BASE_ZONE_END from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        zoneStart = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return zoneStart;
        }
        public bool SetCustomizedBaseZoneEnd(string eesName, int zoneStart)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_BASE_ZONE_END =" + zoneStart
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_BASE_ZONE_END !=" + zoneStart;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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



        public int GetCustomizedNonBaseZoneStart(string eesName)
        {
            int zoneStart = 0;

            string query = "select CUST_NON_BASE_ZONE_START from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        zoneStart = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return zoneStart;
        }
        public bool SetCustomizedNonBaseZoneStart(string eesName, int zoneStart)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_NON_BASE_ZONE_START =" + zoneStart
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_NON_BASE_ZONE_START !=" + zoneStart;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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



        public int GetCustomizedNonBaseZoneEnd(string eesName)
        {
            int zoneStart = 0;

            string query = "select CUST_NON_BASE_ZONE_END from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        zoneStart = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return zoneStart;
        }
        public bool SetCustomizedNonBaseZoneEnd(string eesName, int zoneStart)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_NON_BASE_ZONE_END =" + zoneStart
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_NON_BASE_ZONE_END !=" + zoneStart;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public int GetCustomizedNonBaseRefAisle(string eesName)
        {
            int refAisle = 0;

            string query = "select CUST_NONBASE_REF_AISLE from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        refAisle = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return refAisle;
        }
        public bool SetCustomizedNonBaseRefAisle(string eesName, int refAisle)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_NONBASE_REF_AISLE =" + refAisle
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_NONBASE_REF_AISLE !=" + refAisle;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
        public int GetCustomizedBaseRefAisle(string eesName)
        {
            int refAisle = 0;

            string query = "select CUST_BASE_REF_AISLE from   l2_ees_master"
                + " where EES_NAME='" + eesName + "'";

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        command.CommandType = CommandType.Text;
                        refAisle = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return refAisle;
        }
        public bool SetCustomizedBaseRefAisle(string eesName, int refAisle)
        {
            bool status = false;
            string query = "UPDATE l2_ees_master SET CUST_BASE_REF_AISLE =" + refAisle
                + " where EES_NAME='" + eesName + "'"
                + " and CUST_BASE_REF_AISLE !=" + refAisle;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        public SlotData GetSlotDetails(int slotFloor, int slotAisle, int slotRow)
        {

            SlotData objSlotData = null;
          
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select F_LEVEL_ID,AISLE, F_ROW,SLOT_TYPE,SLOT_STATUS,VALUE,MANUAL_BLOCK,REHANDLE,PREV_SLOT_STATUS FROM L2_PROC_SNAPSHOT"
                        + " where F_LEVEL_ID= " + slotFloor + " and AISLE=" + slotAisle + " and F_ROW=" + slotRow;
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            if (dreader.Read())
                            {
                                objSlotData = new SlotData();
                                objSlotData.SlotFloor = slotFloor;
                                objSlotData.SlotAisle = slotAisle;
                                objSlotData.SlotRow = slotRow;
                               objSlotData.SlotType= int.Parse(Convert.ToString(dreader["SLOT_TYPE"]));
                               objSlotData.SlotStatus = int.Parse(Convert.ToString(dreader["SLOT_STATUS"]));
                               objSlotData.slotValue = Convert.ToString(dreader["VALUE"]);
                               objSlotData.Disable = int.Parse(Convert.ToString(dreader["MANUAL_BLOCK"]))==1;
                               objSlotData.Rehandle = int.Parse(Convert.ToString(dreader["REHANDLE"])) == 1;
                               objSlotData.PrevSlotStatus = int.Parse(Convert.ToString(dreader["PREV_SLOT_STATUS"]));
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return objSlotData;
        }
        public CarData GetSlotCarDetails(String slotValue)
        {
 
            CarData objCarData = null;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select CUSTOMER_ID,IS_MEMBER, CAR_NUMBER,ENTRY_TIME,CAR_TYPE,CARD_ID,GATE,IS_ROTATE,NEED_WASH,IS_COMPLETE_WASH,QUEUE_ID,"
                        + " PUT_TO_SLOT_TIME,ENTRY_NORTH_IMG,ENTRY_SOUTH_IMG,PATRON_NAME FROM L2_CUSTOMERS"
                        + " where CARD_ID= '" + slotValue + "' and IS_RETRIEVED=0";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            if (dreader.Read())
                            {
                                objCarData = new CarData();
                                objCarData.CustomerId = int.Parse(Convert.ToString(dreader["CUSTOMER_ID"]));
                                objCarData.IsMember = int.Parse(Convert.ToString(dreader["IS_MEMBER"])) == 1;
                                objCarData.Plate = Convert.ToString(dreader["CAR_NUMBER"]);
                                objCarData.EntryTime = dreader["ENTRY_TIME"] == DBNull.Value ? System.DateTime.Now :
                                    Convert.ToDateTime(dreader["ENTRY_TIME"]);
                                objCarData.CarType = int.Parse(Convert.ToString(dreader["CAR_TYPE"]));
                                objCarData.CardId = Convert.ToString(dreader["CARD_ID"]);
                                objCarData.EntryGate = Convert.ToString(dreader["GATE"]);

                                objCarData.IsRotated = int.Parse(Convert.ToString(dreader["IS_ROTATE"])) == 1;
                                objCarData.Wash = int.Parse(Convert.ToString(dreader["NEED_WASH"])) == 1;
                                objCarData.IsWashComplete = int.Parse(Convert.ToString(dreader["IS_COMPLETE_WASH"])) == 1;
                                objCarData.EntryQueueId = int.Parse(Convert.ToString(dreader["QUEUE_ID"]));

                                objCarData.PutToSlotTime = dreader["PUT_TO_SLOT_TIME"] == DBNull.Value ? System.DateTime.Now :
                                    Convert.ToDateTime(dreader["PUT_TO_SLOT_TIME"]);
                                objCarData.EntryNorthImg = Convert.ToString(dreader["ENTRY_NORTH_IMG"]);
                                objCarData.EntrySouthImg = Convert.ToString(dreader["ENTRY_SOUTH_IMG"]);
                                objCarData.PatronName = Convert.ToString(dreader["PATRON_NAME"]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return objCarData;
        }
        public Hashtable UpdateSlot(SlotData objSlotData)
        {
            var queueId = 0;

            Hashtable responseTable = null;
            try
            {

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string entry_time = objSlotData.ObjCarData.EntryTime.ToString("dd-MMM-yyyy hh:mm:ss tt");
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PARK_SLOT_MANAGER.update_slot";
                        command.Parameters.Add("arg_card_id", OracleDbType.Varchar2,objSlotData.ObjCarData.CardId, ParameterDirection.Input);
                        command.Parameters.Add("arg_name", OracleDbType.Varchar2, objSlotData.ObjCarData.PatronName, ParameterDirection.Input);
                        command.Parameters.Add("arg_plate_no", OracleDbType.Varchar2, objSlotData.ObjCarData.Plate, ParameterDirection.Input);
                        command.Parameters.Add("arg_floor", OracleDbType.Int32, objSlotData.SlotFloor, ParameterDirection.Input);
                        command.Parameters.Add("arg_aisle", OracleDbType.Int32, objSlotData.SlotAisle, ParameterDirection.Input);
                        command.Parameters.Add("arg_row", OracleDbType.Int32, objSlotData.SlotRow, ParameterDirection.Input);
                        command.Parameters.Add("arg_rotate_status", OracleDbType.Int32, (objSlotData.ObjCarData.IsRotated?1:0), ParameterDirection.Input);
                        command.Parameters.Add("arg_car_type", OracleDbType.Int32, objSlotData.ObjCarData.CarType, ParameterDirection.Input);
                        command.Parameters.Add("arg_slot_status", OracleDbType.Int32, objSlotData.SlotStatus, ParameterDirection.Input);
                        command.Parameters.Add("arg_disable_status", OracleDbType.Int32, (objSlotData.Disable?1:0), ParameterDirection.Input);
                        command.Parameters.Add("arg_date", OracleDbType.TimeStamp, entry_time, ParameterDirection.Input);
                        command.Parameters.Add("arg_rehandle_status", OracleDbType.Char, (objSlotData.Rehandle?1:0), ParameterDirection.Input);
                        command.Parameters.Add("arg_result", OracleDbType.Int64, ParameterDirection.Output);
                        command.Parameters.Add("arg_res_msg", OracleDbType.Varchar2,500,null, ParameterDirection.Output);

                        command.ExecuteNonQuery();
                        responseTable = new Hashtable();
                        responseTable["result"]=command.Parameters["arg_result"].Value;
                        responseTable["resultMsg"] = command.Parameters["arg_res_msg"].Value;

                    }
                }

            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
                responseTable = new Hashtable();
                responseTable["result"] = 0;
                responseTable["resultMsg"] = errMsg.Message;

            }
            return responseTable;

        }


        public List<SlotData> GetSlotDetailsWrtFloor(int slotFloor)
        {

            List<SlotData> slotDataList = null;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "select ps.F_LEVEL_ID,ps.AISLE, ps.F_ROW,ps.SLOT_TYPE,ps.SLOT_STATUS,ps.VALUE,ps.MANUAL_BLOCK,ps.REHANDLE,"
                                +" ps.PREV_SLOT_STATUS, LC.card_Id,LC.car_type,LC.IS_ROTATE"
                                + " FROM L2_PROC_SNAPSHOT ps left join l2_customers lc on ps.value=lc.card_id and LC.IS_RETRIEVED=0 "
                                + " where ps.F_LEVEL_ID= " + slotFloor + " and (ps.SLOT_TYPE>0 or ps.slot_type=-11)";
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            slotDataList = new List<SlotData>();
                            while (dreader.Read())
                            {

                                SlotData objSlotData = null;
                                objSlotData = new SlotData();
                                objSlotData.SlotFloor = slotFloor;
                                objSlotData.SlotAisle = int.Parse(Convert.ToString(dreader["AISLE"]));
                                objSlotData.SlotRow = int.Parse(Convert.ToString(dreader["F_ROW"]));
                                objSlotData.SlotType = int.Parse(Convert.ToString(dreader["SLOT_TYPE"]));
                                objSlotData.SlotStatus = int.Parse(Convert.ToString(dreader["SLOT_STATUS"]));
                                objSlotData.slotValue = Convert.ToString(dreader["VALUE"]);
                                objSlotData.Disable = int.Parse(Convert.ToString(dreader["MANUAL_BLOCK"])) == 1;
                                objSlotData.Rehandle = int.Parse(Convert.ToString(dreader["REHANDLE"])) == 1;
                                objSlotData.PrevSlotStatus = int.Parse(Convert.ToString(dreader["PREV_SLOT_STATUS"]));

                                if (!String.IsNullOrEmpty(Convert.ToString(dreader["card_Id"])))
                                {
                                     CarData objCarData = null;
                                    objCarData = new CarData();
                                    objCarData.CardId = Convert.ToString(dreader["card_Id"]);
                                    objCarData.IsRotated=int.Parse(Convert.ToString(dreader["IS_ROTATE"])) == 1;
                                    objCarData.CarType = int.Parse(Convert.ToString(dreader["car_type"]));
                                    objSlotData.ObjCarData = objCarData;
                                }
                                
                                slotDataList.Add(objSlotData);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return slotDataList;
        }

        public bool RegisterSlotDetailsWrtFloorNotification(int slotFloor)
        {

            bool isRegister = false;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {


                    // where condition is not valid for notification. it will trigger all the updated rows irrespective of where condition
                    string sql = FloorNotificationQuery+ " where F_LEVEL_ID= " + slotFloor;  
                    using (OracleCommand cmd = new OracleCommand(sql, con))
                    {
                        if (con.State == System.Data.ConnectionState.Closed) con.Open();
                         Connection.SlotDependency = new OracleDependency(cmd);
                        cmd.Notification.IsNotifiedOnce = false;
                         Connection.SlotDependency.OnChange += new OnChangeEventHandler(SlotUpdateNotificatonListener);
                        cmd.AddRowid = true;
                        isRegister = cmd.ExecuteNonQuery() == -1; // If oracle returns -1, then the query is successfully registered
                       
                       
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return isRegister;


        }
        public SlotData GetSlotDataUsingNotificationQuery(string query)
        {
            SlotData objSlotData = null;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = query;
                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            if (dreader.Read())
                            {
                                objSlotData = new SlotData();
                                objSlotData.SlotFloor = int.Parse(Convert.ToString(dreader["F_LEVEL_ID"]));
                                objSlotData.SlotAisle = int.Parse(Convert.ToString(dreader["AISLE"])); 
                                objSlotData.SlotRow = int.Parse(Convert.ToString(dreader["F_ROW"]));
                                objSlotData.SlotType = int.Parse(Convert.ToString(dreader["SLOT_TYPE"]));
                                objSlotData.SlotStatus = int.Parse(Convert.ToString(dreader["SLOT_STATUS"]));
                                objSlotData.slotValue = Convert.ToString(dreader["VALUE"]);
                                objSlotData.Disable = int.Parse(Convert.ToString(dreader["MANUAL_BLOCK"])) == 1;
                                objSlotData.Rehandle = int.Parse(Convert.ToString(dreader["REHANDLE"])) == 1;
                                objSlotData.PrevSlotStatus = int.Parse(Convert.ToString(dreader["PREV_SLOT_STATUS"]));
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return objSlotData;
        }
        void SlotUpdateNotificatonListener(object sender, OracleNotificationEventArgs args)
        {
            DataRow detailRow = args.Details.Rows[0];
            string rowid = detailRow["Rowid"].ToString();
            string sqlUpdateQry = FloorNotificationQuery + " where rowid = \'" + rowid + "\'";
            for (int i = 1; i < args.Details.Rows.Count; i++)
            {
                detailRow = args.Details.Rows[i];
                rowid = detailRow["Rowid"].ToString();
                sqlUpdateQry = sqlUpdateQry + " or rowid = \'" + rowid + "\'";
            }

            SlotData objSlotData = GetSlotDataUsingNotificationQuery(sqlUpdateQry);

            triggerSlotUpdate(objSlotData, new EventArgs());

        }

        public void updateCarDataToSlot(int floor, int aisle, int row, string cardId )
        {
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                string qry = "UPDATE l2_proc_snapshot SET SLOT_STATUS=2, VALUE = '" + cardId +
                    "'  where f_level_id = " + floor + " and aisle = " + aisle
                    + " and f_row = " + row;
                // Create the Select command retrieving all data from the Dept table. 
                OracleCommand selectCommand = new OracleCommand(qry, con);
                selectCommand.CommandType = CommandType.Text;
                selectCommand.ExecuteNonQuery();
            }
        }
    }
}
