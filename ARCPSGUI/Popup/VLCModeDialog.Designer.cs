namespace ARCPSGUI.Popup
{
    partial class VLCModeDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.entryRadio = new System.Windows.Forms.RadioButton();
            this.exitRadio = new System.Windows.Forms.RadioButton();
            this.mixRadio = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.vlcNameLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // entryRadio
            // 
            this.entryRadio.AutoSize = true;
            this.entryRadio.Location = new System.Drawing.Point(55, 68);
            this.entryRadio.Name = "entryRadio";
            this.entryRadio.Size = new System.Drawing.Size(49, 17);
            this.entryRadio.TabIndex = 0;
            this.entryRadio.TabStop = true;
            this.entryRadio.Text = "Entry";
            this.entryRadio.UseVisualStyleBackColor = true;
            this.entryRadio.Click += new System.EventHandler(this.entryRadio_CheckedChanged);
            // 
            // exitRadio
            // 
            this.exitRadio.AutoSize = true;
            this.exitRadio.Location = new System.Drawing.Point(55, 96);
            this.exitRadio.Name = "exitRadio";
            this.exitRadio.Size = new System.Drawing.Size(42, 17);
            this.exitRadio.TabIndex = 0;
            this.exitRadio.TabStop = true;
            this.exitRadio.Text = "Exit";
            this.exitRadio.UseVisualStyleBackColor = true;
            this.exitRadio.Click += new System.EventHandler(this.exitRadio_CheckedChanged);
            // 
            // mixRadio
            // 
            this.mixRadio.AutoSize = true;
            this.mixRadio.Location = new System.Drawing.Point(55, 122);
            this.mixRadio.Name = "mixRadio";
            this.mixRadio.Size = new System.Drawing.Size(41, 17);
            this.mixRadio.TabIndex = 0;
            this.mixRadio.TabStop = true;
            this.mixRadio.Text = "Mix";
            this.mixRadio.UseVisualStyleBackColor = true;
            this.mixRadio.Click += new System.EventHandler(this.mixRadio_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.vlcNameLabel);
            this.panel1.Location = new System.Drawing.Point(12, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(153, 165);
            this.panel1.TabIndex = 1;
            // 
            // vlcNameLabel
            // 
            this.vlcNameLabel.AutoSize = true;
            this.vlcNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vlcNameLabel.Location = new System.Drawing.Point(40, 15);
            this.vlcNameLabel.Name = "vlcNameLabel";
            this.vlcNameLabel.Size = new System.Drawing.Size(53, 20);
            this.vlcNameLabel.TabIndex = 0;
            this.vlcNameLabel.Text = "VLC#";
            // 
            // VLCModeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(186, 192);
            this.Controls.Add(this.mixRadio);
            this.Controls.Add(this.exitRadio);
            this.Controls.Add(this.entryRadio);
            this.Controls.Add(this.panel1);
            this.Name = "VLCModeDialog";
            this.Text = "VLC Mode";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton entryRadio;
        private System.Windows.Forms.RadioButton exitRadio;
        private System.Windows.Forms.RadioButton mixRadio;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label vlcNameLabel;
    }
}