using System;
using System.Runtime.InteropServices;
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
                    RestoreOriginalRamp(); // Important: Restaurer gamma normal d'abord
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
            
            return result;
        }
        
        // Méthode par superposition de couleur (plus douce pour les yeux)
        private bool ApplyColorOverlay(FilterSettings settings)
        {
            // Fermer toute fenêtre d'overlay existante d'abord
            CloseOverlayWindow();
            
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
                    // Crucial: désactiver l'interaction avec cette fenêtre
                    IsHitTestVisible = false
                };
                
                // Rendre la fenêtre vraiment transparente aux entrées utilisateur
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
                
                _overlayWindow.Loaded += (s, e) =>
                {
                    // S'assurer que la fenêtre couvre TOUT l'écran
                    _overlayWindow.Left = SystemParameters.VirtualScreenLeft;
                    _overlayWindow.Top = SystemParameters.VirtualScreenTop;
                    _overlayWindow.Width = SystemParameters.VirtualScreenWidth;
                    _overlayWindow.Height = SystemParameters.VirtualScreenHeight;
                };
            }
            
            // Obtenir la couleur pour le filtre en fonction du mode
            Color filterColor = GetFilterColorForSettings(settings);
            
            // Réduire l'opacité pour un effet plus doux, comme Windows Night Light
            // L'intensité contrôle l'opacité du filtre
            byte opacity = (byte)(settings.Intensity * 100);
            
            // Créer un pinceau semi-transparent avec la couleur du filtre
            _overlayWindow.Background = new SolidColorBrush(Color.FromArgb(
                opacity,
                filterColor.R,
                filterColor.G,
                filterColor.B
            ));
            
            // Afficher la fenêtre d'overlay
            if (!_overlayWindow.IsVisible)
            {
                _overlayWindow.Show();
            }
            
            return true;
        }
        
        // Méthode préservant mieux les couleurs avec réduction de luminosité
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
            
            // Préserver la luminosité globale tout en réduisant la lumière bleue
            double avgMultiplier = (redMultiplier + greenMultiplier + blueMultiplier) / 3.0;
            double brightnessAdjust = settings.Brightness * (1.0 / avgMultiplier);
            
            // Application d'une courbe différente qui préserve mieux les couleurs
            for (int i = 0; i < 256; i++)
            {
                double normalizedValue = i / 255.0;
                
                // Formule avancée de préservation des couleurs
                double intensity = settings.Intensity;
                double redAdjusted = Math.Pow(normalizedValue, 1.0 / redMultiplier) * redMultiplier * brightnessAdjust;
                double greenAdjusted = Math.Pow(normalizedValue, 1.0 / greenMultiplier) * greenMultiplier * brightnessAdjust;
                double blueAdjusted = Math.Pow(normalizedValue, 1.0 / blueMultiplier) * blueMultiplier * brightnessAdjust;
                
                // Appliquer l'intensité comme un mélange entre original et filtré
                redAdjusted = normalizedValue * (1.0 - intensity) + redAdjusted * intensity;
                greenAdjusted = normalizedValue * (1.0 - intensity) + greenAdjusted * intensity;
                blueAdjusted = normalizedValue * (1.0 - intensity) + blueAdjusted * intensity;
                
                // Mettre à l'échelle pour les valeurs gamma
                ramp.Red[i] = (ushort)Math.Max(0, Math.Min(65535, redAdjusted * 65535.0));
                ramp.Green[i] = (ushort)Math.Max(0, Math.Min(65535, greenAdjusted * 65535.0));
                ramp.Blue[i] = (ushort)Math.Max(0, Math.Min(65535, blueAdjusted * 65535.0));
            }
            
            bool result = SetRamp(ramp);
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
        
        // Calcule la couleur pour une température donnée (plus précise)
        private void GetColorForTemperature(int temp, out byte r, out byte g, out byte b)
        {
            // Formule de conversion température (Kelvin) → RGB similaire à f.lux
            double temperature = temp / 100.0;
            double red, green, blue;
            
            if (temperature <= 66)
            {
                red = 255;
                
                green = temperature;
                green = 99.4708025861 * Math.Log(green) - 161.1195681661;
                green = Math.Max(0, Math.Min(255, green));
                
                if (temperature <= 19)
                {
                    blue = 0;
                }
                else
                {
                    blue = temperature - 10;
                    blue = 138.5177312231 * Math.Log(blue) - 305.0447927307;
                    blue = Math.Max(0, Math.Min(255, blue));
                }
            }
            else
            {
                red = temperature - 60;
                red = 329.698727446 * Math.Pow(red, -0.1332047592);
                red = Math.Max(0, Math.Min(255, red));
                
                green = temperature - 60;
                green = 288.1221695283 * Math.Pow(green, -0.0755148492);
                green = Math.Max(0, Math.Min(255, green));
                
                blue = 255;
            }
            
            r = (byte)red;
            g = (byte)green;
            b = (byte)blue;
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
            
            // Force refresh (parfois nécessaire sur certains systèmes)
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
    }
}