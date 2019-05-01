using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;
namespace ARCPSGUI.Security
{

   public class Security
    {
       public string userName = "";
       public string password = "";
       public string accessKey = "";
       public bool read = false;
       public bool write = false;
       public int userId = 0;

       public static int currentUserId = 0;

       public static List<Security> lstSecuredItem = new List<Security>(2);
       public void LoadUserSecuredItems(int userId)
       {
           try
           {
               string qry = "";

               using (OracleConnection con = new OracleConnection( Connection.connectionString))
               {
                   if (con.State == System.Data.ConnectionState.Closed) con.Open();
                   using (OracleCommand command = con.CreateCommand())
                   {
                       qry = "SELECT ACCESS_KEY,V_READ,V_WRITE FROM L2_ACCESSRIGHT_GUI WHERE USER_ID = " + userId;
                       command.CommandText = qry;
                       using (OracleDataReader dreader = command.ExecuteReader())
                       {
                           Security securieditem = null;
                           if (dreader.HasRows)
                           {
                               while (dreader.Read())
                               {
                                   securieditem = new Security();
                                   securieditem.userId = userId;
                                   securieditem.accessKey = Convert.ToString(dreader["ACCESS_KEY"]);
                                   securieditem.read = Convert.ToInt32(dreader["V_READ"]) == 1 ? true : false;
                                   securieditem.write = Convert.ToInt32(dreader["V_WRITE"]) == 1 ? true : false;
                                   lstSecuredItem.Add(securieditem);
                               }
                           }
                           else
                           {
                               lstSecuredItem.Clear();
                               
                           }
                       }
                   }
               }
           }
           finally
           {

           }
       }
           
       public static ARCPSGUI.StaticGlobalClass.GlobalData.SecurityAccess HasGotAccessRight(string accessKey)
       {
           ARCPSGUI.StaticGlobalClass.GlobalData.SecurityAccess secAccess = StaticGlobalClass.GlobalData.SecurityAccess.Unknown;
           try
           {
               Security item = null;
               //Security item = (Security)lstSecuredItem.Where(r => r.accessKey == accessKey).First();
               var result = lstSecuredItem.Where(r => r.accessKey == accessKey);
               if (result.Count() > 0)
                   item = result.First();
                 
                   if ( item != null && (item.read || item.write))
                       secAccess = StaticGlobalClass.GlobalData.SecurityAccess.Allow;
                   else
                       secAccess = StaticGlobalClass.GlobalData.SecurityAccess.Disallow;
           }
           finally
           {

           }
           return secAccess;
       }

       public static ARCPSGUI.StaticGlobalClass.GlobalData.SecurityType HasReadAccess(string accessKey)
       {
           ARCPSGUI.StaticGlobalClass.GlobalData.SecurityType secType = StaticGlobalClass.GlobalData.SecurityType.Unknown;
           try
           {
               Security item = null;
               var result = lstSecuredItem.Where(r => r.accessKey == accessKey);

               if (result.Count() > 0)
                   item = result.First();
            
                if (item != null && item.read)
                   secType = StaticGlobalClass.GlobalData.SecurityType.Read;
           }
           finally
           {

           }
           return secType;
       }

       public static ARCPSGUI.StaticGlobalClass.GlobalData.SecurityType HasWriteAccess(string accessKey)
       {
           ARCPSGUI.StaticGlobalClass.GlobalData.SecurityType secType = StaticGlobalClass.GlobalData.SecurityType.Unknown;
           try
           {
               Security item = null;
               var result = lstSecuredItem.Where(r => r.accessKey == accessKey);

               if (result.Count() > 0)
                   item = result.First();

               if (item != null && item.write)
                   secType = StaticGlobalClass.GlobalData.SecurityType.Write;
           }
           finally
           {

           }
           return secType;
       }

       public static int GetUserId(string userName, string password)
       {
           try
           {
               string qry = "";
               if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
               {
                   using (OracleConnection con = new OracleConnection( Connection.connectionString))
                   {
                       if (con.State == System.Data.ConnectionState.Closed) con.Open();
                       using (OracleCommand command = con.CreateCommand())
                       {
                           qry = "SELECT V_USERID FROM L2_AUTHENTICATION WHERE V_USERNAME = '"
                               + userName + "' AND V_PASSWORD = '" + password + "'";
                           command.CommandText = qry;
                           using (OracleDataReader dreader = command.ExecuteReader())
                           {
                               if (dreader.HasRows)
                               {
                                   if (dreader.Read())
                                       int.TryParse(Convert.ToString(dreader["V_USERID"]), out currentUserId);

                                   if (currentUserId > 0)
                                       new Security().LoadUserSecuredItems(currentUserId);
                               }
                               
                           }
                       }
                       con.Close();
                   }
               }
           }
           finally
           { }
           return currentUserId;
       }

       public static string GetUserName(int userId)
       {
           string userName = "";
           try
           {
               string qry = "";
               if (userId > 0)
               {
                   using (OracleConnection con = new OracleConnection( Connection.connectionString))
                   {
                       if (con.State == System.Data.ConnectionState.Closed) con.Open();
                       using (OracleCommand command = con.CreateCommand())
                       {
                           qry = "SELECT V_USERNAME FROM L2_AUTHENTICATION WHERE V_USERID = " + userId;
                               
                           command.CommandText = qry;
                           using (OracleDataReader dreader = command.ExecuteReader())
                           {
                               if (dreader.HasRows)
                               {
                                   if (dreader.Read())     userName = Convert.ToString(dreader["V_USERNAME"]);
                               }
                           }
                       }
                       con.Close();
                   }
               }
           }
           finally
           { }
           return userName;
       
       }


       public static int CheckAuthentication(string userName, string password)
       {
           try
           {
               string qry = "";
               if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
               {
                   using (OracleConnection con = new OracleConnection( Connection.connectionString))
                   {
                       if (con.State == System.Data.ConnectionState.Closed) con.Open();
                       using (OracleCommand command = con.CreateCommand())
                       {
                           qry = "SELECT V_USERID FROM L2_AUTHENTICATION WHERE V_USERNAME = '"
                               + userName + "' AND V_PASSWORD = '" + password + "'";
                           command.CommandText = qry;
                           using (OracleDataReader dreader = command.ExecuteReader())
                           {
                               if (dreader.HasRows)
                               {
                                   if (dreader.Read())
                                       int.TryParse(Convert.ToString(dreader["V_USERID"]), out currentUserId);

                                   if (currentUserId > 0)
                                       new Security().LoadUserSecuredItems(currentUserId);
                               }

                           }
                       }
                   }
               }
           }
           finally
           { }
           return currentUserId;
       }

    }
}
