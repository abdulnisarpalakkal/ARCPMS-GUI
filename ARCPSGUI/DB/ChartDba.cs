using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace ARCPSGUI.DB
{
    class ChartDba
    {
       
        public Hashtable GetExitPeakTimeFindView(string startDate, string EndDate)
        {

            Hashtable objHashtable = new Hashtable();
            Hashtable modelHashTable = new Hashtable();
            string selectedModel = null;
            string assignedModel = null;
            DateTime tempTime = System.DateTime.Now;
            bool isSet = false;

            try
            {
                string sql = "select REQUEST_TIME,CURRENT_HOUR,MIN_INTERVEL,MAX_INTERVEL,EXIT_COUNT from EXIT_PEAK_TIME_FIND_VIEW"
               + " where REQUEST_TIME='" + startDate + "'";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))

                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {

                                    DateTime curDate = Convert.ToDateTime(Convert.ToString(reader["REQUEST_TIME"]));

                                    objHashtable.Add(new DateTime(curDate.Year, curDate.Month, curDate.Day, Convert.ToInt32(reader["CURRENT_HOUR"]), Convert.ToInt32(reader["MIN_INTERVEL"]), 0, 0)
                                            , Convert.ToInt32(Convert.ToString(reader["EXIT_COUNT"])));

                                }

                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return objHashtable;

        }
        public Hashtable GetEntryPeakTimeFindView(string startDate, string EndDate)
        {

            Hashtable objHashtable = new Hashtable();
            Hashtable modelHashTable = new Hashtable();
            string selectedModel = null;
            string assignedModel = null;
            DateTime tempTime = System.DateTime.Now;
            bool isSet = false;

            try
            {
                string sql = "select REQUEST_TIME,CURRENT_HOUR,MIN_INTERVEL,MAX_INTERVEL,ENTRY_COUNT from ENTRY_PEAK_TIME_FIND_VIEW"
               + " where REQUEST_TIME='" + startDate + "'";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))

                {
                    if (con.State == ConnectionState.Closed) con.Open();
                   
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {

                                    DateTime curDate = Convert.ToDateTime(Convert.ToString(reader["REQUEST_TIME"]));

                                    objHashtable.Add(new DateTime(curDate.Year, curDate.Month, curDate.Day, Convert.ToInt32(reader["CURRENT_HOUR"]), Convert.ToInt32(reader["MIN_INTERVEL"]), 0, 0)
                                            , Convert.ToInt32(Convert.ToString(reader["ENTRY_COUNT"])));

                                }

                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return objHashtable;

        }
        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public Hashtable getEntryParksData(string startDate, string EndDate)
        {

            Hashtable objHashtable = new Hashtable();
            DateTime tempTime = System.DateTime.Now;
            string duration = null;
            double min = 0;

            try
            {
                string sql = null;
                if (startDate.Equals(EndDate))
                {
                    sql = "select extract(MINUTE FROM CT.PUT_TO_SLOT_TIME-CT.ENTRY_TIME) duration, CT.ENTRY_TIME ENTRY_TIME"
                        + " from l2_customers CT"
                        + " where CT.ENTRY_TIME is not null and CT.PUT_TO_SLOT_TIME  is not null "
                        + " and CT.ENTRY_TIME<CT.PUT_TO_SLOT_TIME and to_char(CT.ENTRY_TIME,'DY') not in ('FRI')"
                        + " and entry_time>=DATE_TOSTARTOFDAY('" + startDate + "') and entry_time<=Date_ToEndOfDay('" + EndDate + "') ";
                }
                else
                {
                    sql = "SELECT * FROM ENTRY_PARKS_CHART where no_cars>10 and entry_time>='"
                        + startDate + "' and entry_time<='" + EndDate + "'"; // where " + @"""PARKING TIME"" IS NOT NULL AND " + @"""PARKING TIME"" !=':' order by TIME";

                }
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {

                                    DateTime.TryParse(Convert.ToString(reader["ENTRY_TIME"]), out tempTime);
                                    //tempTime=Convert.ToDateTime( tempTime.ToShortDateString());
                                    duration = Convert.ToString(reader["duration"]);
                                    //duration = duration.Split(':')[0];
                                    Double.TryParse(duration, out min);
                                    //objHashtable.Add(tempTime.ToShortDateString(), min);
                                    objHashtable.Add(tempTime, min);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return objHashtable;

        }
        public Hashtable getExitParksData(string startDate, string EndDate)
        {

            Hashtable objHashtable = new Hashtable();
            DateTime tempTime = System.DateTime.Now;
            string duration = null;
            double min = 0;

            try
            {
                string sql = null;
                if (startDate.Equals(EndDate))
                {
                    sql = "select extract(MINUTE FROM CT.EXIT_TIME-CT.GET_FROM_SLOT_TIME) duration, CT.EXIT_TIME EXIT_DATE"
                        + " from l2_customers CT"
                        + " where CT.EXIT_TIME is not null and CT.GET_FROM_SLOT_TIME  is not null "
                        + " and CT.GET_FROM_SLOT_TIME<CT.EXIT_TIME and to_char(CT.EXIT_TIME,'DY') not in ('FRI')"
                        + " and EXIT_TIME>=DATE_TOSTARTOFDAY('" + startDate + "') and EXIT_TIME<=Date_ToEndOfDay('" + EndDate + "') ";
                }
                else
                {
                    sql = "SELECT * FROM EXIT_PARKS_CHART where no_cars>10 and exit_date>='" + startDate + "' and exit_date<='" + EndDate + "'"; // where " + @"""PARKING TIME"" IS NOT NULL AND " + @"""PARKING TIME"" !=':' order by TIME";
                }

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {

                                    DateTime.TryParse(Convert.ToString(reader["EXIT_DATE"]), out tempTime);
                                    duration = Convert.ToString(reader["duration"]);
                                    //duration = duration.Split(':')[0];
                                    Double.TryParse(duration, out min);
                                    //objHashtable.Add(tempTime.ToShortDateString(), min);
                                    objHashtable.Add(tempTime, min);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return objHashtable;

        }
        public bool getAvgEntryCarAndTime(string startDate, string EndDate, out double avgCars, out double avgDuration)
        {

            string avgDurationString = null;
            string avgCarsString = null;
            avgDuration = 0;
            avgCars = 0;


            try
            {
                string sql = "select to_char(avg( no_cars ) , 'fm000') avg_cars,to_char(avg(" + @"""duration"" ) , 'fm00.00') avg_duration  from entry_parks_chart  where no_cars>10 and entry_time>='" + startDate + "' and entry_time<='" + EndDate + "'"; // where " + @"""PARKING TIME"" IS NOT NULL AND " + @"""PARKING TIME"" !=':' order by TIME";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                if (reader.Read())
                                {

                                    avgCarsString = Convert.ToString(reader["avg_cars"]);
                                    Double.TryParse(avgCarsString, out avgCars);
                                    avgDurationString = Convert.ToString(reader["avg_duration"]);
                                    Double.TryParse(avgDurationString, out avgDuration);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return true;
        }
        public bool getAvgExitCarAndTime(string startDate, string EndDate, out double avgCars, out double avgDuration)
        {

            string avgDurationString = null;
            string avgCarsString = null;
            avgDuration = 0;
            avgCars = 0;


            try
            {
                string sql = "select to_char(avg( no_cars ) , 'fm000') avg_cars,to_char(avg( (" + @"""duration"" ) ) , 'fm00.00') avg_duration from exit_parks_chart  where no_cars>10 and exit_date>='" + startDate + "' and exit_date<='" + EndDate + "'"; // where " + @"""PARKING TIME"" IS NOT NULL AND " + @"""PARKING TIME"" !=':' order by TIME";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.CommandText = sql;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                if (reader.Read())
                                {

                                    avgCarsString = Convert.ToString(reader["avg_cars"]);
                                    Double.TryParse(avgCarsString, out avgCars);
                                    avgDurationString = Convert.ToString(reader["avg_duration"]);
                                    Double.TryParse(avgDurationString, out avgDuration);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg);
            }
            return true;
        }
    }
}
