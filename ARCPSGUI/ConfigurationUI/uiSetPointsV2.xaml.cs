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
using System.Data;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;

namespace ARCPSGUI.ConfigurationUI
{
    /// <summary>
    /// Interaction logic for uiSetPointsv1.xaml
    /// </summary>
    public partial class uiSetPointsV2 : UserControl
    {
        public event EventHandler triggerSetPointsUpdate;
        GeneralDba objGeneralDba = null;
        EESDba objEESDba = null;

        public uiSetPointsV2()
        {
            InitializeComponent();
        }

        #region Member Methods
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DoOnLoad();
        }

        public void DoOnLoad()
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objEESDba == null)
                objEESDba = new EESDba();
            try
            {
                SetFloorStatusInLoad();
                AssignDBValuesToControls();
                AssignToEESCombos();
                AssignNormalModeEES();
                SetStatusOfL2AutoScheduleToGui();
                SelectTab();
                initializeTimeSlider();
            }
            catch (Exception ex)
            {

            }
        }
        private void initializeTimeSlider()
        {
            morningRangeSlider.RangeStart = 1 * 60;
            morningRangeSlider.RangeStop = 24 * 60;

            morningRangeSlider.MinRange = 30;

            eveningRangeSlider.RangeStart = 1 * 60;
            eveningRangeSlider.RangeStop = 24 * 60;

            eveningRangeSlider.MinRange = 30;
        }
        private void morningRangeSlider_RangeSelectionChanged(object sender, AC.AvalonControlsLibrary.Controls.RangeSelectionChangedEventArgs e)
        {
           
            MorningModeStartTime.Content = e.NewRangeStart / 60 + ":" + e.NewRangeStart % 60;

            MorningModeEndTime.Content = e.NewRangeStop / 60 + ":" + e.NewRangeStop % 60;
           

        }
        private void eveningRangeSlider_RangeSelectionChanged(object sender, AC.AvalonControlsLibrary.Controls.RangeSelectionChangedEventArgs e)
        {
            eveningModeStartTime.Content = e.NewRangeStart / 60 + ":" + e.NewRangeStart % 60;

            eveningModeEndTime.Content = e.NewRangeStop / 60 + ":" + e.NewRangeStop % 60;
        }
        private DataTable GetModeMaster()
        {
            string query = "SELECT * FROM l2_mode_master";

            DataTable dt = new DataTable();
            dt.TableName = "ModeMaster";
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
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
                MessageBox.Show(errMsg.Message);
            }
            return dt;
        }

        private void SaveModeMaster()
        {
            try
            {

                objGeneralDba.SaveModeMaster(morningRangeSlider.RangeStartSelected / 60, morningRangeSlider.RangeStopSelected / 60,
                    morningRangeSlider.RangeStartSelected % 60, morningRangeSlider.RangeStopSelected % 60,
                    eveningRangeSlider.RangeStartSelected / 60, eveningRangeSlider.RangeStopSelected / 60,
                    eveningRangeSlider.RangeStartSelected % 60, eveningRangeSlider.RangeStopSelected % 60);
               
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while saving mode master. " + errMsg.Message);
            }
        }

        void AssignDBValuesToControls()
        {
            DataTable dt = GetModeMaster();

            foreach (DataRow drow in dt.Rows)
            {

                var startTime = drow["START_HOUR"];
                var endTime = drow["END_HOUR"];
                if (drow["MODE_NAME"].ToString() == "morning")
                {
                   
                    morningRangeSlider.RangeStartSelected = int.Parse(drow["START_HOUR"].ToString()) * 60 + int.Parse(drow["START_MIN"].ToString());
                    morningRangeSlider.RangeStopSelected = int.Parse(drow["END_HOUR"].ToString()) * 60 + int.Parse(drow["END_MIN"].ToString()); 

                }
                else if (drow["MODE_NAME"].ToString() == "evening")
                {
                    eveningRangeSlider.RangeStartSelected = int.Parse(drow["START_HOUR"].ToString()) * 60 + int.Parse(drow["START_MIN"].ToString());
                    eveningRangeSlider.RangeStopSelected = int.Parse(drow["END_HOUR"].ToString()) * 60 + int.Parse(drow["END_MIN"].ToString()); 

                }
            }
        }

        DataTable RetrieveEES()
        {
            string query = "SELECT EES_ID,EES_NAME,EVENING_MODE,MORNING_MODE FROM L2_EES_MASTER";

            DataTable dt = new DataTable();
            dt.TableName = "EESMASTER";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
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

        void AssignToEESCombos()
        {
            DataTable dtEESMaster1 = RetrieveEES().Copy();
            DataTable dtEESMaster2 = RetrieveEES().Copy();

            var drowEntryEESList = dtEESMaster1.AsEnumerable().Where(r => Convert.ToInt32(r["EVENING_MODE"]) == 1);
            var drowExitEESList = dtEESMaster1.AsEnumerable().Where(r => Convert.ToInt32(r["MORNING_MODE"]) == -1);



            DataRow drow1 = dtEESMaster1.NewRow();
            drow1["EES_ID"] = "0";
            drow1["EES_NAME"] = "NONE";
            drow1["EVENING_MODE"] = "0";
            drow1["MORNING_MODE"] = "0";
            dtEESMaster1.Rows.Add(drow1);

            DataRow drow2 = dtEESMaster2.NewRow();
            drow2["EES_ID"] = "0";
            drow2["EES_NAME"] = "NONE";
            drow2["EVENING_MODE"] = "0";
            drow1["MORNING_MODE"] = "0";
            dtEESMaster2.Rows.Add(drow2);

            cmbEntryEES1.DataContext = dtEESMaster1;
            cmbEntryEES2.DataContext = dtEESMaster2;

            cmbExitEES1.DataContext = dtEESMaster1;
            cmbExitEES2.DataContext = dtEESMaster2;
            //DataSet ds = new DataSet();
            //ds.Tables.Add(dtEESMaster1);

            //To display category name (DisplayMember in Visual Studio 2005)
            cmbEntryEES1.DisplayMemberPath =
                 dtEESMaster1.Columns["EES_NAME"].ToString();
            //To store the ID as hidden (ValueMember in Visual Studio 2005)
            cmbEntryEES1.SelectedValuePath =
                 dtEESMaster1.Columns["EES_ID"].ToString();

            //To display category name (DisplayMember in Visual Studio 2005)
            cmbEntryEES2.DisplayMemberPath =
                 dtEESMaster2.Columns["EES_NAME"].ToString();
            //To store the ID as hidden (ValueMember in Visual Studio 2005)
            cmbEntryEES2.SelectedValuePath =
                 dtEESMaster2.Columns["EES_ID"].ToString();



            //To display category name (DisplayMember in Visual Studio 2005)
            cmbExitEES1.DisplayMemberPath =
                 dtEESMaster1.Columns["EES_NAME"].ToString();
            //To store the ID as hidden (ValueMember in Visual Studio 2005)
            cmbExitEES1.SelectedValuePath =
                 dtEESMaster1.Columns["EES_ID"].ToString();

            //To display category name (DisplayMember in Visual Studio 2005)
            cmbExitEES2.DisplayMemberPath =
                 dtEESMaster2.Columns["EES_NAME"].ToString();
            //To store the ID as hidden (ValueMember in Visual Studio 2005)
            cmbExitEES2.SelectedValuePath =
                 dtEESMaster2.Columns["EES_ID"].ToString();


            cmbEntryEES1.SelectedValue = 0;
            cmbEntryEES2.SelectedValue = 0;
            if (drowEntryEESList.Count() > 0)
            {
                foreach (var r in drowEntryEESList)
                {
                    if (Convert.ToInt32(cmbEntryEES1.SelectedValue) == 0)
                        cmbEntryEES1.SelectedValue = r["EES_ID"];
                    else if (Convert.ToInt32(cmbEntryEES2.SelectedValue) == 0)
                        cmbEntryEES2.SelectedValue = r["EES_ID"];
                }
            }


            cmbExitEES1.SelectedValue = 0;
            cmbExitEES2.SelectedValue = 0;
            if (drowExitEESList.Count() > 0)
            {
                foreach (var r in drowExitEESList)
                {
                    if (Convert.ToInt32(cmbExitEES1.SelectedValue) == 0)
                        cmbExitEES1.SelectedValue = r["EES_ID"];
                    else if (Convert.ToInt32(cmbExitEES2.SelectedValue) == 0)
                        cmbExitEES2.SelectedValue = r["EES_ID"];
                }
            }

        }

     

      

        void SetStatusOfL2AutoScheduleToGui()
        {
            chkbUseL2Scheduling.IsChecked = objGeneralDba.GetStatusOfL2AutoSchedule();

           
        }

       
        void SaveEveningModeEntryEES()
        {
            try
            {
               
                int entry_ees1 = 0;
                int entry_ees2 = 0;
                string sql = null;
                entry_ees1 = (int)cmbEntryEES1.SelectedValue;
                entry_ees2 = (int)cmbEntryEES2.SelectedValue;
                string inItems = null;
                if (entry_ees1 != 0)
                    inItems += entry_ees1;
                if (entry_ees2 != 0)
                {
                    if (entry_ees1 != 0)
                        inItems += "," + entry_ees2;
                    else
                        inItems += entry_ees2;
                }
                if (entry_ees1 != 0 || entry_ees2 != 0)
                    inItems = "(" + inItems + ")";
                objEESDba.SaveEveningModeEntryEES(inItems);


            }
            catch (Exception errMsg)
            {
                MessageBox.Show(" Error occured while saving evening mode entry ees. " + errMsg.Message);
            }
        }
        void SaveMorningModeExitEES()
        {
            try
            {

                int exit_ees1 = 0;
                int exit_ees2 = 0;
                string sql = null;
                exit_ees1 = (int)cmbExitEES1.SelectedValue;
                exit_ees2 = (int)cmbExitEES2.SelectedValue;
                string inItems = null;
                if (exit_ees1 != 0)
                    inItems += exit_ees1;
                if (exit_ees2 != 0)
                {
                    if (exit_ees1 != 0)
                        inItems += "," + exit_ees2;
                    else
                        inItems += exit_ees2;
                }
                if (exit_ees1 != 0 || exit_ees2 != 0)
                    inItems = "(" + inItems + ")";
                objEESDba.SaveMorningModeExitEES(inItems);



            }
            catch (Exception errMsg)
            {
                MessageBox.Show(" Error occured while saving morning mode exit ees. " + errMsg.Message);
            }
        }
        

        void AssignNormalModeEES()
        {
            DataTable dtNormal = objEESDba.RetrieveNormalEES();

            Label lblName = null;
            CheckBox chkEntry = null;
            CheckBox chkExit = null;
            CheckBox chkMixed = null;
            Viewbox vb1 = null;
            Viewbox vb2 = null;
            Viewbox vb3 = null;
            Viewbox vb4 = null;
            int counter = 2;
            bool entryMode = false;
            bool exitMode = false;
            bool normalMode = false;
            foreach (DataRow drow in dtNormal.Rows)
            {
                lblName = new Label();
                lblName.Content = drow[1].ToString();
                vb1 = new Viewbox();
                vb1.Stretch = Stretch.Uniform;
                lblName.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                lblName.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                lblName.FontSize = 25;
                lblName.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                vb1.Child = lblName;
                Grid.SetColumn(vb1, 0);
                Grid.SetRow(vb1, counter);
                grdNormalMode.Children.Add(vb1);

                if (Convert.ToString(drow["NORMAL_MIX_EES"]) == "0" && Convert.ToString(drow["NORMAL_MODE"]) == "1")
                {
                    entryMode = true;
                    exitMode = false;
                    normalMode = false;
                }
                else if (Convert.ToString(drow["NORMAL_MIX_EES"]) == "0" && Convert.ToString(drow["NORMAL_MODE"]) == "-1")
                {
                    entryMode = false;
                    exitMode = true;
                    normalMode = false;
                }
                else if (Convert.ToString(drow["NORMAL_MIX_EES"]) != "0")
                {
                    entryMode = false;
                    exitMode = false;
                    normalMode = true;
                }

                chkEntry = new CheckBox();
                chkEntry.Name = drow[1].ToString() + 1;
                chkEntry.Tag = drow[1].ToString();
                //lblName.Content = drow[1].ToString();
                //vb2 = new Viewbox();
                //vb2.Child = chkEntry;
                Grid.SetColumn(chkEntry, 1);
                Grid.SetRow(chkEntry, counter);
                grdNormalMode.Children.Add(chkEntry);
                chkEntry.IsChecked = entryMode;
                chkEntry.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                chkEntry.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                chkEntry.Checked += new RoutedEventHandler(chkEntry_Checked);

                chkExit = new CheckBox();
                chkExit.Name = drow[1].ToString() + 2;
                chkExit.Tag = drow[1].ToString();
                //lblName.Content = drow[1].ToString();
                //vb3 = new Viewbox();
                //vb3.Child = chkExit;
                Grid.SetColumn(chkExit, 2);
                Grid.SetRow(chkExit, counter);
                grdNormalMode.Children.Add(chkExit);
                chkExit.IsChecked = exitMode;
                chkExit.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                chkExit.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                chkExit.Checked += new RoutedEventHandler(chkExit_Checked);

                chkMixed = new CheckBox();
                chkMixed.Name = drow[1].ToString() + 3;
                chkMixed.Tag = drow[1].ToString(); ;
                //lblName.Content = drow[1].ToString();
                //vb4 = new Viewbox();
                //vb4.Child = chkMixed;
                Grid.SetColumn(chkMixed, 3);
                Grid.SetRow(chkMixed, counter);
                grdNormalMode.Children.Add(chkMixed);
                chkMixed.IsChecked = normalMode;
                chkMixed.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                chkMixed.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                chkMixed.Checked += new RoutedEventHandler(chkMixed_Checked);

                counter += 2;
            }


        }

        void chkMixed_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCheckBoxState((CheckBox)sender);
        }
        void chkExit_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCheckBoxState((CheckBox)sender);
        }
        void chkEntry_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCheckBoxState((CheckBox)sender);
        }
        void ToggleCheckBoxState(CheckBox chkb)
        {
            string chkboxName = "";

            chkboxName = chkb.Name;

            int number = Convert.ToInt32(chkboxName.Substring(4, 1));
            string chkb2name = "";
            string chkb3name = "";

            if (number == 1)
            {
                chkb2name = chkboxName.Substring(0, 4) + "2";
                chkb3name = chkboxName.Substring(0, 4) + "3";
            }
            else if (number == 2)
            {
                chkb2name = chkboxName.Substring(0, 4) + "1";
                chkb3name = chkboxName.Substring(0, 4) + "3";
            }
            else if (number == 3)
            {
                chkb2name = chkboxName.Substring(0, 4) + "1";
                chkb3name = chkboxName.Substring(0, 4) + "2";
            }


            if (chkb.IsChecked == true)
            {
                CheckBox chk1 = null;
                var result1 = grdNormalMode.Children.OfType<CheckBox>().Where(name => name.Name == chkb2name);
                if (result1.Count() > 0)
                {
                    chk1 = result1.First();
                }

                CheckBox chk2 = null;
                var result2 = grdNormalMode.Children.OfType<CheckBox>().Where(name => name.Name == chkb3name);
                if (result2.Count() > 0)
                {
                    chk2 = result2.First();
                }

                if (chkb2name.Substring(4, 1) == "1")
                {
                    chk1.Checked -= chkExit_Checked;
                    chk2.Checked -= chkMixed_Checked;
                }
                else if (chkb2name.Substring(4, 1) == "2")
                {
                    chk1.Checked -= chkEntry_Checked;
                    chk2.Checked -= chkMixed_Checked;
                }
                else if (chkb2name.Substring(4, 1) == "2")
                {
                    chk1.Checked -= chkEntry_Checked;
                    chk2.Checked -= chkExit_Checked;
                }

                chk1.IsChecked = false;
                chk2.IsChecked = false;

                chk1.Checked += chkEntry_Checked;
                chk2.Checked += chkExit_Checked;
            }


        }

        void SaveNormalModeEES()
        {
            try
            {
                var lstChkBos = grdNormalMode.Children.OfType<CheckBox>().Where(res => res.IsChecked == true);

                string eesName = "";
                int eesMode = 0;


                foreach (var val in lstChkBos)
                {
                    eesName = val.Tag.ToString();
                    if (val.Name.Substring(4, 1) == "1") eesMode = 1;
                    if (val.Name.Substring(4, 1) == "2") eesMode = -1;
                    if (val.Name.Substring(4, 1) == "3") eesMode = 0;

                    objEESDba.SaveNormalModeEES(eesName, eesMode);
                }

            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while saving normal ees mode. " + errMsg.Message);
            }
        }


        #endregion

     
      

        void SelectTab()
        {
            int currentMode = Connection.GetCurrentMode();
            if (currentMode == 1)
                tabControl1.SelectedIndex = 0;
            else if (currentMode == 2)
                tabControl1.SelectedIndex = 2;
            else if (currentMode == 3)
                tabControl1.SelectedIndex = 1;

            if (tabControl1.SelectedItem != null)
                ((TabItem)tabControl1.SelectedItem).Style = (Style)FindResource("CurrentModeTabColor");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveModeMaster();
            SaveMorningModeExitEES();
            SaveEveningModeEntryEES();
            SaveNormalModeEES();
            objGeneralDba.SaveStatusOfL2AutoSchedule(chkbUseL2Scheduling.IsChecked == true);
          
            MessageBox.Show("Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.triggerSetPointsUpdate(sender,new EventArgs());
        }

        private void chkLevel_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox objCheck = (CheckBox)sender;
           
            //bool isParking = false;
            int floor = 0;
            string enableType = objCheck.Name.Split('_')[0];
            //isParking = objCheck.Name.Split('_')[0]=="P";
            floor = Convert.ToInt32(objCheck.Name.Split('_')[1]);
            bool isBlocked=!(bool)objCheck.IsChecked;
           // P_1
            if (enableType=="P") //parking
            {
                objGeneralDba.UpdateParkFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetParkFloorStatus(floor);
            }
            else if (enableType == "FPM")  //pallet feeding
            {
                objGeneralDba.UpdatePMSFeedFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetPMSFeedFloorStatus(floor);
            }
            else if (enableType == "SPM") //pallet storing
            {
                objGeneralDba.UpdatePMSStoreFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetPMSStoreFloorStatus(floor);
            }
            
        }
        private void chkLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox objCheck = (CheckBox)sender;

            //bool isParking = false;
            int floor = 0;
            string enableType = objCheck.Name.Split('_')[0];
           // isParking = objCheck.Name.Split('_')[0] == "P";
            floor = Convert.ToInt32(objCheck.Name.Split('_')[1]);
            bool isBlocked = !(bool)objCheck.IsChecked;
            // P_1
            if (enableType == "P") //parking
            {
                objGeneralDba.UpdateParkFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetParkFloorStatus(floor);
            }
            else if (enableType == "FPM")  //pallet feeding
            {
                objGeneralDba.UpdatePMSFeedFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetPMSFeedFloorStatus(floor);
            }
            else if (enableType == "SPM")  //pallet feeding
            {
                objGeneralDba.UpdatePMSStoreFloorStatus(floor, isBlocked);
                objCheck.IsChecked = !objGeneralDba.GetPMSStoreFloorStatus(floor);
            }
        }
        void AddHours()
        {

        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentMode = Connection.GetCurrentMode();

            tbiMorMode.Style = (Style)FindResource("DefaultTabColor");
            tbiNorMode.Style = (Style)FindResource("DefaultTabColor");
            tbiEveMode.Style = (Style)FindResource("DefaultTabColor");
            tbiConfig.Style = (Style)FindResource("DefaultTabColor");


            if(tabControl1.SelectedItem != null)
             ((TabItem)tabControl1.SelectedItem).Style = (Style)FindResource("SelectedTabColor");

            if (currentMode == 1)
                tbiMorMode.Style = (Style)FindResource("CurrentModeTabColor");
            else if (currentMode == 3)
                tbiNorMode.Style = (Style)FindResource("CurrentModeTabColor");
            else if (currentMode == 2)
                tbiEveMode.Style = (Style)FindResource("CurrentModeTabColor");

        }

      
        void SetFloorStatusInLoad()
        {
            bool isBlocked = false;
            string query = "";
            for (int i = 1; i <= 9; i++)
            {

                isBlocked = objGeneralDba.GetParkFloorStatus(i);
                switch (i)
                {
                    case 1:
                        P_1.IsChecked = !isBlocked;
                        break;
                    case 2:
                        P_2.IsChecked = !isBlocked;
                        break;
                    case 3:
                        P_3.IsChecked = !isBlocked;
                        break;
                    case 4:
                        P_4.IsChecked = !isBlocked;
                        break;

                    case 5:
                        P_5.IsChecked = !isBlocked;
                        break;
                    case 6:
                        P_6.IsChecked = !isBlocked;
                        break;
                    case 7:
                        P_7.IsChecked = !isBlocked;
                        break;
                    case 8:
                        P_8.IsChecked = !isBlocked;
                        break;
                    case 9:
                        P_9.IsChecked = !isBlocked;
                        break;
                    default:
                        break;
                };


            }

            for (int i = 4; i <= 7; i++)
            {

                isBlocked = objGeneralDba.GetPMSFeedFloorStatus(i);
                switch (i)
                {

                    case 4:
                        FPM_4.IsChecked = !isBlocked;
                        break;

                    case 5:
                        FPM_5.IsChecked = !isBlocked;
                        break;
                    case 6:
                        FPM_6.IsChecked = !isBlocked;
                        break;
                    case 7:
                        FPM_7.IsChecked = !isBlocked;
                        break;
                    default:
                        break;
                };


            }
            for (int i = 4; i <= 7; i++)
            {

                isBlocked = objGeneralDba.GetPMSStoreFloorStatus(i);
                switch (i)
                {

                    case 4:
                        SPM_4.IsChecked = !isBlocked;
                        break;

                    case 5:
                        SPM_5.IsChecked = !isBlocked;
                        break;
                    case 6:
                        SPM_6.IsChecked = !isBlocked;
                        break;
                    case 7:
                        SPM_7.IsChecked = !isBlocked;
                        break;
                    default:
                        break;
                };


            }

        }

       
       

      

       
    }
}
