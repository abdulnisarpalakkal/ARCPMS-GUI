using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARCPSGUI.Popup
{
    public partial class VLCModeDialog : Form
    {
        public event EventHandler updateModeEvent;

        //public string machineCode
        //{
        //    get
        //    {
        //        return this.vlc_code_text.Text;
        //    }
        //    set
        //    {
        //        this.vlc_code_text.Text = value;
        //    }
        //}
        public string vlcName
        {
            get
            {
                return this.vlcNameLabel.Text;
            }
            set
            {
                this.vlcNameLabel.Text = value;
            }
        }

        public VLCModeDialog()
        {
            InitializeComponent();
            
        }

        private void mixRadio_CheckedChanged(object sender, EventArgs e)
        {
           
            Dictionary<string, string> vlcModeData = new Dictionary<string, string>();
            vlcModeData.Add("vlcMode","0");
            vlcModeData.Add("vlcName", vlcName);
            updateModeEvent(vlcModeData, e);
            this.Close();
        }

        private void entryRadio_CheckedChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> vlcModeData = new Dictionary<string, string>();
            vlcModeData.Add("vlcMode", "1");
            vlcModeData.Add("vlcName", vlcName);
            updateModeEvent(vlcModeData, e);
            this.Close();
        }

        private void exitRadio_CheckedChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> vlcModeData = new Dictionary<string, string>();
            vlcModeData.Add("vlcMode", "2");
            vlcModeData.Add("vlcName", vlcName);
            updateModeEvent(vlcModeData, e);
            this.Close();
        }
        public void initializeRadio(int vlcMode)
        {
            if (vlcMode==1)
            {
                entryRadio.Checked = true;
            }
            else if (vlcMode == 2)
            {
                exitRadio.Checked = true;
            }
            else 
            {
                mixRadio.Checked = true;
            }
        }


    }
}
