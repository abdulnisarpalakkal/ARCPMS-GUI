using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARCPSGUI.OPC;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucDemoCM.xaml
    /// </summary>
    public partial class ucDemoCM : UserControl
    {
        public int machineType = 0; //1=cm,2=ps,3=vlc
        public string channel = "";
        public string machineName = "";
        public string moveCommand = "";
        public string commandDone = "";
        private bool isActive = false;
        public event EventHandler OnClick;
        public int row = 0;
        public int column = 0;
        public int floor = 0;
        ucDemoMode g_demomode = null;

        public event EventHandler OnDemoMode = null;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public ucDemoCM(ucDemoMode demo)
        {
            InitializeComponent();
            this.g_demomode = demo;
            this.g_demomode.OnStart += new EventHandler(g_demomode_OnStart);
            this.g_demomode.OnStart1 += new EventHandler(g_demomode_OnStart1);
        }

        void g_demomode_OnStart1(object sender, EventArgs e)
        {
            if (isActive)
            {

                Int16 aisleTo = 0;
                Int16 aisleFrom = 0;
                Int32 cycle = 0;
                Int32 delay = 0;
                Int16.TryParse(txtToAisle.Text, out aisleTo);
                Int16.TryParse(txtFromAisle.Text, out aisleFrom);
                Int32.TryParse(txtCycles.Text, out cycle);
                Int32.TryParse(txtDelay.Text, out delay);

                SaveDemoModeValue(machineName, aisleFrom, aisleTo, delay, cycle);

                if (machineType == 2)
                    Task.Factory.StartNew(new Action(() => PSMove(channel, machineName, aisleTo, aisleFrom, cycle, delay)));
                else if (machineType == 3)
                    Task.Factory.StartNew(new Action(() => VLCMove(channel, machineName, aisleTo, aisleFrom, cycle, delay)));
            }
        }

        void g_demomode_OnStart(object sender, EventArgs e)
        {
            if (isActive)
            {

                Int16 aisleTo = 0;
                Int16 aisleFrom = 0;
                Int32 cycle = 0;
                Int32 delay = 0;
                Int16.TryParse(txtToAisle.Text, out aisleTo);
                Int16.TryParse(txtFromAisle.Text, out aisleFrom);
                Int32.TryParse(txtCycles.Text, out cycle);
                Int32.TryParse(txtDelay.Text, out delay);

                SaveDemoModeValue(machineName, aisleFrom, aisleTo, delay, cycle);

                if(machineType == 1)
               Task.Factory.StartNew(new Action(() =>  CMMove(channel, machineName, aisleTo, aisleFrom, cycle, delay)));
               
            }
        }


        private void btnCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //int isSelectable = 0;
                bool isBlocked = true;
                bool isInDemoMode = false;

                if(machineType == 1)
                    new Connection().IsCMMachineBlocked(this.machineName, out isBlocked, out isInDemoMode);
                else if (machineType == 2)
                    new Connection().IsPSMachineBlocked(this.machineName, out isBlocked, out isInDemoMode);
                else if (machineType == 3)
                    new Connection().IsVLCMachineBlocked(this.machineName, out isBlocked, out isInDemoMode);

                if ( (isBlocked && isInDemoMode) || (!isBlocked) )
                    ChangeCMBorderColor(!isActive);
                else
                {
                    MessageBox.Show("Machine is blocked", "Infomration", MessageBoxButton.OK);
                }
            }
            finally { }
        }

        void ChangeCMBorderColor( bool acitve)
        {

            txtCycles.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                         delegate()
                         {
                             if (acitve)
                             {
                                 btnCM.Style = FindResource("cmGreenBorder") as Style;
                                 IsActive = true;
                             }
                             else
                             {
                                 btnCM.Style = FindResource("cmGrayBorder") as Style;
                                 IsActive = false;

                             }
                         }));
        }

        void CMMove(string channel, string machine, Int16 aisleTo, Int16 aisleFrom, Int32 cycle, Int32 delay)
        {
            Connection db = new Connection();
            int aisleToDb = 0;
            string demoMachine = "";
            try
            {
                OPCServerDirector opcd = new OPCServerDirector();
                Int16 row = 0;
                Int16 currentPosition = 0;
                int steps = 0;
               
                db.UpdateCMBlockedStatusForHomMove(machineName, 1, 1, 0);

                demoMachine = machine + ",T";
                if (this.OnDemoMode != null) this.OnDemoMode(demoMachine, new EventArgs());

                while (cycle > 0)
                {
                    
                    if (!isActive) break;
                    currentPosition = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle");

                    string remName = GetREMRelatedToMachine(machineName);
                    row = opcd.ReadTag<Int16>(channel + "." + remName + "." + "L2_Commanded_ROW");
                    
                    if (currentPosition !=  aisleTo)
                    {
                        db.UpdateCMBlockedStatusForHomMove(machineName, 1, 1, 0);
                        row = GetValidRowForMoving(this.floor, aisleTo);
                        opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Row", row);
                        opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle", aisleTo);
                        opcd.Write<bool>(channel + "." + machine + "." + "L2_Move_Cmd", true);
                        System.Threading.Thread.Sleep(500);
                        opcd.ReadCMCommandDoneStatus(channel , machine , "L2_CMD_DONE");
                        steps += 1;
                        aisleToDb = aisleTo;
                        db.UpdateCMBlockedStatusForHomMove(machineName, 1, 1, aisleTo);
                    }
                    else if (currentPosition != aisleFrom)
                    {
                        db.UpdateCMBlockedStatusForHomMove(machineName, 1, 1, 0);
                        row = GetValidRowForMoving(this.floor, aisleFrom);
                        opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Row", row);
                        opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle", aisleFrom);
                        opcd.Write<bool>(channel + "." + machine + "." + "L2_Move_Cmd", true);
                        System.Threading.Thread.Sleep(500);
                        opcd.ReadCMCommandDoneStatus(channel, machine, "L2_CMD_DONE");
                        steps += 1;
                        aisleToDb = aisleFrom;
                        db.UpdateCMBlockedStatusForHomMove(machineName, 1, 1, aisleFrom);
                    }

                    if (steps == 2)
                    {
                        steps = 0;
                        cycle -= 1;
                        txtCycles.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                             delegate()
                             {
                                 txtCycles.Text = cycle.ToString();
                             }));
                    }
                    System.Threading.Thread.Sleep(delay * 1000);
                }
                 ChangeCMBorderColor(false);
            }
            finally
            {
                db.UpdateCMBlockedStatusForHomMove(machineName, 0, 0, 0);
                if (this.OnDemoMode != null && !string.IsNullOrEmpty(demoMachine))
                {
                    demoMachine = machine + ",F";
                    this.OnDemoMode(demoMachine, new EventArgs());
                }
            }
        }

        void PSMove(string channel, string machine, Int16 aisleTo, Int16 aisleFrom, Int32 cycle, Int32 delay)
        {
            Connection db = new Connection();
            string demoMachine = "";
            try
            {
                OPCServerDirector opcd = new OPCServerDirector();
                Int16 row = 0;
                Int16 currentPosition = 0;
                int steps = 0;
              
                db.UpdateBlockStatusOnPalletShuttle(machine, 1);
                 demoMachine = machine + ",T";
                if (this.OnDemoMode != null) this.OnDemoMode(demoMachine, new EventArgs());

                while (cycle > 0)
                {
                    if (!isActive) break;
                    
                        currentPosition = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle");
                        if (currentPosition != aisleTo)
                        {
                            opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle", aisleTo);
                            opcd.Write<bool>(channel + "." + machine + "." + "L2_MoveCmd", true);
                            System.Threading.Thread.Sleep(500);
                            opcd.ReadCMCommandDoneStatus(channel, machine, "L2_CMD_DONE");
                            steps += 1;
                        }
                        else if (currentPosition != aisleFrom)
                        {
                            opcd.Write<Int16>(channel + "." + machine + "." + "L2_Destination_Aisle", aisleFrom);
                            opcd.Write<bool>(channel + "." + machine + "." + "L2_MoveCmd", true);
                            System.Threading.Thread.Sleep(500);
                            opcd.ReadCMCommandDoneStatus(channel, machine, "L2_CMD_DONE");
                            steps += 1;
                        }

                        if (steps == 2)
                        {
                            steps = 0;
                            cycle -= 1;
                            txtCycles.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                                 delegate()
                                 {
                                     txtCycles.Text = cycle.ToString();
                                 }));
                        }

                        System.Threading.Thread.Sleep(delay * 1000);
                    
                }
                ChangeCMBorderColor(false);
            }
            finally
            {
                db.UpdateBlockStatusOnPalletShuttle(machine, 0);
                if (this.OnDemoMode != null && !string.IsNullOrEmpty(demoMachine))
                {
                    demoMachine = machine + ",F";
                    this.OnDemoMode(demoMachine, new EventArgs());
                }
            }
        }

        void VLCMove(string channel, string machine, Int16 aisleTo, Int16 aisleFrom, Int32 cycle, Int32 delay)
        {
            Connection db = new Connection();
            string demoMachine = "";
            try
            {
                OPCServerDirector opcd = new OPCServerDirector();
                Int16 row = 0;
                Int16 currentPosition = 0;
                int steps = 0;
               
                db.UpdateVLCBlockedStatus(machine, 1);
                demoMachine = machine + ",T";
                if (this.OnDemoMode != null) this.OnDemoMode(demoMachine, new EventArgs());

                while (cycle > 0)
                {
                    if (!isActive) break;
                    currentPosition = opcd.ReadTag<Int16>(channel + "." + machine + "." + "DestFloor");
                    if ( currentPosition != aisleTo)
                    {
                        opcd.Write<Int16>(channel + "." + machine + "." + "DestFloor", aisleTo);
                        opcd.Write<bool>(channel + "." + machine + "." + "CP_Start", true);
                        System.Threading.Thread.Sleep(500);
                        opcd.ReadCMCommandDoneStatus(channel, machine, "CP_Done");
                        steps += 1;
                    }
                    else if (currentPosition != aisleFrom)
                    {
                        opcd.Write<Int16>(channel + "." + machine + "." + "DestFloor", aisleFrom);
                        opcd.Write<bool>(channel + "." + machine + "." + "CP_Start", true);
                        System.Threading.Thread.Sleep(500);
                        opcd.ReadCMCommandDoneStatus(channel, machine, "CP_Done");
                        steps += 1;
                    }

                    if (steps == 2)
                    {
                        steps = 0;
                        cycle -= 1;
                        txtCycles.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                             delegate()
                             {
                                 txtCycles.Text = cycle.ToString();
                             }));
                    }

                    System.Threading.Thread.Sleep(delay * 1000);
                }
                ChangeCMBorderColor(false);
            }
            finally
            {
                db.UpdateVLCBlockedStatus(machine, 0);
                if (this.OnDemoMode != null && !string.IsNullOrEmpty(demoMachine))
                {
                    demoMachine = machine + ",F";
                    this.OnDemoMode(demoMachine, new EventArgs());
                }
            }
        }

        void SaveDemoModeValue(string machineName,Int16 fromAisle, Int16 toAisle, int delay, int cycle)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "UPDATE L2_DEMO_MODE SET F_FROM = " + fromAisle + ", F_TO = " + toAisle
                            + ", F_CYCLE = " + cycle + ", F_DELAY =" + cycle  +"  WHERE MACHINENAME= '" + machineName + "'";
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            }
            finally
            { 
            
            }
        }

        public string GetREMRelatedToMachine(string machineCode)
        {
            string result = "";

            string sql = "";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    sql = "select DISTINCT opc1.machine from l2_opc_tag_master OPC INNER JOIN l2_opc_tag_master OPC1 ON OPC.CHANNEL = opc1.channel "
                          + " where OPC.machine ='" + machineCode + "' AND OPC1.MACHINE LIKE 'REM%'";
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
                Console.WriteLine(errmsg.Message);
            }

            return result;
        }

        Int16 GetValidRowForMoving(int floor, Int16 aisle)
        {
            Int16 validRow = 0;
                    
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    using (OracleCommand command = con.CreateCommand())
                    {
                        if (con.State == System.Data.ConnectionState.Closed) con.Open();

                        command.CommandText = "CONFIG_PACKAGE.get_valid_row_for_moving";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("floor", OracleDbType.Int32, floor, ParameterDirection.Input);
                        command.Parameters.Add("floor_aisle", OracleDbType.Int32, aisle, ParameterDirection.Input);
                        command.Parameters.Add("floor_row", OracleDbType.Int32, row, ParameterDirection.Output);
                        command.ExecuteNonQuery();
                     
                        string tmpMoveRow = "";
                        tmpMoveRow = Convert.ToString(command.Parameters["floor_row"].Value);
                        Int16.TryParse(tmpMoveRow, out validRow);
                    }

                }
            }
            finally
            { }
            return validRow;
        }

       
    }
}
