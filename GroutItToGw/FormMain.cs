using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroutItToGw
{
    public partial class FormMain : Form
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public bool FormClosePending { get; private set; }

        private AppMainService appMainService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public FormMain(AppMainService appMainService)
        {
            this.appMainService = appMainService;
            this.FormClosePending = false;
            
            InitializeComponent();
            this.appMainService.ScanProgress += updateOnScanProgress;
            this.appMainService.ScanCancelled += updateOnScanCancelled;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        private void FormMain_Load(object sender, EventArgs e)
        {
            readAppSettingsFromXmlHelper();
        }

        //event delegate BtnStart_Click
        private void btnStart_Click(object sender, EventArgs e)
        {
            readAppSettingsFromXmlHelper();
            BtnStart.Enabled = false;
            BtnStop.Enabled = true;
            TxBStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":Starting file scanning...";
            appMainService.StartFilesScan();
        }

        //event delegate BtnStop_Click
        private void btnStop_Click(object sender, EventArgs e)
        {
            BtnStop.Enabled = false;
            TxBStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":Stopping file scanning...";
            appMainService.StopFilesScan();
        }

        //event delegate BtnSettings_Click
        private void BtnSettings_Click(object sender, EventArgs e)
        {
            var settingsLoaded = readAppSettingsFromXmlHelper();
            if (!settingsLoaded) { return; }

            MessageBox.Show("Settings reloaded.\nSettings can be changed in 'AppSettings.xml' file in directory:\n " + Application.StartupPath,
                "GroutItToGw Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //event delegate BtnClose_Click
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //event delegate to subscibe to appMainService.ScanProgress event
        private void updateOnScanProgress(object sender, AppProgressEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, AppProgressEventArgs>(updateOnScanProgress), sender, args);
                return;
            }

            TxBStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + args.progressMessage;
        }

        //event delegate to subscibe to appMainService.ScanCancelled event
        private void updateOnScanCancelled(object sender, AppProgressEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, AppProgressEventArgs>(updateOnScanCancelled), sender, args);
                return;
            }
            BtnStart.Enabled = true;
            BtnStop.Enabled = false;
            TxBStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + args.progressMessage;
            if (FormClosePending) { this.Close(); }
        }

        //event delegate FormMain_FormClosing
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (appMainService.ScanIsRunning)
            {
                this.Enabled = false;
                FormClosePending = true;
                e.Cancel = true;
                TxBStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":Closing GroutItToGw...";
                appMainService.StopFilesScan();
            }
        }

        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //readAppSettings from xmlFile, show message box is exception thrown
        private bool readAppSettingsFromXmlHelper()
        {
            var settingsLoaded = false;
            try
            {
                appMainService.ReadAppSettingsFromXml();
                settingsLoaded = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error loading settings: " + exception.GetBaseException().Message, "GroutItToGw Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return settingsLoaded;
        }

        #endregion
        
    }
}
