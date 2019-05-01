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


namespace ARCPSGUI.OPC
{
    public static class OPCServerManagement
    {
        //  OPCTagMasterTransaction opcTagMaster = null;
       public static OpcThread SrvAccess = null;
       public static OpcServer opcServer = null;
       public static int rtc;

       static string opcMachineHost;
       static string opcServerName;
       static object opcConLock = new object();
       
        public static bool StartOPCServer(out string errorMessage)
        {
            int rtc = 0;
            errorMessage = "";
            try
            {
                opcServer = new OpcServer();



                //opcMachineHost = ConfigurationManager.AppSettings["OPCSHostMachine"];
                //opcServerName = ConfigurationManager.AppSettings["OPCServer"];

                Connection dbpm = new Connection();
                //for main l2 server
                //opcMachineHost = dbpm.GetConfigValue("HostServer", "", "Name");
                //opcServerName = dbpm.GetConfigValue("OpcServerName", "", "Name");
                //opcMachineHost = "NISAR";
                //opcServerName = "Matrikon.OPC.Simulation.1";
                //for working machine
                opcMachineHost = ConfigurationManager.AppSettings["OPCSHostMachine"];
                opcServerName = ConfigurationManager.AppSettings["OPCServer"];

                if (opcServer.isConnectedDA == false)
                    rtc = opcServer.Connect(opcMachineHost, opcServerName);

                SrvAccess = new OpcThread(opcServer);
            }
            catch (Exception errMsg)
            {
                errorMessage = errMsg.Message;
                throw errMsg;
            }
            return HRESULTS.Succeeded(rtc);
        }
        //public static OpcServer GetOPCServerConnection()
        //{


        //    if (opcServer == null) opcServer = new OpcServer();

        //    int rtc = 0;
        //    SERVERSTATUS objSERVERSTATUS = new SERVERSTATUS();
        //    bool isConnected = false;
        //    bool isServerRunning = true;

        //    lock (opcConLock)
        //    {

        //        //do
        //        //{
        //            try
        //            {
        //                isConnected = opcServer.isConnectedDA;
        //                if (isConnected)
        //                {
        //                    opcServer.GetStatus(out objSERVERSTATUS);
        //                    isServerRunning = objSERVERSTATUS.eServerState == OpcServerState.Running;

        //                }


        //                if (!isConnected || !isServerRunning)
        //                {
        //                    Connection dbpm = new Connection();
        //                    //opcMachineHost = GlobalValues.OPC_MACHINE_HOST;
        //                    //opcServerName = GlobalValues.OPC_SERVER_NAME;
        //                    //opcMachineHost = dbpm.GetConfigValue("HostServer", "", "Name");
        //                    //opcServerName = dbpm.GetConfigValue("OpcServerName", "", "Name");
        //                    opcMachineHost = ConfigurationManager.AppSettings["OPCSHostMachine"];
        //                    opcServerName = ConfigurationManager.AppSettings["OPCServer"];
        //                    //opcMachineHost = "NISAR";
        //                    //opcServerName = "Matrikon.OPC.Simulation.1";
        //                    rtc = opcServer.Connect(opcMachineHost, opcServerName);
        //                    //if (!isServerRunning && IsOPCServerIsRunning())
        //                    //    new InitializeEngine().AsynchReadSettings();

        //                }


        //            }
        //            catch (Exception errMsg)
        //            {
        //                Console.WriteLine("" + errMsg.Message);

        //            }
        //            finally { }

        //        //} while (opcServer.isConnectedDA == false);
        //    }



        //    return opcServer;



        //}
        //static bool IsOPCServerIsRunning()
        //{
        //    bool isRunning = false;
        //    SERVERSTATUS objSERVERSTATUS = new SERVERSTATUS();
        //    opcServer.GetStatus(out objSERVERSTATUS);
        //    isRunning = objSERVERSTATUS.eServerState == OpcServerState.Running;
        //    return isRunning;
        //}
        //public static T ReadTag<T>(string command)
        //{
        //    StartOPcServerIfStopped();

        //    T result = default(T);

        //    ItemProperties[] item = null;


        //    //get value of command.
        //    item = opcServer.GetProperties(new string[1] { command }, true, new int[] { 2 });

        //    bool bOk = false;

        //    bOk = item != null && item.Length > 0;
        //    bOk &= bOk &&
        //              item[0].Properties != null && item[0].Properties.Length > 0;

        //    if (item[0].Properties == null) return result;
        //    Property property = item[0].Properties[0];

        //    if (property.Value != null)
        //        result = (T)property.Value;

        //    return result;

        //}

    

        //public static void ReIssueCommand(string command)
        //{
        //    StartOPcServerIfStopped();
        //    SrvAccess.Request(new OpcRequest(Command.Write, command, (object)true));
        //}

        static void StartOPcServerIfStopped()
        {

          
            int rtc = 0;

            if (opcServer == null)
            {
              
                opcServer = new OpcServer();
            }

            if (SrvAccess == null)
            {
                
                SrvAccess = new OpcThread(opcServer);
            }

            if (opcServer.isConnectedDA == false)
            {
               
                rtc = opcServer.Connect("HPIServer", "Intellution.IntellutionGatewayOPCServer");
            }

        }
        public static void dispose()
        {
            if (opcServer != null)
            {
                opcServer.Disconnect();
                opcServer = null;

            }
        }
    }

    public class OPCServerDirector
    {
        public  OpcThread SrvAccess = null;
        public  OpcServer opcServer = null;
     
        void StartOPcServerIfStopped()
        {

         
            int rtc = 0;

            if (opcServer == null)
            {
              //   rptEngine.Notification("FROM GUI : TRY TO RECONNECT OPC SERVER 1");
                opcServer = new OpcServer();
            }

            if (SrvAccess == null)
            {
              //  rptEngine.Notification("FROM GUI : TRY TO RECONNECT OPC SERVER 2");
                SrvAccess = new OpcThread(opcServer);
            }

            if (opcServer.isConnectedDA == false)
            {
               
                rtc = opcServer.Connect("HPIServer", "Intellution.IntellutionGatewayOPCServer");
            }
        }

        public OPCServerDirector()
        {
            SrvAccess = OPCServerManagement.SrvAccess;
            opcServer = OPCServerManagement.opcServer;
        }

        public T ReadTag<T>(string command)
        {

            StartOPcServerIfStopped();
            T result = default(T);
           
            ItemProperties[] item = null;
            //get value of command.
            item = opcServer.GetProperties(new string[1] { command }, true, new int[] { 2 });

            bool bOk = false;

            bOk = item != null && item.Length > 0;
            bOk &= bOk &&
                      item[0].Properties != null && item[0].Properties.Length > 0;

            if (item[0].Properties == null) return result;
            Property property = item[0].Properties[0];

            if (property.Value != null)
                result = (T)property.Value;

            return result;

        }

        public bool IsMachineHealthy(string command)
        {

            bool result = true;

            ItemProperties[] item = null;
            //get value of command.
            item = opcServer.GetProperties(new string[1] { command }, true, new int[] { 2 });

            bool bOk = false;

            bOk = item != null && item.Length >  0;
            bOk &= bOk && item[0].Properties != null && item[0].Properties.Length > 0;

            if (bOk)
            {
                Property property = item[0].Properties[0];
                result = property.Value != null;
            }
            else
                result = false;
            
            return result;

        }

        public qualityBits IsMachineQualityHealthy(string command)
        {
            qualityBits machineQuality = qualityBits.bad;
          
            try
            {
               
                ItemProperties[] item = null;

                //get value of command.
                item = opcServer.GetProperties(new string[1] { command }, true, new int[] { 3 });
                Property property = item[0].Properties[0];
                machineQuality = ((OPCDA.OPCQuality)property.Value).QualityField;
            }
            catch (Exception errMsg)
            {
                //rptEngine.Alarm_OPC_NotificatonMessage(command, "Quality is " + machineQuality.ToString()
                //     + " Message = " + errMsg.Message);
            }
            return machineQuality;
        }

        public bool Write<T>(string instruction, T value)
        {
            StartOPcServerIfStopped();
            bool bOk = false;
            OpcThread opcthread = new OpcThread(opcServer);

            try
            {               
               opcthread.Request(new OpcRequest(Command.Write, instruction, value));
            }
            catch (Exception errMsg)
            {
                bOk = false;
            }
            finally
            {
            }
            return bOk;
        }

        public bool IsCMMachineInAutoMode(string channel, string machine, string machineType)
        {
            bool isAutoMode = false;

            try
            {
                isAutoMode = ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");

                if (machineType.Contains("UCM"))
                    isAutoMode &= ReadTag<bool>(channel + "." + machine + "." + "L2_AUTO_READY");
                else if (machineType.Contains("VLC"))
                    isAutoMode &= ReadTag<bool>(channel + "." + machine + "." + "Auto_Ready");
            }

            catch (Exception errMsg)
            {

            }
            return isAutoMode;
        }
        
        public bool ReadCMCommandDoneStatus(string channel, string machine, string command)
        {
            bool result = false;
            bool isWaitingForCmdDoneOn = false;
            int counter = 1;
            try
            {
                Thread.Sleep(2000);
                result = false;
                TimeSpan startTime = System.DateTime.Now.TimeOfDay;

                while (result == false)
                {
                    result = ReadTag<bool>(channel + "." + machine + "." + command);

                    if (counter > 3) Thread.Sleep(1000);
                    counter += 1;
                }
            }
            catch (Exception errMsg)
            {
                //  Console.WriteLine(errMsg.Message, command);
            }
            finally
            {
            }
            return result;
        }
    }

    public class CamOpcServer
    {
        OpcServer camOPCServer = null;
        int rtc = 0;


        public CamOpcServer()
        {
            camOPCServer = new OpcServer();

            if (camOPCServer.isConnectedDA == false) rtc = camOPCServer.Connect("localhost", "CameraOPCSvr");

        }
        public bool TakePhoto(string machineName, string tagName, bool value)
        {

            bool bOk = false;
            try
            {
                bOk = PhotoWrite<bool>(machineName + "." + tagName, value);
                System.Threading.Thread.Sleep(500);

            }
            catch (Exception errMsg)
            {
                bOk = false;
            }
            return bOk;
        }

        bool PhotoWrite<T>(string instruction, T value)
        {

            bool bOk = false;
            // bOk = IsValidTag(instruction);
            OpcThread opcthread = new OpcThread(camOPCServer);

            try
            {

                opcthread.Request(new OpcRequest(Command.Write, instruction, value));

            }
            catch (Exception errMsg)
            {
                bOk = false;

            }
            finally
            {
                opcthread.Stop();
            }
            return bOk;
        }

        public T PhotoReadTag<T>(string command)
        {

            T result = default(T);
            int counter = 1;
            bool loopValue = true;
            while (loopValue == true)
            {
                if (counter == 20)
                {
                    loopValue = false;
                }
                counter += 1;

                try
                {

                    ItemProperties[] item = null;
                    item = camOPCServer.GetProperties(new string[1] { command }, true, new int[] { 2 });
                    Property property = item[0].Properties[0];
                    result = (T)property.Value;
                }

                catch (Exception errMsg)
                {

                }
                finally
                {

                }

            }
            return result;

        }

        public string GetImagePathAndName(string machineName, string tagName)
        {
            // Int16 errorCode = 0;
            string imagePathAndName = "";
            try
            {
                imagePathAndName = PhotoReadTag<string>(machineName + "." + tagName);
                System.Threading.Thread.Sleep(500);

            }
            catch (Exception errMsg)
            {

            }
            return imagePathAndName;
        }

    }
}
