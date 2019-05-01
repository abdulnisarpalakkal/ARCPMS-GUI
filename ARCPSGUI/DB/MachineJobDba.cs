using ARCPSGUI.Model;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.DB
{
    class MachineJobDba
    {
        public List<MachineJobData> GetMachineJobsWrtFloor(int slotFloor)
        {

            List<MachineJobData> MachineJobDataList = null;

            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    string sql = "SELECT p.machine_name machine, "
                    +" CASE "
                    +" WHEN EQ.IS_ENTRY = 1"
                    +" THEN 'ENTRY'"
                    +" WHEN EQ.IS_ENTRY = 0"
                    +" THEN 'EXIT'"
                    +" WHEN EQ.IS_ENTRY =5 "
                    +" THEN 'TRANSFER'" 
                    +" WHEN EQ.IS_ENTRY =6 "
                    +" THEN 'REHANDLE' "
                    +" ELSE 'WASH'"
                    +" END P_MODE,"
                    +" p.floor,"
                    +" p.aisle,"
                    +" p.f_row,"
                    +" eq.TRANS_STATUS STATUS"
                    +" FROM l2_path_details P"
                    +" LEFT JOIN l2_ees_queue eq ON P.queue_id = eq.ID "
                    +" LEFT JOIN L2_SLOT_PATH SP ON P.queue_id = SP.PATH_ID "
                    + " WHERE p.DONE =2 and  p.floor=" + slotFloor;

                    OracleCommand selectCommand = new OracleCommand(sql, con);
                    using (OracleDataReader dreader = selectCommand.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            MachineJobDataList = new List<MachineJobData>();
                            while (dreader.Read())
                            {

                                MachineJobData objMachineJobData = null;
                                objMachineJobData = new MachineJobData();
                                objMachineJobData.Machine = Convert.ToString(dreader["machine"]);
                                objMachineJobData.Mode= Convert.ToString(dreader["P_MODE"]);
                                objMachineJobData.Floor = int.Parse(Convert.ToString(dreader["floor"]));
                                objMachineJobData.Aisle = int.Parse(Convert.ToString(dreader["aisle"]));
                                objMachineJobData.Row = int.Parse(Convert.ToString(dreader["f_row"]));
                                objMachineJobData.Status = Convert.ToString(dreader["STATUS"]);

                                MachineJobDataList.Add(objMachineJobData);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return MachineJobDataList;
        }

    }
}
