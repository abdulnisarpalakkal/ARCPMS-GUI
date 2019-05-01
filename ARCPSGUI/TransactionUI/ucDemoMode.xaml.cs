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
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucDemoMode.xaml
    /// </summary>
    public partial class ucDemoMode : UserControl
    {

        public event EventHandler OnStart;
        public event EventHandler OnStart1;

        public event EventHandler OnDemoMode = null; //notifiy to frmMain, to create notificaiton.
        public ucDemoMode()
        {
            InitializeComponent();
            CreateLCM();
            CreateUCM();
            CreadPS();
            CreateVLC();
        }

        void CreateLCM()
        {
            int row = 12;
            string channel = "";
            string machine = "";

            for (int column = 2; column <= 12; column++)
            {
                if ((column % 2) == 0)
                {
                    Viewbox vwChild = new Viewbox();
                    vwChild.Stretch = Stretch.Fill;
                    vwChild.Tag = 12;
                    ucDemoCM demoCM = new ucDemoCM(this);
                    vwChild.Child = demoCM;
                    demoCM.lblCMNumber.Content = GetMachineNumber(row, column);
                    vwChild.Name = "LCM" + row + column;
                    GetMachineNameAndChannel(row, column, out channel, out machine);
                    demoCM.channel = channel;
                    demoCM.machineName = machine;
                    demoCM.Name = "LCM" + row + column;
                    demoCM.machineType = 1;
                    demoCM.floor = 4;

                    Int16 aisleFrom = 0;
                    Int16 aisleTo = 0;
                    int delay = 0;
                    int cycle = 0;

                    GetDemoModeValue(demoCM.machineName, out aisleFrom, out aisleTo, out delay, out cycle);

                    demoCM.txtFromAisle.Text = Convert.ToString(aisleFrom);
                    demoCM.txtToAisle.Text = Convert.ToString(aisleTo);
                    demoCM.txtDelay.Text = Convert.ToString(delay);
                    demoCM.txtCycles.Text = Convert.ToString(cycle);

                    Grid.SetColumn(vwChild, column);
                    Grid.SetRow(vwChild, row);
                    grdMachineHolder.Children.Add(vwChild);

                    demoCM.OnDemoMode += (s, e) => {
                        if (OnDemoMode != null) OnDemoMode(s, e);
                    };

                }
            }
        }

        void CreateUCM()
        {
            string channel = "";
            string machine = "";
            for (int row = 2; row <= 18; row++)
            {
                if (row == 12) continue;
                if ((row % 2) == 0)
                {
                    for (int column = 2; column <= 18; column++)
                    {
                        if (column == 2 || column == 6 || column == 10)
                        {
                            Viewbox vwChild = new Viewbox();
                            vwChild.Stretch = Stretch.Fill;
                            vwChild.Tag = row;
                            vwChild.Name = "UCM" + row + column;
                            ucDemoCM demoCM = new ucDemoCM(this);
                            demoCM.Tag = row;
                            GetMachineNameAndChannel(row, column, out channel, out machine);
                            demoCM.channel = channel;
                            demoCM.machineName = machine;
                            demoCM.row = row;
                            demoCM.column = column;
                            demoCM.floor = GetFloor(row);
                            demoCM.machineType = 1;
                            demoCM.lblCMNumber.Content = GetMachineNumber(row, column);
                            demoCM.Name = "UCM" + row + column;
                            vwChild.Child = demoCM;
                            
                            Int16 aisleFrom = 0;
                            Int16 aisleTo = 0;
                            int delay = 0;
                            int cycle = 0;

                            GetDemoModeValue(demoCM.machineName, out aisleFrom, out aisleTo, out delay, out cycle);

                            demoCM.txtFromAisle.Text = Convert.ToString(aisleFrom);
                            demoCM.txtToAisle.Text = Convert.ToString(aisleTo);
                            demoCM.txtDelay.Text = Convert.ToString(delay);
                            demoCM.txtCycles.Text = Convert.ToString(cycle);

                            demoCM.OnClick += new EventHandler(demoCM_OnClick);
                            Grid.SetColumn(vwChild, column);
                            Grid.SetRow(vwChild, row);
                            grdMachineHolder.Children.Add(vwChild);

                            demoCM.OnDemoMode += (s, e) =>
                            {
                                if (OnDemoMode != null) OnDemoMode(s, e);
                            };
                        }
                    }
                }
            }
        }

        void CreadPS()
        {
            int row = 1;
            string channel = "";
            string machine = "";
            int counter = 1;
            for (int column = 1; column <= 8; column++)
            {
                if (column == 1 || column == 3 || column == 5 || column== 7)
                {
                    Viewbox vwChild = new Viewbox();
                    vwChild.Stretch = Stretch.Fill;
                    vwChild.Tag = 4;
                    ucDemoCM demoCM = new ucDemoCM(this);
                    vwChild.Child = demoCM;
                    demoCM.lblCMNumber.Content =  counter;
                    vwChild.Name = "PS" + row + column;
                    demoCM.channel = "CH45" + counter;
                    demoCM.machineName = "PS_FLR4_0" + counter;
                    demoCM.Name = "PS" + row + column;
                    demoCM.machineType = 2;
                   
                    Int16 aisleFrom = 0;
                    Int16 aisleTo = 0;
                    int delay = 0;
                    int cycle = 0;

                    GetDemoModeValue(demoCM.machineName, out aisleFrom, out aisleTo, out delay, out cycle);

                    demoCM.txtFromAisle.Text = Convert.ToString(aisleFrom);
                    demoCM.txtToAisle.Text = Convert.ToString(aisleTo);
                    demoCM.txtDelay.Text = Convert.ToString(delay);
                    demoCM.txtCycles.Text = Convert.ToString(cycle);

                    Grid.SetColumn(vwChild, column);
                    Grid.SetRow(vwChild, row);
                    grd2.Children.Add(vwChild);
                    counter += 1;

                    demoCM.OnDemoMode += (s, e) =>
                    {
                        if (OnDemoMode != null) OnDemoMode(s, e);
                    };
                }
            }
        }

        void CreateVLC()
        {
         
            int counter = 1;
            for(int row = 3; row <= 5; row ++)
            {
                if (row == 4) continue;
                for (int column = 1; column <= 6; column++)
                {
                    if (column == 1 || column == 3 || column ==5)
                    {
                        Viewbox vwChild = new Viewbox();
                        vwChild.Stretch = Stretch.Fill;
                        vwChild.Tag = 4;
                        ucDemoCM demoCM = new ucDemoCM(this);
                        vwChild.Child = demoCM;
                        demoCM.lblCMNumber.Content =  counter;
                        vwChild.Name = "VLC" + row + column;
                        demoCM.channel = "CH00" + counter;
                        demoCM.machineType = 3;
                        demoCM.machineName = "VLC_Drive_0" + counter;
                        demoCM.Name = "VLC" + row + column;
                        Grid.SetColumn(vwChild, column);
                        Grid.SetRow(vwChild, row);
                        Int16 aisleFrom = 0;
                        Int16 aisleTo = 0;
                        int delay = 0;
                        int cycle = 0;

                        GetDemoModeValue(demoCM.machineName, out aisleFrom, out aisleTo, out delay, out cycle);

                        demoCM.txtFromAisle.Text = Convert.ToString(aisleFrom);
                        demoCM.txtToAisle.Text = Convert.ToString(aisleTo);
                        demoCM.txtDelay.Text = Convert.ToString(delay);
                        demoCM.txtCycles.Text = Convert.ToString(cycle);

                        grd2.Children.Add(vwChild);
                        counter += 1;
                        demoCM.OnDemoMode += (s, e) =>
                        {
                            if (OnDemoMode != null) OnDemoMode(s, e);
                        };
                    }
                }
            }
        }

        void demoCM_OnClick(object sender, EventArgs e)
        {
            string[] val = Convert.ToString(sender).Split(',');
            //int row = 0;
            //int column = 0;
            string floor = "";
            bool isActive = false;
            if (val != null && val.Length == 2)
            {
               // int.TryParse(val[0], out floor);
                floor = Convert.ToString(val[0]);
                bool.TryParse(val[1], out isActive);
            }

            ChaneCMBorderColor( floor, isActive);
        }


        int GetMachineNumber(int row, int col)
        {
           int cmNumber = 0;

            try
            {
                if (row == 2)//9
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 4)//8
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 6)//7
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 8)//6
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 10)//5
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 12)//4
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 4) // 4
                        cmNumber = 2;
                    else if (col == 6) // 6
                        cmNumber = 3;
                    else if (col == 8) // 8
                        cmNumber = 4;
                    else if (col == 10) // 10
                        cmNumber = 5;
                    else if (col == 12) // 12
                        cmNumber = 6;
                }
                else if (row == 14)//3
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 16)//2
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }
                else if (row == 18)//1
                {
                    if (col == 2) // 2
                        cmNumber = 1;
                    else if (col == 6) // 4
                        cmNumber = 2;
                    else if (col == 10) // 6
                        cmNumber = 3;
                }

            }
            finally
            { 
            
            }

            return cmNumber;
        }

        int GetFloor(int row)
        {
            int floor = 0;
            switch (row)
            { 
                case 2:
                    floor = 9;
                    break;
                case 4:
                    floor = 8;
                    break;
                case 6:
                    floor = 7;
                    break;

                case 8:
                    floor = 6;
                    break;
                case 10:
                    floor = 5;
                    break;
                case 12:
                    floor = 4;
                    break;

                case 14:
                    floor = 3;
                    break;
                case 16:
                    floor = 2;
                    break;
                case 18:
                    floor = 1;
                    break;

                default:
                    break;
            }
            return floor;
        }

        void GetMachineNameAndChannel(int row, int col,out string channel, out string machine)
        {
            channel = "";
            machine = "";
            try
            {
                if (row == 2)//9
                {
                    if (col == 2) // 1
                    {
                        channel = "CH931";
                        machine = "UCM_FLR09_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH932";
                        machine = "UCM_FLR09_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH933";
                        machine = "UCM_FLR09_03";
                    }
                }
                else if (row == 4)//8
                {
                    if (col == 2) //1
                    {
                        channel = "CH831";
                        machine = "UCM_FLR08_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH832";
                        machine = "UCM_FLR08_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH833";
                        machine = "UCM_FLR08_03";
                    }
                }
                else if (row == 6)//7
                {
                    if (col == 2) // 1
                    {
                        channel = "CH731";
                        machine = "UCM_FLR7_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH732";
                        machine = "UCM_FLR7_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH733";
                        machine = "UCM_FLR7_03";
                    }
                }
                else if (row == 8)//6
                {
                    if (col == 2) // 1
                    {
                        channel = "CH631";
                        machine = "UCM_FLR6_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH632";
                        machine = "UCM_FLR6_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH633";
                        machine = "UCM_FLR6_03";
                    }
                }
                else if (row == 10)//5
                {
                    if (col == 2) // 1
                    {
                        channel = "CH531";
                        machine = "UCM_FLR5_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH532";
                        machine = "UCM_FLR5_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH533";
                        machine = "UCM_FLR5_03";
                    }
                }
                else if (row == 12)//4
                {
                    if (col == 2) // 1
                    {
                        channel = "CH441";
                        machine = "LCM_FLR04_01";
                    }
                    else if (col == 4) // 2
                    {
                        channel = "CH442";
                        machine = "LCM_FLR04_02";
                    }
                    else if (col == 6) // 3
                    {
                        channel = "CH443";
                        machine = "LCM_FLR4_03";
                    }
                    else if (col == 8) // 4
                    {
                        channel = "CH444";
                        machine = "LCM_FLR4_04";
                    }
                    else if (col == 10) // 5
                    {
                        channel = "CH445";
                        machine = "LCM_FLR4_05";
                    }
                    else if (col == 12) // 6
                    {
                        channel = "CH446";
                        machine = "LCM_FLR4_06";
                    }
                }
                else if (row == 14)//3
                {
                    if (col == 2) // 1
                    {
                        channel = "CH331";
                        machine = "UCM_FLR03_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH332";
                        machine = "UCM_FLR03_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH333";
                        machine = "UCM_FLR03_03";
                    }
                }
                else if (row == 16)//2
                {
                    if (col == 2) // 1
                    {
                        channel = "CH231";
                        machine = "UCM_FLR02_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH232";
                        machine = "UCM_FLR02_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH233";
                        machine = "UCM_FLR02_03";
                    }
                }
                else if (row == 18)//1
                {
                    if (col == 2) // 1
                    {
                        channel = "CH131";
                        machine = "UCM_FLR01_01";
                    }
                    else if (col == 6) // 2
                    {
                        channel = "CH132";
                        machine = "UCM_FLR01_02";
                    }
                    else if (col == 10) // 3
                    {
                        channel = "CH133";
                        machine = "UCM_FLR01_03";
                    }
                }

            }
            finally
            {

            }
        }
   
        void ChaneCMBorderColor(string floor, bool acitve)
        {

            var result = grdMachineHolder.Children.OfType<Viewbox>().Where(r => Convert.ToString(r.Tag) == floor);
            foreach (Viewbox vb in result)
            {
                ucDemoCM machine = vb.Child as ucDemoCM;
                if (machine != null)
                {
                    if (acitve)
                    {
                        machine.btnCM.Style = FindResource("cmGreenBorder") as Style;
                        machine.IsActive = true;
                    }
                    else
                    {
                        machine.btnCM.Style = FindResource("cmGrayBorder") as Style;
                        machine.IsActive = false;
                        
                    }
                }
            }
        }
   

        private void lbl9_Click(object sender, RoutedEventArgs e)
        {
            if (lbl9.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl9.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("2", false);
            }
            else if (lbl9.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl9.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("2", true);
            }


        }

        private void lbl8_Click(object sender, RoutedEventArgs e)
        {
            if (lbl8.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl8.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("4", false);
            }
            else if (lbl8.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl8.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("4", true);
            }
        }

        private void lbl7_Click(object sender, RoutedEventArgs e)
        {
            if (lbl7.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl7.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("6", false);
            }
            else if (lbl7.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl7.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("6", true);
            }
        }

        private void lbl6_Click(object sender, RoutedEventArgs e)
        {
            if (lbl6.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl6.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("8", false);
            }
            else if (lbl6.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl6.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("8", true);
            }
        }

        private void lbl5_Click(object sender, RoutedEventArgs e)
        {
            if (lbl5.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl5.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("10", false);
            }
            else if (lbl5.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl5.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("10", true);
            }
        }

        private void lbl4_Click(object sender, RoutedEventArgs e)
        {
            if (lbl4.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl4.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("12", false);
            }
            else if (lbl4.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl4.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("12", true);
            }
        }

        private void lbl3_Click(object sender, RoutedEventArgs e)
        {
            if (lbl3.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl3.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("14", false);
            }
            else if (lbl3.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl3.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("14", true);
            }
        }

        private void lbl2_Click(object sender, RoutedEventArgs e)
        {
            if (lbl2.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                lbl2.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("16", false);
            }
            else if (lbl2.Template == FindResource("GlassButton") as ControlTemplate)
            {
                lbl2.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("16", true);
            }
        }

        private void label1_Click(object sender, RoutedEventArgs e)
        {
            if (label1.Template == FindResource("GlassButtonGreen") as ControlTemplate)
            {
                label1.Template = FindResource("GlassButton") as ControlTemplate;
                ChaneCMBorderColor("18", false);
            }
            else if (label1.Template == FindResource("GlassButton") as ControlTemplate)
            {
                label1.Template = FindResource("GlassButtonGreen") as ControlTemplate;
                ChaneCMBorderColor("18", true);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            OnStart(sender,e);
            //RetrieveSelectedCM();
        }

        private void btnStart1_Click(object sender, RoutedEventArgs e)
        {
            OnStart1(sender, e);
        }

        
        void GetDemoModeValue(string machineName, out Int16 fromAisle, out Int16 toAisle, out int delay, out int cycle)
        {
            try
            {
                fromAisle = 0;
                toAisle = 0;
                delay = 0;
                cycle = 0;
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        string sql = "SELECT F_FROM,F_TO,F_CYCLE,F_DELAY FROM L2_DEMO_MODE " + "  WHERE MACHINENAME= '" + machineName + "'";
                        command.CommandText = sql;
                        command.CommandType = System.Data.CommandType.Text;
                        
                        using (OracleDataReader dreader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                        {
                            if(dreader.HasRows)
                            {
                             dreader.Read();
                            fromAisle = Convert.ToInt16(dreader["F_FROM"]);
                            toAisle = Convert.ToInt16(dreader["F_TO"]);
                            cycle = Convert.ToInt16(dreader["F_CYCLE"]);
                            delay = Convert.ToInt32(dreader["F_DELAY"]);
                            }
                        }
                    }
                }
            }
            finally
            {

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
