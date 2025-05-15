// FoxyBlueLight/Models/FilterSettings.cs
using System;

namespace FoxyBlueLight.Models
{
    // Les différents modes de filtre disponibles
    public enum FilterMode
    {
        Temperature,   // Basé sur la température de couleur (Kelvin)
        Warm,          // Filtre chaud pré-configuré
        VeryWarm,      // Filtre très chaud pré-configuré
        Sepia,         // Mode sépia
        Grayscale,     // Niveaux de gris
        PureRed,       // Rouge pur (pour la vision nocturne)
        Custom         // Paramètres RGB personnalisés
    }
    
    // Types de planification disponibles
    public enum ScheduleType
    {
        None,          // Pas de planification
        Fixed,         // Heures fixes
        Sunset         // Basé sur le coucher/lever de soleil
    }

    // Classe principale pour stocker tous les paramètres du filtre
    public class FilterSettings
    {
        // Température de couleur en Kelvin (1200K - 6500K)
        public int ColorTemperature { get; set; } = 6500;
        
        // Intensité du filtre (0.0 - 1.0)
        public double Intensity { get; set; } = 0.5;
        
        // Luminosité logicielle (0.1 - 1.0)
        public double Brightness { get; set; } = 1.0;
        
        // Mode personnalisé RGB
        public double RedMultiplier { get; set; } = 1.0;
        public double GreenMultiplier { get; set; } = 1.0;
        public double BlueMultiplier { get; set; } = 1.0;
        
        // Mode de filtre sélectionné
        public FilterMode Mode { get; set; } = FilterMode.Temperature;
        
        // Statut du filtre
        public bool IsEnabled { get; set; } = true;
        
        // Type de planification
        public ScheduleType ScheduleType { get; set; } = ScheduleType.None;
        
        // Heures planifiées (format 24h)
        public TimeSpan StartTime { get; set; } = new TimeSpan(20, 0, 0); // 20:00
        public TimeSpan EndTime { get; set; } = new TimeSpan(8, 0, 0);    // 08:00
        
        // Nom du profil (pour les sauvegardes)
        public string ProfileName { get; set; } = "Défaut";
    }
}