using System;
using System.Collections.Generic;
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
        public FilterMode Mode { get; set; } = FilterMode.NightRed; // Mode rouge par défaut
        public AttenuationType AttenuationType { get; set; } = AttenuationType.ReduceBrightness; // Mode luminosité préservée par défaut
        public double Opacity { get; set; } = 0.85; // Transparence du widget
    
        // Paramètres de couleur
        public int ColorTemperature { get; set; } = 4500; // Kelvin (1900-6500)
        public double Intensity { get; set; } = 0.5;      // 0.0-1.0
        public double Brightness { get; set; } = 1.0;     // 0.1-1.0
    
        // Valeurs RGB pour le mode personnalisé - réglages rouges
        public double RedMultiplier { get; set; } = 1.0;
        public double GreenMultiplier { get; set; } = 0.3; // Réduit pour obtenir une teinte plus rouge
        public double BlueMultiplier { get; set; } = 0.3;  // Réduit pour obtenir une teinte plus rouge
        
        // Paramètres de planification
        public bool UseSchedule { get; set; } = false;
        public TimeSpan ActivationTime { get; set; } = new TimeSpan(20, 0, 0); // 20:00
        public TimeSpan DeactivationTime { get; set; } = new TimeSpan(7, 0, 0); // 07:00
        
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
        
        // Couleurs prédéfinies pour le mode personnalisé
        public static readonly List<ColorInfo> PresetColors = new List<ColorInfo>
        {
            new ColorInfo("#FFE3B9", "Ambre chaud"),
            new ColorInfo("#FFD689", "Orange doux"),
            new ColorInfo("#FFECB3", "Jaune doux"),
            new ColorInfo("#E0F2F1", "Cyan très doux"),
            new ColorInfo("#E1BEE7", "Mauve tendre"),
            new ColorInfo("#D1C4E9", "Lavande")
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

    public class ColorInfo
    {
        public Color Color { get; set; }
        public string Name { get; set; }
        
        public ColorInfo(string hexCode, string name)
        {
            Color = (Color)ColorConverter.ConvertFromString(hexCode);
            Name = name;
        }
    }
}