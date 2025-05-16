using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using FoxyBlueLight.Native;
using Microsoft.Win32;
using static FoxyBlueLight.Native.DisplayAPI;

namespace FoxyBlueLight.Services
{
    public static class RestoreScreen
    {
        // Restaure les couleurs de l'écran et nettoie toutes les traces
        public static void RestoreNormalColors(bool showMessage = true)
        {
            try
            {
                // 1. Restaurer les couleurs de l'écran
                RestoreScreenColors();
                
                // 2. Nettoyer toutes les entrées de registre
                CleanupRegistry();
                
                // 3. Nettoyer les fichiers temporaires
                CleanupTempFiles();
                
                // 4. Désactiver le démarrage automatique
                DisableAutostart();
                
                if (showMessage)
                {
                    MessageBox.Show(
                        "Les couleurs de l'écran ont été restaurées et toutes les traces de l'application ont été nettoyées.",
                        "Restauration complète", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show($"Erreur lors de la restauration complète : {ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        // Restaure uniquement les couleurs de l'écran
        private static void RestoreScreenColors()
        {
            try
            {
                // Obtenir le handle du DC de l'écran
                IntPtr hDC = GetDC(IntPtr.Zero);
                
                // Créer une rampe gamma par défaut (linéaire)
                RAMP ramp = CreateRamp();
                
                // Remplir avec des valeurs linéaires (écran normal)
                for (int i = 0; i < 256; i++)
                {
                    ushort value = (ushort)(i * 256);
                    ramp.Red[i] = value;
                    ramp.Green[i] = value;
                    ramp.Blue[i] = value;
                }
                
                // Appliquer la rampe gamma par défaut
                SetDeviceGammaRamp(hDC, ref ramp);
                
                // Libérer le DC
                ReleaseDC(IntPtr.Zero, hDC);
                
                // Forcer un rafraîchissement complet de l'écran
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, null, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Nettoie toutes les entrées de registre liées à l'application
        private static void CleanupRegistry()
        {
            try
            {
                // Supprimer toutes les clés de registre de l'application
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true))
                {
                    key?.DeleteSubKeyTree("FoxyLightFilter", false);
                }
                
                // Supprimer les entrées dans Run si elles existent
                using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    runKey?.DeleteValue("FoxyBlueLight", false);
                    runKey?.DeleteValue("FoxyLightFilter", false);
                }
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Nettoie les fichiers temporaires créés par l'application
        private static void CleanupTempFiles()
        {
            try
            {
                // Dossier AppData pour notre application
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FoxyBlueLight");
                
                // Dossier LocalAppData pour notre application
                string localAppDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FoxyBlueLight");
                
                // Suppression des dossiers s'ils existent
                if (Directory.Exists(appDataPath))
                {
                    Directory.Delete(appDataPath, true);
                }
                
                if (Directory.Exists(localAppDataPath))
                {
                    Directory.Delete(localAppDataPath, true);
                }
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Désactive le démarrage automatique
        private static void DisableAutostart()
        {
            try
            {
                // Supprimer de la liste des programmes au démarrage
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key?.DeleteValue("FoxyBlueLight", false);
                }
                
                // Supprimer des tâches planifiées si elles existent
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = "/Delete /TN \"FoxyBlueLight\" /F",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    // Ignorer le résultat - cela peut échouer si la tâche n'existe pas
                    process?.WaitForExit(1000);
                }
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Sauvegarde l'état du filtre dans le registre
        public static void SaveFilterStateToRegistry(bool enabled)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\FoxyLightFilter"))
                {
                    key?.SetValue("FilterEnabled", enabled ? 1 : 0);
                    key?.SetValue("LastUsed", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Lit l'état du filtre depuis le registre
        public static bool GetFilterStateFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\FoxyLightFilter"))
                {
                    if (key == null) return false;
                    return Convert.ToInt32(key.GetValue("FilterEnabled", 0)) == 1;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}