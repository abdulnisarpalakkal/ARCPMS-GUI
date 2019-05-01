using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RPMEEManageEngine;
using OPCDA.NET;
using OPC;
using System.Threading;
using OPCDA;
using System.Configuration;
using ARCPSGUI.DB;
using System.Windows;
using OPC.Common;


namespace ARCPSGUI.OPC
{
    public static class BGOPCServerManagement
    {
       public static OpcThread SrvAccess = null;
       public static BGServer bgSrv1 = null;
       

       static string opcMachineHost=null;
       static string opcServerName=null;
       static object opcConLock = new object();
       static bool connectStatus = false;


       public static BGServer GetBGOPCServer(Window owner)
        {


            if (bgSrv1 == null)
                bgSrv1 = new BGServer(owner);

            lock (opcConLock)
            {

             
                    try
                    {

                        bgSrv1.GetStatus(null, onBGSrvGetStatus);


                        if (!connectStatus)
                        {
                            if (opcMachineHost == null)
                                opcMachineHost = ConfigurationManager.AppSettings["OPCSHostMachine"];
                            if (opcServerName == null)
                                opcServerName = ConfigurationManager.AppSettings["OPCServer"];
                            Host opcHost = new Host();
                            opcHost.HostName = opcMachineHost;

                            bgSrv1.Connect(opcHost, opcServerName, null, onConnectComplete);
                          
                        }


                    }
                    catch (Exception errMsg)
                    {
                        Console.WriteLine("" + errMsg.Message);

                    }
                    finally { }

            }

            return bgSrv1;



        }
        private static void onBGSrvGetStatus(BGException ex, SERVERSTATUS stat, object tag)
        {
            connectStatus = ex == null && stat.eServerState == OpcServerState.Running;
            
        }
        private static void onConnectComplete(BGException ex, object tag)
        {
            if (ex != null)
                Console.WriteLine("Connect Error:  " + ex.Message);
            else
            {
                Console.WriteLine("connected");
            }
        }
        public static void dispose()
        {   
            if (bgSrv1 != null)
            {
                bgSrv1.Dispose();
                bgSrv1 = null;
                
            }
        }
        

       
    }

   

    
}
