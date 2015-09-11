using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GroutItToGw
{
    public class AppSettings
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public string InputFolder { get; set; }
        public string ProcessedFolder { get; set; }
        public string ErrorFolder { get; set; }
        public string OutputFolder { get; set; }
        public string IgnoreFolder { get; set; }
        public string IgnoreFileNames { get; set; }
        public string InputFileDateTimeFormat { get; set; }

        private int folderScanSeconds;
        public int FolderScanSeconds 
        {
            get { return this.folderScanSeconds; }
            set { this.folderScanSeconds = (value >= 1 && value <= 3600) ? value : folderScanSeconds; }
        }

        private int outputFileIntervalSeconds;
        public int OutputFileIntervalSeconds
        {
            get { return this.outputFileIntervalSeconds; }
            set { this.outputFileIntervalSeconds = (value >= 1 && value <= 3600) ? value : outputFileIntervalSeconds; }
        }

        private int keepAliveIntervalSeconds;
        public int KeepAliveIntervalSeconds
        {
            get { return this.keepAliveIntervalSeconds; }
            set { this.keepAliveIntervalSeconds = (value >= 0 && value <= 14400) ? value : keepAliveIntervalSeconds; }
        }

        public string ExtCLICommand { get; set; }
        public string ExtCLICommandArgs { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AppSettings()
        {
            
            this.InputFolder = Application.StartupPath + @"\input";
            this.ProcessedFolder = Application.StartupPath + @"\processed";
            this.ErrorFolder = Application.StartupPath + @"\error";
            this.OutputFolder = Application.StartupPath + @"\output";
            this.IgnoreFolder = Application.StartupPath + @"\ignore";
            this.IgnoreFileNames = String.Empty;
            this.InputFileDateTimeFormat = @"dd/MM/yyyy HH:mm:ss";
            this.FolderScanSeconds = 5;
            this.OutputFileIntervalSeconds = 60;
            this.keepAliveIntervalSeconds = 3600;

        }
        
        //Methods--------------------------------------------------------------------------------------------------------------//

        //ReadFromXML - overload for default settings file name
        public void ReadFromXML()
        {
            ReadFromXML("AppSettings.xml");
        }

        //ReadFromXML
        public void ReadFromXML(string fileName)
        {
            var settingsFile = new XmlDocument();
            if (!File.Exists(Application.StartupPath + @"\" + fileName)) 
                { throw new ArgumentException("Settings file '" + fileName + "' does not exist."); }
            settingsFile.Load(fileName);
            var settingsMainNode = settingsFile.SelectSingleNode("AppSettings");
            if (settingsMainNode == null)
                { throw new ArgumentException(fileName + " is empty of file format not correct. Settings not loaded."); }

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var childNode in settingsMainNode.Cast<XmlNode>())
            {
                setPropertyFromXmlNode(childNode, properties);
            } 
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //savePropertyFromChildNode
        private void setPropertyFromXmlNode(XmlNode node, PropertyInfo[] properties)
        {
            var property = properties.FirstOrDefault(x => x.Name == node.Name);
            if (property != null && !String.IsNullOrEmpty(node.InnerText))
            {
                if (property.PropertyType == typeof(int)) { property.SetValue(this, int.Parse(node.InnerText), null); }
                if (property.PropertyType == typeof(String)) { property.SetValue(this, node.InnerText, null); }

            }
        } 

        #endregion


        
    }
}
