using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace FT.Input.Data
{
    public static class Schema
    {
        public static void Validate()
        {
            if (!CheckExists() || RefreshIfNecessary())
            {
                Load();
            }
            
            if (!CheckVersion())
            {
                Update();
            }            
        }

        private static bool RefreshIfNecessary()
        {
            return bool.Parse(ConfigurationManager.AppSettings["db_refresh"]);
        }
       
        private static void Update()
        {
            ExecuteFile("patch");
        }

        private static void ExecuteFile(string configKey)
        {
            TextReader tr = new StreamReader(ConfigurationManager.AppSettings[configKey]);
            string sql = tr.ReadToEnd();
            Transaction.ExecuteNonQuery(sql);
        }

        private static bool CheckVersion()
        {
            return (GetVersion() == Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private static void Load()
        {
            ExecuteFile("schema");
            ExecuteFile("seed");
        }

        private static string GetVersion()
        {
            return (string)Transaction.ExecuteScalar("select value from properties where property = 'version'");
        }

        private static bool CheckExists()
        {
            try
            {
                GetVersion();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
