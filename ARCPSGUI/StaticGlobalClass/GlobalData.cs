using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Media.Imaging;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;

namespace ARCPSGUI.StaticGlobalClass
{
   public static class GlobalData
    {
       static string diagnosticImagePath;
       static BitmapImage diagnostic_LcmUcm = null;
       public static string eesImagePath = "";
       public static BitmapImage Diagnostic_LcmUcm
       {
           get { return GlobalData.diagnostic_LcmUcm; }
           set { GlobalData.diagnostic_LcmUcm = value; }
       }

        public static string DiagnosticImagePath
        {
            get { return diagnosticImagePath; }
        }

        static GlobalData()
        {
            LoadDiagnosticImagesPath();
           
        }

        //public static void InitializeDiagnosticImages()
        //{
        //    LoadDiagnosticLcmUcm();
        //}

       public static void GetEESImagePath()
        {
            try
            {
                string qry = "";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "select VALUE from l2_config_master where module_name = 'EES_Photo_Path' AND PROPERTY_NAME = 'EESPhotoPath'";
                        command.CommandText = qry;
                        eesImagePath = Convert.ToString(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }

        static void LoadDiagnosticImagesPath()
        {
            if (string.IsNullOrEmpty(diagnosticImagePath))
            {
                diagnosticImagePath = ConfigurationManager.AppSettings["DiagnosticImagePath"].ToString();
            }
        }
       public enum SecurityType
        {
            Unknown = 0,
            Read = 1,
            Write = 2,
            ReadWrite = 3
        };

       public enum SecurityAccess
       {
           Unknown = 0,
           Allow = 1,
           Disallow = 2
       };
       public enum CarType
       {
           LOW = 1,
           MID = 3,
           HIGH = 2
       };
    }
}
