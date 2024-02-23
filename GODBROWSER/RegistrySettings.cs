using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace DanTup.BrowserSelector
{
    static class RegistrySettings
    {
        const string AppID = "DanTup.BrowserSelector";
        const string AppName = "DanTup's Browser Selector";
        const string AppDescription = "DanTup's Browser Selector";

        internal static void RegisterBrowser(string customExePath)
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            string appIcon = appPath + ",0";
            string appOpenUrlCommand = customExePath + " %1";

            // Extract the executable name from the path
            string exeName = Path.GetFileNameWithoutExtension(customExePath);

            // Register application.
            var appReg = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\{exeName}");

            // Register capabilities.
            var capabilityReg = appReg.CreateSubKey("Capabilities");
            capabilityReg.SetValue("ApplicationName", exeName);
            capabilityReg.SetValue("ApplicationIcon", appIcon);
            capabilityReg.SetValue("ApplicationDescription", $"{exeName} Web Browser");

            // Set up protocols we want to handle.
            var urlAssocReg = capabilityReg.CreateSubKey("URLAssociations");
            urlAssocReg.SetValue("http", $"{exeName}URL");
            urlAssocReg.SetValue("https", $"{exeName}URL");
            urlAssocReg.SetValue("ftp", $"{exeName}URL");

            // Register as application.
            Registry.LocalMachine.OpenSubKey("SOFTWARE\\RegisteredApplications", true).SetValue(exeName, $"SOFTWARE\\{exeName}\\Capabilities");

            // Set URL Handler.
            var handlerReg = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\Classes\\{exeName}URL");
            handlerReg.SetValue("", exeName);
            handlerReg.SetValue("FriendlyTypeName", $"{exeName} Web Browser");

            handlerReg.CreateSubKey("shell\\open\\command").SetValue("", appOpenUrlCommand);
        }

        internal static void UnregisterBrowser()
        {
            Registry.LocalMachine.DeleteSubKeyTree(string.Format("SOFTWARE\\{0}", AppID), false);
            Registry.LocalMachine.OpenSubKey("SOFTWARE\\RegisteredApplications", true).DeleteValue(AppID);
            Registry.LocalMachine.DeleteSubKey(string.Format("SOFTWARE\\Classes\\{0}URL", AppID));
        }
    }
}
