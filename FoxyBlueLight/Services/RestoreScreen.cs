using System;
using System.Windows;
using FoxyBlueLight.Native;
using Microsoft.Win32;

namespace FoxyBlueLight.Services
{
    public static class RestoreScreen
    {
        // Restaure les couleurs de l'écran à leurs valeurs normales
        public static void RestoreNormalColors()
        {
            try
            {
                // Obtenir le handle du DC de l'écran
                IntPtr hDC = DisplayAPI.GetDC(IntPtr.Zero);
                
                // Créer une rampe gamma par défaut (linéaire)
                DisplayAPI.RAMP ramp = DisplayAPI.CreateRamp();
                
                // Remplir avec des valeurs linéaires (écran normal)
                for (int i = 0; i < 256; i++)
                {
                    ushort value = (ushort)(i * 256);
                    ramp.Red[i] = value;
                    ramp.Green[i] = value;
                    ramp.Blue[i] = value;
                }
                
                // Appliquer la rampe gamma par défaut
                bool success = DisplayAPI.SetDeviceGammaRamp(hDC, ref ramp);
                
                // Libérer le DC
                DisplayAPI.ReleaseDC(IntPtr.Zero, hDC);
                
                // Enregistrer l'état "désactivé" dans le registre
                SaveFilterStateToRegistry(false);
                
                if (success)
                {
                    MessageBox.Show("Les couleurs de l'écran ont été restaurées à leurs valeurs par défaut.", 
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("La restauration des couleurs a échoué. Essayez de redémarrer votre ordinateur.", 
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la restauration des couleurs : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Enregistre l'état du filtre dans le registre Windows
        private static void SaveFilterStateToRegistry(bool enabled)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\FoxyLightFilter"))
                {
                    key?.SetValue("FilterEnabled", enabled ? 1 : 0);
                }
            }
            catch
            {
                // Silencieux en cas d'échec
            }
        }
        
        // Lit l'état du filtre depuis le registre Windows
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