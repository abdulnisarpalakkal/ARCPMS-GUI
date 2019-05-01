using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;

namespace ARCPSGUI.Controls
{
    public partial class ucRptViewer : UserControl
    {
        public ucRptViewer()
        {
            InitializeComponent();
        }

        public void LoadCurrentParksReport(DataSet ds)
        {
            try
            {
                 string filePath = Application.StartupPath + @"\Reports\"; //ConfigurationSettings.AppSettings["ReportPath"];
                //crystalReportViewer1.ReportSource = filePath + @"rptHistory.rpt";

                ReportDocument rptdc = new ReportDocument();
                rptdc.Load(filePath + @"rptCurrentParks.rpt");
                rptdc.SetDataSource(ds);
                //Bind ReportSource to crystalReportViewer
                crystalReportViewer1.ReportSource = rptdc;

            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally
            { 
            }
        }

        public void LoadHistoryReport(DataSet ds)
        {
            try
            {
                string filePath = Application.StartupPath + @"\Reports\";
                    
                    //ConfigurationSettings.AppSettings["ReportPath"]; // @"C:\NILE\L2 Software\JANUARY\Day6\ARCPSGUI\ARCPSGUI\TransactionUI\"; // ConfigurationSettings.AppSettings["ReportPath"];
                //crystalReportViewer1.ReportSource = filePath + @"rptHistory.rpt";
                ds.Namespace = "CURRENTPARKS";
                ds.Tables[0].TableName = "CURRENTPARKS";
                ReportDocument rptdc = new ReportDocument();
                rptdc.Load(filePath + @"rptHistory.rpt");
                rptdc.SetDataSource(ds);
                //Bind ReportSource to crystalReportViewer
                crystalReportViewer1.ReportSource = rptdc;

            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally
            {
            }
        }
        public void LoadDelayedHistoryReport(DataSet ds)
        {
            try
            {
                string filePath = Application.StartupPath + @"\Reports\";
              //  ds.Namespace = "DELAY_HISTORY_VIEW";
               // ds.Tables[0].TableName = "DELAY_HISTORY_VIEW";
                ReportDocument rptdc = new ReportDocument();
                rptdc.Load(filePath + @"DelayedHist.rpt");
                rptdc.SetDataSource(ds);
                crystalReportViewer1.ReportSource = rptdc;

            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally
            {
            }
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
