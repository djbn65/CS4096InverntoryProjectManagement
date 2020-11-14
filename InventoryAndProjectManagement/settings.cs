using System;
using System.Configuration;

namespace InventoryAndProjectManagement
{
    internal class Settings
    {
        public static String GetConnection() 
        {
            if (ConfigurationManager.AppSettings.Get("RemoteServer") != "true")
            {
                return $"Server={ConfigurationManager.AppSettings.Get("ServerLocation")};Database={ConfigurationManager.AppSettings.Get("Database")};User Id={ConfigurationManager.AppSettings.Get("Username")};Password={ConfigurationManager.AppSettings.Get("Password")};";
            }
            else
            {
                return $"Server={ConfigurationManager.AppSettings.Get("ServerLocation")},{ConfigurationManager.AppSettings.Get("Port")};Database={ConfigurationManager.AppSettings.Get("Database")};User Id={ConfigurationManager.AppSettings.Get("Username")};Password={ConfigurationManager.AppSettings.Get("Password")};";
            }
        }
    }
}
