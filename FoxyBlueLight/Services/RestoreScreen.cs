using System;
using System.Windows;
using FoxyBlueLight.Native;

namespace FoxyBlueLight.Services
{
    public static class RestoreScreen
    {
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
                    ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = (ushort)(i * 256);
                }
                
                // Appliquer la rampe gamma par défaut
                DisplayAPI.SetDeviceGammaRamp(hDC, ref ramp);
                
                // Libérer le DC
                DisplayAPI.ReleaseDC(IntPtr.Zero, hDC);
                
                MessageBox.Show("Les couleurs de l'écran ont été restaurées avec succès.", 
                    "Écran restauré", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la restauration des couleurs : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}