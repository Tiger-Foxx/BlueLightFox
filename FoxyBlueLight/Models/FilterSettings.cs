using System;
using System.Windows.Media;

namespace FoxyBlueLight.Models
{
    public enum FilterMode
    {
        Temperature,
        Warm,
        VeryWarm,
        Sepia,
        Grayscale,
        NightRed,
        Custom
    }
    
    public enum AttenuationType
    {
        GammaAdjust,    // Ajustement gamma traditionnel
        ColorOverlay,   // Superposition de couleur (moins intense pour les yeux)
        ReduceBrightness // Réduction de luminosité avec préservation des couleurs
    }

    public class FilterSettings
    {
        // Paramètres principaux
        public bool IsEnabled { get; set; } = false;
        public FilterMode Mode { get; set; } = FilterMode.Temperature;
        public AttenuationType AttenuationType { get; set; } = AttenuationType.ColorOverlay;
        public double Opacity { get; set; } = 0.85; // Transparence du widget
        
        // Paramètres de couleur
        public int ColorTemperature { get; set; } = 4500; // Kelvin (1900-6500)
        public double Intensity { get; set; } = 0.5;      // 0.0-1.0
        public double Brightness { get; set; } = 1.0;     // 0.1-1.0
        
        // Valeurs RGB pour le mode personnalisé
        public double RedMultiplier { get; set; } = 1.0;
        public double GreenMultiplier { get; set; } = 0.9;
        public double BlueMultiplier { get; set; } = 0.8;
        
        // Paramètres de planification
        public bool UseSchedule { get; set; } = false;
        public TimeSpan ActivationTime { get; set; } = new TimeSpan(20, 0, 0); // 20:00
        public TimeSpan DeactivationTime { get; set; } = new TimeSpan(7, 0, 0); // 07:00
        
        // Couleurs prédéfinies améliorées pour l'interface
        public static readonly Color[] PresetColors = new[]
        {
            Color.FromRgb(255, 243, 224),  // Ambre chaud
            Color.FromRgb(255, 224, 178),  // Orange doux
            Color.FromRgb(255, 236, 179),  // Jaune doux
            Color.FromRgb(224, 242, 241),  // Cyan très doux
            Color.FromRgb(225, 190, 231),  // Mauve tendre
            Color.FromRgb(209, 196, 233)   // Lavande
        };
        
        // Noms des modes pour l'affichage
        public static readonly string[] ModeNames = new[]
        {
            "Température (K)",
            "Chaud",
            "Très chaud",
            "Sépia",
            "Niveaux de gris",
            "Rouge nuit",
            "Personnalisé"
        };
        
        // Noms des types d'atténuation
        public static readonly string[] AttenuationTypeNames = new[]
        {
            "Gamma (Standard)",
            "Superposition douce",
            "Luminosité préservée"
        };
        
        // Clone les paramètres
        public FilterSettings Clone()
        {
            return new FilterSettings
            {
                IsEnabled = this.IsEnabled,
                Mode = this.Mode,
                AttenuationType = this.AttenuationType,
                Opacity = this.Opacity,
                ColorTemperature = this.ColorTemperature,
                Intensity = this.Intensity,
                Brightness = this.Brightness,
                RedMultiplier = this.RedMultiplier,
                GreenMultiplier = this.GreenMultiplier,
                BlueMultiplier = this.BlueMultiplier,
                UseSchedule = this.UseSchedule,
                ActivationTime = this.ActivationTime,
                DeactivationTime = this.DeactivationTime
            };
        }
    }
}