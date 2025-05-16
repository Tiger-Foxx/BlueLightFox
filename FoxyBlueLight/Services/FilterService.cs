using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using FoxyBlueLight.Models;
using FoxyBlueLight.Native;
using static FoxyBlueLight.Native.DisplayAPI;

namespace FoxyBlueLight.Services
{
    public class FilterService
    {
        private RAMP _originalRamp;
        private bool _hasOriginalRamp;
        private Window _overlayWindow;
        
        public FilterService()
        {
            SaveOriginalRamp();
        }
        
        public bool Apply(FilterSettings settings)
        {
            if (!settings.IsEnabled)
            {
                // Désactiver tous les types de filtres
                CloseOverlayWindow();
                return RestoreOriginalRamp();
            }
            
            // Appliquer le type d'atténuation choisi
            switch (settings.AttenuationType)
            {
                case AttenuationType.GammaAdjust:
                    CloseOverlayWindow();
                    return ApplyGammaRamp(settings);
                
                case AttenuationType.ColorOverlay:
                    RestoreOriginalRamp(); // Restaurer gamma normal
                    return ApplyColorOverlay(settings);
                
                case AttenuationType.ReduceBrightness:
                    CloseOverlayWindow();
                    return ApplyBrightnessPreservingFilter(settings);
                
                default:
                    CloseOverlayWindow();
                    return ApplyGammaRamp(settings);
            }
        }
        
        // Méthode traditionnelle par ajustement de gamma
        private bool ApplyGammaRamp(FilterSettings settings)
        {
            RAMP ramp = CalculateRamp(settings);
            bool result = SetRamp(ramp);
            
            // Enregistrer l'état du filtre
            SaveFilterStateToRegistry(settings.IsEnabled);
            
            return result;
        }
        
        // Méthode par superposition de couleur (plus douce pour les yeux) qui permet les clics
        private bool ApplyColorOverlay(FilterSettings settings)
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new Window
                {
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Background = Brushes.Transparent,
                    Topmost = true,
                    WindowState = WindowState.Maximized,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                    // Désactiver l'interaction avec cette fenêtre
                    IsHitTestVisible = false
                };
                
                // Créer la fenêtre et appliquer les styles pour passer les clics
                _overlayWindow.SourceInitialized += (s, e) => 
                {
                    IntPtr hwnd = new WindowInteropHelper(_overlayWindow).Handle;
                    int extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
                    NativeMethods.SetWindowLong(
                        hwnd, 
                        NativeMethods.GWL_EXSTYLE, 
                        extendedStyle | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_NOACTIVATE
                    );
                };
            }
            
            Color filterColor = GetFilterColorForSettings(settings);
            double opacity = settings.Intensity * 0.5; // Réduire l'intensité pour un effet plus doux
            
            _overlayWindow.Background = new SolidColorBrush(Color.FromArgb(
                (byte)(opacity * 255),
                filterColor.R,
                filterColor.G,
                filterColor.B
            ));
            
            if (!_overlayWindow.IsVisible)
            {
                _overlayWindow.Show();
            }
            
            return true;
        }
        
        // Méthode préservant mieux les couleurs avec réduction de luminosité ajustée
        private bool ApplyBrightnessPreservingFilter(FilterSettings settings)
        {
            RAMP ramp = new RAMP
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };
            
            // Calculer les multiplicateurs en fonction du mode
            double redMultiplier = 1.0;
            double greenMultiplier = 1.0;
            double blueMultiplier = 1.0;
            
            switch (settings.Mode)
            {
                case FilterMode.Custom:
                    redMultiplier = settings.RedMultiplier;
                    greenMultiplier = settings.GreenMultiplier;
                    blueMultiplier = settings.BlueMultiplier;
                    break;
                    
                case FilterMode.Temperature:
                    CalculateTemperatureMultipliers(settings.ColorTemperature, out redMultiplier, out greenMultiplier, out blueMultiplier);
                    break;
                    
                default:
                    GetMultipliersForMode(settings.Mode, out redMultiplier, out greenMultiplier, out blueMultiplier);
                    break;
            }
            
            // Appliquer une courbe de préservation des couleurs
            for (int i = 0; i < 256; i++)
            {
                double normalizedValue = i / 255.0;
                
                // Formule de préservation des couleurs avec réduction de luminosité sélective
                double adjustedRed = Math.Pow(normalizedValue, 1.0 + (1.0 - redMultiplier) * settings.Intensity);
                double adjustedGreen = Math.Pow(normalizedValue, 1.0 + (1.0 - greenMultiplier) * settings.Intensity);
                double adjustedBlue = Math.Pow(normalizedValue, 1.0 + (1.0 - blueMultiplier) * settings.Intensity);
                
                // Appliquer la luminosité
                adjustedRed = adjustedRed * redMultiplier * settings.Brightness;
                adjustedGreen = adjustedGreen * greenMultiplier * settings.Brightness;
                adjustedBlue = adjustedBlue * blueMultiplier * settings.Brightness;
                
                // Mettre à l'échelle
                ramp.Red[i] = (ushort)Math.Max(0, Math.Min(65535, adjustedRed * 65535.0));
                ramp.Green[i] = (ushort)Math.Max(0, Math.Min(65535, adjustedGreen * 65535.0));
                ramp.Blue[i] = (ushort)Math.Max(0, Math.Min(65535, adjustedBlue * 65535.0));
            }
            
            bool result = SetRamp(ramp);
            SaveFilterStateToRegistry(settings.IsEnabled);
            
            return result;
        }
        
        // Ferme la fenêtre de superposition de couleur
        private void CloseOverlayWindow()
        {
            if (_overlayWindow != null && _overlayWindow.IsVisible)
            {
                _overlayWindow.Hide();
            }
        }
        
        // Obtient la couleur du filtre en fonction des paramètres
        private Color GetFilterColorForSettings(FilterSettings settings)
        {
            byte r, g, b;
            
            switch (settings.Mode)
            {
                case FilterMode.Temperature:
                    GetColorForTemperature(settings.ColorTemperature, out r, out g, out b);
                    break;
                
                case FilterMode.Custom:
                    r = (byte)(settings.RedMultiplier * 255);
                    g = (byte)(settings.GreenMultiplier * 255);
                    b = (byte)(settings.BlueMultiplier * 255);
                    break;
                
                default:
                    GetColorForMode(settings.Mode, out r, out g, out b);
                    break;
            }
            
            return Color.FromRgb(r, g, b);
        }
        
        // Calcule la couleur pour une température donnée
        private void GetColorForTemperature(int temp, out byte r, out byte g, out byte b)
        {
            double red, green, blue;
            CalculateTemperatureMultipliers(temp, out red, out green, out blue);
            
            r = (byte)(red * 255);
            g = (byte)(green * 255);
            b = (byte)(blue * 255);
        }
        
        // Calcule la couleur pour un mode prédéfini
        private void GetColorForMode(FilterMode mode, out byte r, out byte g, out byte b)
        {
            switch (mode)
            {
                case FilterMode.Warm:
                    r = 255;
                    g = 230;
                    b = 179;
                    break;
                
                case FilterMode.VeryWarm:
                    r = 255;
                    g = 213;
                    b = 102;
                    break;
                
                case FilterMode.Sepia:
                    r = 255;
                    g = 225;
                    b = 153;
                    break;
                
                case FilterMode.Grayscale:
                    r = g = b = 200;
                    break;
                
                case FilterMode.NightRed:
                    r = 180;
                    g = 0;
                    b = 0;
                    break;
                
                default:
                    r = 255;
                    g = 230;
                    b = 179;
                    break;
            }
        }
        
        // Récupère les multiplicateurs RGB pour un mode donné
        private void GetMultipliersForMode(FilterMode mode, out double red, out double green, out double blue)
        {
            switch (mode)
            {
                case FilterMode.Warm:
                    red = 1.0;
                    green = 0.9;
                    blue = 0.7;
                    break;
                
                case FilterMode.VeryWarm:
                    red = 1.0;
                    green = 0.7;
                    blue = 0.4;
                    break;
                
                case FilterMode.Sepia:
                    red = 1.0;
                    green = 0.85;
                    blue = 0.6;
                    break;
                
                case FilterMode.Grayscale:
                    red = 0.3;
                    green = 0.59;
                    blue = 0.11;
                    break;
                
                case FilterMode.NightRed:
                    red = 0.7;
                    green = 0.1;
                    blue = 0.1;
                    break;
                
                default:
                    red = 1.0;
                    green = 0.9;
                    blue = 0.7;
                    break;
            }
        }
        
        // Méthode traditionnelle de calcul des rampes gamma
        private RAMP CalculateRamp(FilterSettings settings)
        {
            RAMP ramp = CreateRamp();
            double redMultiplier = 1.0;
            double greenMultiplier = 1.0;
            double blueMultiplier = 1.0;
            
            // Calcul des multiplicateurs selon le mode
            switch (settings.Mode)
            {
                case FilterMode.Temperature:
                    CalculateTemperatureMultipliers(settings.ColorTemperature, out redMultiplier, out greenMultiplier, out blueMultiplier);
                    break;
                case FilterMode.Custom:
                    redMultiplier = settings.RedMultiplier;
                    greenMultiplier = settings.GreenMultiplier;
                    blueMultiplier = settings.BlueMultiplier;
                    break;
                default:
                    GetMultipliersForMode(settings.Mode, out redMultiplier, out greenMultiplier, out blueMultiplier);
                    break;
            }
            
            // Appliquer l'intensité du filtre
            redMultiplier = 1.0 - (settings.Intensity * (1.0 - redMultiplier));
            greenMultiplier = 1.0 - (settings.Intensity * (1.0 - greenMultiplier));
            blueMultiplier = 1.0 - (settings.Intensity * (1.0 - blueMultiplier));
            
            // Appliquer la luminosité
            redMultiplier *= settings.Brightness;
            greenMultiplier *= settings.Brightness;
            blueMultiplier *= settings.Brightness;
            
            // Génération de la rampe gamma
            for (int i = 0; i < 256; i++)
            {
                double value = i / 255.0;
                
                // Correction gamma pour une transition plus naturelle
                double correctedValue = Math.Pow(value, 1.0 / 2.2); // Gamma 2.2
                
                // Appliquer les multiplicateurs et mettre à l'échelle
                ramp.Red[i] = (ushort)Math.Max(0, Math.Min(65535, correctedValue * redMultiplier * 65535.0));
                ramp.Green[i] = (ushort)Math.Max(0, Math.Min(65535, correctedValue * greenMultiplier * 65535.0));
                ramp.Blue[i] = (ushort)Math.Max(0, Math.Min(65535, correctedValue * blueMultiplier * 65535.0));
            }
            
            return ramp;
        }
        
        private void CalculateTemperatureMultipliers(int temperature, out double red, out double green, out double blue)
        {
            temperature = Math.Max(1000, Math.Min(40000, temperature)) / 100;
            
            if (temperature <= 66)
            {
                red = 1.0;
                green = Math.Max(0, Math.Min(1.0, 0.39008157876901960784 * Math.Log(temperature) - 0.63184144378862745098));
            }
            else
            {
                red = Math.Max(0, Math.Min(1.0, 1.29293618606274509804 * Math.Pow(temperature - 60, -0.1332047592)));
                green = Math.Max(0, Math.Min(1.0, 1.12989086089529411765 * Math.Pow(temperature - 60, -0.0755148492)));
            }
            
            if (temperature >= 66)
                blue = 1.0;
            else if (temperature <= 19)
                blue = 0.0;
            else
                blue = Math.Max(0, Math.Min(1.0, 0.54320678911019607843 * Math.Log(temperature - 10) - 1.19625408914));
        }
        
        private void SaveOriginalRamp()
        {
            _hasOriginalRamp = GetCurrentRamp(out _originalRamp);
        }
        
        public bool RestoreOriginalRamp()
        {
            if (!_hasOriginalRamp)
                return false;
                
            bool result = SetRamp(_originalRamp);
            
            // Force refresh
            try
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, null, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            catch
            {
                // Silencieux en cas d'échec
            }
            
            return result;
        }
        
        private void SaveFilterStateToRegistry(bool isEnabled)
        {
            try
            {
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FoxyBlueLight");
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FoxyBlueLight", true);
                if (key != null)
                {
                    key.SetValue("FilterEnabled", isEnabled ? 1 : 0);
                    key.Close();
                }
            }
            catch
            {
                // Ignorer les erreurs
            }
        }
    }
}