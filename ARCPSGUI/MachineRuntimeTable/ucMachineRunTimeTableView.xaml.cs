﻿using System;
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

using ARCPSGUI.DB;
using ARCPSGUI.OPC;
using System.Threading.Tasks;

namespace ARCPSGUI.MachineRuntimeTable
{
    /// <summary>
    /// Interaction logic for ucMachineRunTimeTableView.xaml
    /// </summary>
    public partial class ucMachineRunTimeTableView : UserControl
    {
        OPCServerDirector opcd = new OPCServerDirector();
        Connection dbpm = new Connection();

       public  List<string> lstRunTimeMachineAlarm = new List<string>();

        public ucMachineRunTimeTableView()
        {
            InitializeComponent();
            tbctrlRTable.SelectedIndex = 0;
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoad();
          
        }

        public void OnLoad()
        {
            GetMachines(1);
            GetREM(1);
            GetMachines(2);
            GetREM(2);
            GetMachines(3);
            // GetREM(3);
        }

        void GetMachines(int page)
        {
            try
            {
                Grid grd = null;
                if (page == 1)
                    grd = grdMachineList;
                else if (page == 2)
                    grd = grdMachineList2;
                else if (page == 3)
                    grd = grdMachineList3;

                int runhr = 0;
                int setpointvalue = 0;
                int cablereelvalue = 0;
                int cablereeldbvalue = 0;
                int ttvalue = 0;
                int ttdbvalue = 0;

                bool needToReset = false;
                if (grd != null)
                {
                    DataTable dtMachines = new Connection().GetCMRunTimeTable(page, false);
                    int grdrowpos = 2;
                    string tagName = "";
                   
                    if (page == 3) grdrowpos = 4;

                    for (int row = 0; row <= dtMachines.Rows.Count - 1; row++)
                    {
                        List<string> lstTag = new List<string>(); ;
                        runhr = 0;
                        setpointvalue = 0;
                        cablereelvalue = 0;
                        cablereeldbvalue = 0;
                        ttvalue = 0;
                        ttdbvalue = 0;
                        needToReset = false;
                        //Create machine name.
                        lstTag.Add(Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                        Viewbox vbMachine = CreateMachines(Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                        Grid.SetColumn(vbMachine, 0);
                        Grid.SetRow(vbMachine, grdrowpos);
                        grd.Children.Add(vbMachine);

                        //Create Run Time Hours - Run Hours
                        tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                  + "." + Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]);
                        lstTag.Add(tagName);

                        Viewbox vbRTH_RH = CreateRunHours(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                             "rthrh" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out runhr, Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                        Grid.SetColumn(vbRTH_RH, 2);
                        Grid.SetRow(vbRTH_RH, grdrowpos);
                        grd.Children.Add(vbRTH_RH);

                        //Create Run Time Hours - Set Points
                        Viewbox vbRTH_SP = CreateSetPoints(Convert.ToString(dtMachines.Rows[row]["MACHINE"]),
                            "rthsp" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out setpointvalue);
                        Grid.SetColumn(vbRTH_SP, 4);
                        Grid.SetRow(vbRTH_SP, grdrowpos);
                        grd.Children.Add(vbRTH_SP);

                        if (runhr != 0 && setpointvalue != 0) needToReset = (runhr >= setpointvalue);

                        if (page != 3)
                        {
                            //Create Cable Reel - Run Hours
                            tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                      + "." + Convert.ToString(dtMachines.Rows[row]["CABLEREELNOOFHOURS"]);
                            lstTag.Add(tagName);

                            Viewbox vbCR_RH = CreateRunHours(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                                 "cbrh" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out cablereelvalue, Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                            Grid.SetColumn(vbCR_RH, 6);
                            Grid.SetRow(vbCR_RH, grdrowpos);
                            grd.Children.Add(vbCR_RH);

                            //Create Run Time Hours - Set Points
                            Viewbox vbCB_SP = CreateSetPoints(Convert.ToString(dtMachines.Rows[row]["MACHINE"]),
                                "crsp" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out cablereeldbvalue);
                            Grid.SetColumn(vbCB_SP, 8);
                            Grid.SetRow(vbCB_SP, grdrowpos);
                            grd.Children.Add(vbCB_SP);

                            if (cablereelvalue != 0 && cablereeldbvalue != 0) needToReset |= (cablereelvalue >= cablereeldbvalue);

                            //LCM - Rotate
                            if (Convert.ToString(dtMachines.Rows[row]["MACHINE"]).Contains("LCM"))
                            {
                                tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                          + "." + Convert.ToString(dtMachines.Rows[row]["TTNoRot"]);
                                lstTag.Add(tagName);

                                Viewbox vbRotation = CreateRotation(Convert.ToString(dtMachines.Rows[row]["MACHINE"]), tagName,
                                     "lcmRotation" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out ttvalue, Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                                Grid.SetColumn(vbRotation, 12);
                                Grid.SetRow(vbRotation, grdrowpos);
                                grd.Children.Add(vbRotation);

                                //Create Turn Table - Set Points
                                Viewbox vbTT_SP = CreateSetPoints(Convert.ToString(dtMachines.Rows[row]["MACHINE"]),
                                    "tt" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out ttdbvalue);
                                Grid.SetColumn(vbTT_SP, 14);
                                Grid.SetRow(vbTT_SP, grdrowpos);
                                grd.Children.Add(vbTT_SP);

                                if (ttvalue != 0 && ttdbvalue != 0) needToReset |= (ttvalue >= ttdbvalue);
                            }

                            //Create Machine - ResetButton
                            tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                       + "." + Convert.ToString(dtMachines.Rows[row]["RESETALL"]);

                            Viewbox vbMachineReset = CreateReset(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                                 "macReset" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), needToReset, lstTag);
                            Grid.SetColumn(vbMachineReset, 16);
                            Grid.SetRow(vbMachineReset, grdrowpos);
                            grd.Children.Add(vbMachineReset);



                        }
                        else if (page == 3)
                        {
                            //Create Machine - ResetButton
                            tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                       + "." + Convert.ToString(dtMachines.Rows[row]["RESETALL"]);
                            //lstTag.Add(tagName);

                            Viewbox vbMachineReset = CreateReset(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                                 "macReset" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), needToReset, lstTag);
                            Grid.SetColumn(vbMachineReset, 6);
                            Grid.SetRow(vbMachineReset, grdrowpos);
                            grd.Children.Add(vbMachineReset);
                        }
                        grdrowpos += 2;
                    }
                }
            }
            finally
            {

            }
        }

        void GetREM(int page)
        {
            try
            {
                Grid grd = null;
                if (page == 1)
                    grd = grdMachineList;
                else if (page == 2)
                    grd = grdMachineList2;

                if (grd != null)
                {
                    float remRunHours = 0;
                    float remSetPointValue = 0;
                    bool needToReset = false;

                    DataTable dtMachines = new Connection().GetCMRunTimeTable(page, true);
                    int grdrowpos = 2;
                    string tagName = "";
                  
                    for (int row = 0; row <= dtMachines.Rows.Count - 1; row++)
                    {
                        List<string> lstTag = new List<string>();
                        remRunHours = 0;
                        remSetPointValue = 0;
                        needToReset = false;
                        //Create machine name.
                        lstTag.Add(Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                        Viewbox vbMachine = CreateMachines(Convert.ToString(dtMachines.Rows[row]["MACHINE"]));
                        Grid.SetColumn(vbMachine, 18);
                        Grid.SetRow(vbMachine, grdrowpos);
                        grd.Children.Add(vbMachine);

                        //Create Run Time Hours - Run Hours
                        tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                  + "." + Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]);
                        lstTag.Add(tagName);

                        Viewbox vbRTH_RH = CreateREMRunHours(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                             "remrthrh" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out remRunHours);
                        Grid.SetColumn(vbRTH_RH, 20);
                        Grid.SetRow(vbRTH_RH, grdrowpos);
                        grd.Children.Add(vbRTH_RH);

                        //Create Run Time Hours - Set Points
                        Viewbox vbRTH_SP = CreateREMSetPoints(Convert.ToString(dtMachines.Rows[row]["MACHINE"]),
                            "remrthsp" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), out remSetPointValue);
                        Grid.SetColumn(vbRTH_SP, 22);
                        Grid.SetRow(vbRTH_SP, grdrowpos);
                        grd.Children.Add(vbRTH_SP);

                        if (remRunHours != 0 && remSetPointValue != 0) needToReset = (remRunHours >= remSetPointValue);

                        //Create Machine - ResetButton
                        tagName = Convert.ToString(dtMachines.Rows[row]["CHANNEL"]) + "." + Convert.ToString(dtMachines.Rows[row]["MACHINE"])
                                   + "." + Convert.ToString(dtMachines.Rows[row]["RESETALL"]);
                        Viewbox vbMachineReset = CreateReset(Convert.ToString(dtMachines.Rows[row]["MACHINENOOFHOURS"]), tagName,
                             "remReset" + Convert.ToString(dtMachines.Rows[row]["MACHINE"]), needToReset, lstTag);
                        Grid.SetColumn(vbMachineReset, 24);
                        Grid.SetRow(vbMachineReset, grdrowpos);
                        grd.Children.Add(vbMachineReset);

                        grdrowpos += 2;
                    }
                }
            }
            finally
            {

            }
        }

        Viewbox CreateMachines(string machineName)
        {
            Viewbox vbMachine = new Viewbox();
            Label lblMachine = new Label();
            lblMachine.Name = machineName;
            lblMachine.Content = machineName;
            vbMachine.Child = lblMachine;
            return vbMachine;
        }

        Viewbox CreateRunHours(string machineName, string tagName, string name, out int value,string machineActualName)
        {
            value = 0;
            Viewbox vbMachine = new Viewbox();
            Label lblMachine = new Label();
            lblMachine.Name = name;
            value = ReadValue(tagName);
            lblMachine.Content = value;
            lblMachine.Tag = tagName;
            vbMachine.Tag = tagName;
            vbMachine.Name = machineActualName;
            vbMachine.Child = lblMachine;
            return vbMachine;
        }
        Viewbox CreateREMRunHours(string machineName, string tagName, string name, out float value)
        {
            value = 0;
            Viewbox vbMachine = new Viewbox();
            Label lblMachine = new Label();
            lblMachine.Name = name;
            value = ReadREMValue(tagName);
            lblMachine.Content = value;
            lblMachine.Tag = tagName;
            vbMachine.Tag = tagName;
            vbMachine.Name = machineName;
            vbMachine.Child = lblMachine;
            return vbMachine;
        }
        Viewbox CreateRotation(string machineName, string tagName, string name, out int value, string machineActualName)
        {
            value = 0;
            Viewbox vbMachine = new Viewbox();
            Label lblMachine = new Label();
            lblMachine.Name = name;
            value = ReadValue(tagName);
            lblMachine.Content = value;
            lblMachine.Tag = tagName;
            vbMachine.Tag = tagName;
            vbMachine.Name = machineActualName;
            vbMachine.Child = lblMachine;
            return vbMachine;
        }

        Viewbox CreateSetPoints(string machineName, string name, out int value)
        {
            value = 0;
            Viewbox vbMachine = new Viewbox();
            TextBox txtRTH_SP = new TextBox();
            txtRTH_SP.BorderThickness = new Thickness(0.2);
            txtRTH_SP.MaxLength = 4;
            txtRTH_SP.Name = name;
            txtRTH_SP.Tag = machineName;
            int type = 0;
            if (name.Contains("rthsp"))
                type = 1;
            else if (name.Contains("crsp"))
                type = 2;
            else if (name.Contains("tt"))
                type = 3;

            value = dbpm.GetSetPoints(machineName, type);
            txtRTH_SP.Text = Convert.ToString(value);
            txtRTH_SP.TextAlignment = TextAlignment.Right;
            vbMachine.Child = txtRTH_SP;
            txtRTH_SP.Width = 40;
            txtRTH_SP.Height = 20;
            txtRTH_SP.Background = Brushes.LightYellow;
            txtRTH_SP.PreviewTextInput += new TextCompositionEventHandler(txtRTH_SP_PreviewTextInput);
            txtRTH_SP.KeyUp += new KeyEventHandler(OnTextEnterEvent);
            return vbMachine;
        }
        Viewbox CreateREMSetPoints(string machineName, string name, out float value)
        {
            value = 0;
            Viewbox vbMachine = new Viewbox();
            TextBox txtRTH_SP = new TextBox();
            txtRTH_SP.BorderThickness = new Thickness(0.2);
            txtRTH_SP.MaxLength = 4;
            txtRTH_SP.Name = name;
            txtRTH_SP.Tag = machineName;
            int type = 0;
            if (name.Contains("rthsp"))
                type = 1;
            else if (name.Contains("crsp"))
                type = 2;
            else if (name.Contains("tt"))
                type = 3;

            value = dbpm.GetSetRemPoints(machineName, type);
            txtRTH_SP.Text = Convert.ToString(value);
            txtRTH_SP.TextAlignment = TextAlignment.Right;
            vbMachine.Child = txtRTH_SP;
            txtRTH_SP.Width = 40;
            txtRTH_SP.Height = 20;
            txtRTH_SP.Background = Brushes.LightYellow;
            txtRTH_SP.PreviewTextInput += new TextCompositionEventHandler(txtRTH_SP_PreviewTextInput);
            txtRTH_SP.KeyUp += new KeyEventHandler(OnREMTextEnterEvent);
            return vbMachine;
        }

        Viewbox CreateReset(string machineName, string tagName, string name, bool isEnable, List<string> lstReference)
        {
            Viewbox vbMachine = new Viewbox();
            System.Windows.Controls.Button btnMacRest = new System.Windows.Controls.Button();
            btnMacRest.Name = name;
            btnMacRest.Content = "Reset";
            btnMacRest.Width = 45;
            vbMachine.Child = btnMacRest;

            if (isEnable)
            {
                btnMacRest.Template = (ControlTemplate)FindResource("ResetActive");

                if (!lstRunTimeMachineAlarm.Contains(lstReference[0]))
                    lstRunTimeMachineAlarm.Add(lstReference[0]);
            }
            else
            {
                btnMacRest.Template = (ControlTemplate)FindResource("ResetDeActive");
                if (lstRunTimeMachineAlarm.Contains(lstReference[0]))
                    lstRunTimeMachineAlarm.Add(lstReference[0]);
            }
            btnMacRest.IsEnabled = isEnable;

            btnMacRest.Tag = lstReference; // as on feb 26
            vbMachine.Tag = tagName; 
            btnMacRest.IsEnabled = isEnable;
            btnMacRest.Click += new RoutedEventHandler(btnMacRest_Click);
            return vbMachine;
        }
   
        void OnTextEnterEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtSetPoints = sender as TextBox;
                UInt16 setPointValue = 0;
                if(txtSetPoints != null)
                {
                   UInt16.TryParse(txtSetPoints.Text.ToString(), out setPointValue);
                   if (setPointValue != 0)
                   {
                       bool bOk = Save(Convert.ToString(txtSetPoints.Name), Convert.ToString(txtSetPoints.Tag), setPointValue);
                       
                       if (bOk) MessageBox.Show("Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                       
                       TabItem tbitem = tbctrlRTable.SelectedItem as TabItem;
                       if (tbitem != null) tbitem.Focus();

                   }
                }
            }
        }

        void OnREMTextEnterEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtSetPoints = sender as TextBox;
                float setPointValue = 0;
                if (txtSetPoints != null)
                {
                    float.TryParse(txtSetPoints.Text.ToString(), out setPointValue);
                    if (setPointValue != 0)
                    {
                      bool bOk = Save(Convert.ToString(txtSetPoints.Name), Convert.ToString(txtSetPoints.Tag), setPointValue);
                      if (bOk) MessageBox.Show("Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                      TabItem tbitem = tbctrlRTable.SelectedItem as TabItem;
                      if (tbitem != null) tbitem.Focus();

                    }
                }
            }
        }

        void txtRTH_SP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        void btnMacRest_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btnMacRest = null;
            try
            {
                string macName = "";
                string channel = "";
                string tag = "";

              bool isRem = false;
                btnMacRest = ( System.Windows.Controls.Button) sender;
                if(btnMacRest != null)
                {
                      List<string> lstRef =  (List<string>) btnMacRest.Tag;

                      string[] split = lstRef[1].Split('.');

                      channel = split[0];
                      macName = split[1];
                      tag = "Reset_All";
                      string command = channel + "." + macName + "." + tag;
                    
                    if (btnMacRest.Name.Contains("remReset"))
                    { //Reset_All
                       // opcd.Write<bool>(Convert.ToString(btnMacRest.Tag.ToString()), true);
                        opcd.Write<bool>(command, true);
                        isRem = true;
                        opcd.Write<bool>(command, false);
                        //opcd.Write<bool>(Convert.ToString(btnMacRest.Tag.ToString()), false);
                    }
                    else
                    {
                        //opcd.Write<bool>(Convert.ToString(btnMacRest.Tag.ToString()), true);
                        opcd.Write<bool>(command, true);
                        isRem = false;
                       // opcd.Write<bool>(Convert.ToString(btnMacRest.Tag.ToString()), false);
                        opcd.Write<bool>(command, false);
                    }

                    //string[] split = Convert.ToString(btnMacRest.Tag).Split('.');
                    List<string> lststring = (List<string>) btnMacRest.Tag;

                    //if (split.Length == 3)
                    if (lststring != null) 
                    {
                        string machineName = lststring[0];  //split[1];
                        ResetTextBoxValues(machineName,isRem);
                        btnMacRest.Template = (ControlTemplate)FindResource("ResetDeActive");
                        btnMacRest.IsEnabled = false;
                    }
                }
            }
            finally { }
            
        }

        UInt16 ReadValue(string tagName)
        {
            return opcd.ReadTag<UInt16>(tagName);
        }
        float ReadREMValue(string tagName)
        {
            return opcd.ReadTag<float>(tagName);
        }

        bool Save(string tagvalue, string machineName, float value)
        {
            int type = 0;
            if (machineName.Contains("rthsp"))
                type = 1;
            else if (machineName.Contains("crsp"))
                type = 2;
            else if (machineName.Contains("tt"))
                type = 3;

            return dbpm.SaveSetPoints(machineName, value, type);

        }

        bool Save(string tagvalue, string machineName, UInt16 value)
        {
            int type = 0;
            if (tagvalue.Contains("rthsp"))
                type = 1;
            else if (tagvalue.Contains("crsp"))
                type = 2;
            else if (tagvalue.Contains("tt"))
                type = 3;

            return dbpm.SaveSetPoints(machineName, value, type);

        }

        void Refresh()
        {
            Grid grd = null;
        
            bool isRem = false;
            if (tbctrlRTable.SelectedIndex == 0)
                grd = grdMachineList;
            else if (tbctrlRTable.SelectedIndex == 1)
                grd = grdMachineList2;
            else if (tbctrlRTable.SelectedIndex == 2)
                grd = grdMachineList3;

            var result = from r in grd.Children.OfType<Viewbox>()
                         select r.Child;

            Label lblResult = null;
            System.Windows.Controls.Button btnMacRest = null;

            foreach (UIElement uielem in result) 
            {
                lblResult = null;
                btnMacRest = null;

                if (uielem is System.Windows.Controls.Button)
                    btnMacRest = uielem as System.Windows.Controls.Button;
                else if (uielem is Label)
                    lblResult = uielem as Label;
                
                if (lblResult != null && !string.IsNullOrEmpty(Convert.ToString(lblResult.Tag)))
                {
                    isRem = lblResult.Name.Contains("rem");
                    if (isRem) lblResult.Content = ReadREMValue(Convert.ToString(lblResult.Tag));
                    else lblResult.Content = ReadValue(Convert.ToString(lblResult.Tag));
                }
                else if (btnMacRest != null)
                {
                    List<string> lstRefer = (List<string>)btnMacRest.Tag;
                    string machineName = lstRefer[0];
                   
                    ushort noOfHours = 0;
                    ushort dbNoOfHoursSetPoints = 0;

                    ushort cableReelNoOfHours = 0;
                    ushort dbCableReelNoOfHoursSetPoints = 0;

                    ushort ttNoRotNoOfHours = 0;
                    ushort dbTTNoRotNoOfHoursSetPoints = 0;

                    float remNoOfHours = 0;
                    float dbRemNoOfHoursSetPoints = 0;
                    float remCableReelNoOfHours = 0;
                    float dbRemCableReelNoOfHoursSetPoints = 0;
                    float RemTTNoRotNoOfHours = 0;
                    float dbRemTTNoRotNoOfHoursSetPoints = 0;

                    bool needToReset = false;
                    for (int i = 1; i < lstRefer.Count; i++)
                    {
                        if (lstRefer[i].Contains("REM_"))
                        {
                            if (lstRefer[i].Contains("N0_of_hours"))
                            {
                                remNoOfHours = ReadREMValue(lstRefer[i]);
                                dbRemNoOfHoursSetPoints = dbpm.GetSetRemPoints(machineName, 1);
                                needToReset = NeedToEnableResetButtoin(remNoOfHours, dbRemNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }
                            else if (lstRefer[i].Contains("CableReel_No_hours"))
                            {
                                remCableReelNoOfHours = ReadREMValue(lstRefer[i]);
                                dbRemCableReelNoOfHoursSetPoints = dbpm.GetSetRemPoints(machineName, 2);
                                needToReset = NeedToEnableResetButtoin(remCableReelNoOfHours, dbRemCableReelNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }
                            else if (lstRefer[i].Contains("TT_No_Rot"))
                            {
                                RemTTNoRotNoOfHours = ReadREMValue(lstRefer[i]);
                                dbRemTTNoRotNoOfHoursSetPoints = dbpm.GetSetRemPoints(machineName, 3);
                                needToReset = NeedToEnableResetButtoin(RemTTNoRotNoOfHours, dbRemTTNoRotNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }  

                        }
                        else
                        {
                            if (lstRefer[i].Contains("N0_of_hours") || lstRefer[i].Contains("Runtime_Hours"))
                            {
                                noOfHours = ReadValue(lstRefer[i]);
                                dbNoOfHoursSetPoints = dbpm.GetSetPoints(machineName, 1);
                                needToReset = NeedToEnableResetButtoin(noOfHours, dbNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }
                            else if (lstRefer[i].Contains("CableReel_No_hours"))
                            {
                                cableReelNoOfHours = ReadValue(lstRefer[i]);
                                dbCableReelNoOfHoursSetPoints = dbpm.GetSetPoints(machineName, 2);
                                needToReset = NeedToEnableResetButtoin(cableReelNoOfHours, dbCableReelNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }
                            else if (lstRefer[i].Contains("TT_No_Rot"))
                            {
                                ttNoRotNoOfHours = ReadValue(lstRefer[i]);
                                dbTTNoRotNoOfHoursSetPoints = dbpm.GetSetPoints(machineName, 3);
                                needToReset = NeedToEnableResetButtoin(ttNoRotNoOfHours, dbTTNoRotNoOfHoursSetPoints);
                                ResetButtonStyle(ref btnMacRest, needToReset);
                                if (needToReset) break;
                            }
                        }

                    }
                }
            }
        }

        bool NeedToEnableResetButtoin(ushort valueFromMachine, ushort valueFromDB)
        { 
            bool needtoReset = false;
            if ((valueFromMachine != 0 && valueFromDB != 0) && (valueFromMachine >= valueFromDB))
                needtoReset = true;

            return needtoReset;
        }
        bool NeedToEnableResetButtoin(float valueFromMachine, float valueFromDB)
        {
            bool needtoReset = false;
            if ((valueFromMachine != 0 && valueFromDB != 0) && (valueFromMachine >= valueFromDB))
                needtoReset = true;

            return needtoReset;
        }

        void ResetButtonStyle(ref  System.Windows.Controls.Button btnMacRest, bool isEnable)
        {
             if (isEnable)
                btnMacRest.Template = (ControlTemplate)FindResource("ResetActive");
            else
                btnMacRest.Template = (ControlTemplate)FindResource("ResetDeActive");

             btnMacRest.IsEnabled = isEnable;

        }
        void ResetTextBoxValues(string machineName,bool isRem)
        {
            Grid grd = null;

            if (tbctrlRTable.SelectedIndex == 0)
                grd = grdMachineList;
            else if (tbctrlRTable.SelectedIndex ==1)
                grd = grdMachineList2;
            else if (tbctrlRTable.SelectedIndex == 2)
                grd = grdMachineList3;


            var vb = grd.Children.OfType<Viewbox>().Where(r => r.Name == machineName);
            if (vb != null)
            {
                foreach(var ctrl in vb)
                {
                    Label lbl = ctrl.Child as Label;
                    string name = lbl.Name;
                    if(!isRem)
                      lbl.Content = ReadValue(Convert.ToString(lbl.Tag));
                    else
                        lbl.Content = ReadREMValue(Convert.ToString(lbl.Tag));
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

    }
}
