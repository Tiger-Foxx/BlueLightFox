using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FoxyBlueLight.Models;
using FoxyBlueLight.Services;

namespace FoxyBlueLight.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FilterService _filterService;
        private readonly DispatcherTimer _timer;
        
        private FilterSettings _settings;
        private ObservableCollection<FilterSettings> _profiles;
        private int _selectedProfileIndex;
        private string _statusText;
        private int _selectedModeIndex;
        private Color _customColor = Colors.White;
        private Color _previewColor = Colors.White; // Déplacé ici!
        
        // Propriétés observables
        public FilterSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ObservableCollection<FilterSettings> Profiles
        {
            get => _profiles;
            set
            {
                if (_profiles != value)
                {
                    _profiles = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public int SelectedProfileIndex
        {
            get => _selectedProfileIndex;
            set
            {
                if (_selectedProfileIndex != value)
                {
                    _selectedProfileIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set
            {
                if (_selectedModeIndex != value)
                {
                    _selectedModeIndex = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsCustomMode));
                }
            }
        }
        
        public Color CustomColor
        {
            get => _customColor;
            set
            {
                if (_customColor != value)
                {
                    _customColor = value;
                    OnPropertyChanged();
                    
                    // Mode custom automatique quand on change la couleur
                    Settings.Mode = FilterMode.Custom;
                    SelectedModeIndex = (int)FilterMode.Custom;
                    
                    // Convertir la couleur en multiplicateurs RGB
                    UpdateCustomColorMultipliers(value);
                }
            }
        }
        
        // Propriété PreviewColor déplacée ici
        public Color PreviewColor
        {
            get => _previewColor;
            set
            {
                if (_previewColor != value)
                {
                    _previewColor = value;
                    OnPropertyChanged();
                }
            }
        }
        
        // Liste des modes de filtre pour l'affichage dans le ComboBox
        public List<string> FilterModeNames { get; } = new List<string> {
            "Température (Kelvin)", "Chaud", "Très chaud", "Sépia", "Niveaux de gris", "Rouge nuit", "Personnalisé"
        };
        
        // Propriétés calculées
        public bool IsTemperatureMode => Settings.Mode == FilterMode.Temperature;
        public bool IsCustomMode => Settings.Mode == FilterMode.Custom;
        
        // Commandes
        public ICommand ToggleFilterCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand LoadProfileCommand { get; }
        public ICommand ExitCommand { get; }
        
        public MainViewModel()
        {
            // Initialiser les services
            _filterService = new FilterService();
            
            // Initialiser les paramètres
            _settings = new FilterSettings();
            _settings.IsEnabled = false; // Désactivé par défaut
            
            _profiles = new ObservableCollection<FilterSettings>();
            _statusText = "Filtre désactivé";
            
            // Définir l'indice du mode sélectionné
            _selectedModeIndex = (int)_settings.Mode;
            
            // Créer les commandes
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            SaveProfileCommand = new RelayCommand(SaveProfile);
            LoadProfileCommand = new RelayCommand(LoadProfile);
            ExitCommand = new RelayCommand(Exit);
            
            // Configurer le timer pour la planification
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            
            // Mettre à jour l'état initial
            UpdateStatusText();
        }
        
        public void UpdateCustomColor(Color color)
        {
            _customColor = color;
            OnPropertyChanged(nameof(CustomColor));
            
            // Convertir la couleur en multiplicateurs RGB
            UpdateCustomColorMultipliers(color);
            
            // Appliquer le filtre
            ApplyFilter();
        }
        
        private void UpdateCustomColorMultipliers(Color color)
        {
            // Normaliser les valeurs entre 0 et 1
            Settings.RedMultiplier = Math.Max(0.1, color.R / 255.0);
            Settings.GreenMultiplier = Math.Max(0.1, color.G / 255.0);
            Settings.BlueMultiplier = Math.Max(0.1, color.B / 255.0);
    
            OnPropertyChanged(nameof(Settings));
    
            // Mettre à jour la couleur d'aperçu
            UpdatePreviewColor();
        }

        private void UpdatePreviewColor()
        {
            // Créer une couleur à partir des multiplicateurs
            PreviewColor = Color.FromRgb(
                (byte)(Settings.RedMultiplier * 255),
                (byte)(Settings.GreenMultiplier * 255),
                (byte)(Settings.BlueMultiplier * 255)
            );
        }
        
        public void UpdateFilterMode()
        {
            Settings.Mode = (FilterMode)SelectedModeIndex;
            OnPropertyChanged(nameof(IsTemperatureMode));
            OnPropertyChanged(nameof(IsCustomMode));
            ApplyFilter();
        }
        
        public void ApplyFilter()
        {
            _filterService.Apply(Settings);
            UpdateStatusText();
        }
        
        public void ToggleFilter()
        {
            Settings.IsEnabled = !Settings.IsEnabled;
            ApplyFilter();
        }
        
        private void SaveProfile()
        {
            // Créer un nouveau profil
            var newProfile = new FilterSettings
            {
                ColorTemperature = Settings.ColorTemperature,
                Intensity = Settings.Intensity,
                Brightness = Settings.Brightness,
                RedMultiplier = Settings.RedMultiplier,
                GreenMultiplier = Settings.GreenMultiplier,
                BlueMultiplier = Settings.BlueMultiplier,
                Mode = Settings.Mode
            };
            
            Profiles.Add(newProfile);
            SelectedProfileIndex = Profiles.Count - 1;
        }
        
        private void LoadProfile()
        {
            if (SelectedProfileIndex >= 0 && SelectedProfileIndex < Profiles.Count)
            {
                var profile = Profiles[SelectedProfileIndex];
                Settings.ColorTemperature = profile.ColorTemperature;
                Settings.Intensity = profile.Intensity;
                Settings.Brightness = profile.Brightness;
                Settings.RedMultiplier = profile.RedMultiplier;
                Settings.GreenMultiplier = profile.GreenMultiplier;
                Settings.BlueMultiplier = profile.BlueMultiplier;
                Settings.Mode = profile.Mode;
                
                // Mettre à jour l'indice du mode sélectionné
                SelectedModeIndex = (int)Settings.Mode;
                
                ApplyFilter();
            }
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Vérifier la planification
            if (Settings.ScheduleType == ScheduleType.Fixed)
            {
                var now = DateTime.Now.TimeOfDay;
                bool shouldBeEnabled;
                
                if (Settings.StartTime <= Settings.EndTime)
                {
                    // Période normale (ex: 20:00 - 08:00)
                    shouldBeEnabled = now >= Settings.StartTime && now <= Settings.EndTime;
                }
                else
                {
                    // Période qui traverse minuit (ex: 22:00 - 06:00)
                    shouldBeEnabled = now >= Settings.StartTime || now <= Settings.EndTime;
                }
                
                if (Settings.IsEnabled != shouldBeEnabled)
                {
                    Settings.IsEnabled = shouldBeEnabled;
                    ApplyFilter();
                }
            }
        }
        
        private void UpdateStatusText()
        {
            StatusText = Settings.IsEnabled
                ? $"Filtre actif - Mode: {FilterModeNames[(int)Settings.Mode]}"
                : "Filtre désactivé";
        }
        
        private void Exit()
        {
            // Désactiver le filtre
            Settings.IsEnabled = false;
            ApplyFilter();
    
            // Arrêter les timers
            _timer.Stop();
    
            // Restauration complète et nettoyage des traces
            RestoreScreen.RestoreNormalColors(false); // pas de message
    
            // Quitter l'application
            Application.Current.Shutdown();
        }

        public void Cleanup()
        {
            // Arrêter les timers
            _timer.Stop();
    
            // S'assurer que l'état du filtre est "désactivé" dans le registre
            RestoreScreen.SaveFilterStateToRegistry(false);
    
            // Restaurer les couleurs de l'écran
            _filterService.RestoreOriginalRamp();
        }
        
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    // Classe RelayCommand simple pour les commandes
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        
        public void Execute(object parameter) => _execute();
        
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}