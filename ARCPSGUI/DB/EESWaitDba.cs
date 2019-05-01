using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ARCPSGUI.Model;
using Oracle.DataAccess.Client;

namespace ARCPSGUI.DB
{
    class EESWaitDba
    {
        public List<EESWaitData> GetEESWaitList(EESWaitData objEESWaitDataForSearch)
        {
            List<EESWaitData> waitList = null;
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "select rownum sno,WAIT_HIST_ID,WAIT_GATE,WAIT_LOCATION_ID,LOCATION,WAIT_TIME,WAIT_UPDATE_TIME"
                        +" from "
                        +" ("
                        + " select WAIT_HIST_ID,WAIT_GATE,WAIT_LOCATION_ID,(select wait_location from wait_car_location_master where loc_id=WAIT_LOCATION_ID) LOCATION,"
                        +" WAIT_TIME,WAIT_UPDATE_TIME"
                        +" from WAIT_CAR_FLOW_EES_HIST order by WAIT_UPDATE_TIME desc"
                        + " ) where TRUNC(WAIT_UPDATE_TIME)=:WaitUpdateTime ";
                        if (!string.IsNullOrEmpty(objEESWaitDataForSearch.WaitGate))
                        {
                            sql += " and WAIT_GATE=:WaitGate";
                        }
                        if (objEESWaitDataForSearch.WaitLocationId != 0)
                        {
                            sql += " and WAIT_LOCATION_ID=:WaitLocationId";
                        }
                        command.BindByName = true;
                        command.Parameters.Add("WaitUpdateTime", objEESWaitDataForSearch.WaitUpdateTime.ToString("dd-MMM-yyyy"));
                        if (!string.IsNullOrEmpty(objEESWaitDataForSearch.WaitGate))
                        {
                            command.Parameters.Add("WaitGate", objEESWaitDataForSearch.WaitGate);
                        }
                        if (objEESWaitDataForSearch.WaitLocationId != 0)
                        {
                            command.Parameters.Add("WaitLocationId", objEESWaitDataForSearch.WaitLocationId);
                        }

                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                waitList = new List<EESWaitData>();

                                while (reader.Read())
                                {

                                    EESWaitData objEESWaitData = new EESWaitData();

                                    objEESWaitData.sno = Int32.Parse(reader["sno"].ToString());
                                    objEESWaitData.WaitHistID = Int32.Parse(reader["WAIT_HIST_ID"].ToString());
                                    objEESWaitData.WaitGate = reader["WAIT_GATE"].ToString();

                                    DateTime tempDate = new DateTime();
                                    DateTime.TryParse(reader["WAIT_UPDATE_TIME"].ToString(), out tempDate);
                                    objEESWaitData.WaitUpdateTime = tempDate;

                                    objEESWaitData.WaitLocation = reader["LOCATION"].ToString();
                                    objEESWaitData.WaitTime = reader["WAIT_TIME"].ToString();


                                    waitList.Add(objEESWaitData);
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
            return waitList;
        }
    }
}
