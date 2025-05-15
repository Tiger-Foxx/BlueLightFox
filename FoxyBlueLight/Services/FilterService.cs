// FoxyBlueLight/Services/FilterService.cs
using System;
using FoxyBlueLight.Models;
using FoxyBlueLight.Native;
using static FoxyBlueLight.Native.DisplayAPI;

namespace FoxyBlueLight.Services
{
    public class FilterService
    {
        // Stocke la rampe gamma originale pour pouvoir la restaurer
        private RAMP _originalRamp;
        private bool _hasOriginalRamp;
        private FilterSettings _currentSettings;
        
        public FilterService()
        {
            // Paramètres par défaut
            _currentSettings = new FilterSettings();
            
            // Sauvegarde de la rampe gamma originale
            SaveOriginalRamp();
        }
        
        // Applique les paramètres du filtre
        public bool Apply(FilterSettings settings)
        {
            _currentSettings = settings;
            
            // Si le filtre est désactivé, on restaure les paramètres d'origine
            if (!settings.IsEnabled)
            {
                return RestoreOriginalRamp();
            }
            
            // Sinon on calcule et applique la nouvelle rampe gamma
            RAMP ramp = CalculateRamp(settings);
            return SetRamp(ramp);
        }
        
        // Calcule la rampe gamma en fonction des paramètres
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
                    CalculateTemperatureMultipliers(settings.ColorTemperature, 
                        out redMultiplier, out greenMultiplier, out blueMultiplier);
                    break;
                case FilterMode.Warm:
                    redMultiplier = 1.0;
                    greenMultiplier = 0.9;
                    blueMultiplier = 0.7;
                    break;
                case FilterMode.VeryWarm:
                    redMultiplier = 1.0;
                    greenMultiplier = 0.7;
                    blueMultiplier = 0.4;
                    break;
                case FilterMode.Sepia:
                    redMultiplier = 1.0;
                    greenMultiplier = 0.85;
                    blueMultiplier = 0.6;
                    break;
                case FilterMode.Grayscale:
                    // Simulation de niveaux de gris
                    redMultiplier = 0.299;
                    greenMultiplier = 0.587;
                    blueMultiplier = 0.114;
                    break;
                case FilterMode.PureRed:
                    redMultiplier = 0.5;
                    greenMultiplier = 0.0;
                    blueMultiplier = 0.0;
                    break;
                case FilterMode.Custom:
                    redMultiplier = settings.RedMultiplier;
                    greenMultiplier = settings.GreenMultiplier;
                    blueMultiplier = settings.BlueMultiplier;
                    break;
            }
            
            // Appliquer l'intensité du filtre
            redMultiplier = 1 - ((1 - redMultiplier) * settings.Intensity);
            greenMultiplier = 1 - ((1 - greenMultiplier) * settings.Intensity);
            blueMultiplier = 1 - ((1 - blueMultiplier) * settings.Intensity);
            
            // Appliquer la luminosité (software dimming)
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
        
        // Calcule les multiplicateurs RGB en fonction de la température de couleur
        private void CalculateTemperatureMultipliers(int temperature, out double red, out double green, out double blue)
        {
            // Algorithme de calcul de couleur en fonction de la température (Kelvin)
            // Basé sur une approximation du corps noir
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
        
        // Sauvegarde la rampe gamma originale du système
        private void SaveOriginalRamp()
        {
            _hasOriginalRamp = GetCurrentRamp(out _originalRamp);
        }
        
        // Restaure la rampe gamma originale
        public bool RestoreOriginalRamp()
        {
            if (!_hasOriginalRamp) return false;
            return SetRamp(_originalRamp);
        }
    }
}