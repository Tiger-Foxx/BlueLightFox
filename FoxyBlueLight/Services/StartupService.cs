using System;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using System.Windows;

namespace FoxyBlueLight.Services
{
    public class StartupService
    {
        private const string StartupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "FoxyBlueLight";
        
        public static bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, false))
            {
                return key?.GetValue(AppName) != null;
            }
        }
        
        public static void SetStartupEnabled(bool enabled)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true))
                {
                    if (enabled)
                    {
                        string appPath = Assembly.GetExecutingAssembly().Location;
                        key?.SetValue(AppName, $"\"{appPath}\"");
                    }
                    else
                    {
                        key?.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la configuration du démarrage automatique : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}