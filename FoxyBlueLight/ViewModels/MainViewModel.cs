using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private FilterSettings _settings;
        private string _statusText;
        private Color _previewColor;
        private DispatcherTimer _statusTimer;
        private DispatcherTimer _scheduleTimer;
        private int _modeIndex;
        private int _attenuationTypeIndex;
        
        // Événements
        public event PropertyChangedEventHandler PropertyChanged;
        
        // Propriétés
        public FilterSettings Settings
        {
            get => _settings;
            private set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }
        
        public string StatusText
        {
            get => _statusText;
            private set
            {
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }
        
        public Color PreviewColor
        {
            get => _previewColor;
            private set
            {
                _previewColor = value;
                OnPropertyChanged(nameof(PreviewColor));
            }
        }
        
        public int ModeIndex
        {
            get => _modeIndex;
            set
            {
                if (_modeIndex != value)
                {
                    _modeIndex = value;
                    Settings.Mode = (FilterMode)value;
                    OnPropertyChanged(nameof(ModeIndex));
                    OnPropertyChanged(nameof(IsTemperatureMode));
                    OnPropertyChanged(nameof(IsCustomMode));
                }
            }
        }
        
        public int AttenuationTypeIndex
        {
            get => _attenuationTypeIndex;
            set
            {
                if (_attenuationTypeIndex != value)
                {
                    _attenuationTypeIndex = value;
                    Settings.AttenuationType = (AttenuationType)value;
                    OnPropertyChanged(nameof(AttenuationTypeIndex));
                }
            }
        }
        
        // Propriétés dérivées
        public bool IsTemperatureMode => Settings.Mode == FilterMode.Temperature;
        public bool IsCustomMode => Settings.Mode == FilterMode.Custom;
        public List<string> FilterModeNames => new List<string>(FilterSettings.ModeNames);
        public List<string> AttenuationTypeNames => new List<string>(FilterSettings.AttenuationTypeNames);
        
        // Commandes
        public ICommand ToggleFilterCommand { get; }
        public ICommand ApplyChangesCommand { get; }
        public ICommand RestoreScreenCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SelectColorCommand { get; }
        
        public MainViewModel()
        {
            // Initialisation des services
            _filterService = new FilterService();
            
            // Initialisation des paramètres
            _settings = LoadSettings();
            
            // Initialiser les index des ComboBox
            _modeIndex = (int)_settings.Mode;
            _attenuationTypeIndex = (int)_settings.AttenuationType;
            
            // Mise à jour de la couleur d'aperçu
            UpdatePreviewColor();
            
            // Création des commandes
            ToggleFilterCommand = new RelayCommand(ExecuteToggleFilter);
            ApplyChangesCommand = new RelayCommand(ExecuteApplyChanges);
            RestoreScreenCommand = new RelayCommand(ExecuteRestoreScreen);
            ExitCommand = new RelayCommand(ExecuteExit);
            SelectColorCommand = new RelayCommand<Color>(ExecuteSelectPresetColor);
            
            // Timer pour les messages de statut
            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) => {
                _statusTimer.Stop();
                UpdateStatusText();
            };
            
            // Timer pour la planification
            _scheduleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _scheduleTimer.Tick += (s, e) => CheckScheduledActivation();
            _scheduleTimer.Start();
            
            // État initial
            UpdateStatusText();
            
            // Appliquer les paramètres au démarrage si nécessaire
            if (_settings.IsEnabled)
            {
                ApplyFilter();
            }
        }
        
        // Actions des commandes
        private void ExecuteToggleFilter()
        {
            Settings.IsEnabled = !Settings.IsEnabled;
            
            if (Settings.IsEnabled)
            {
                ApplyFilter();
                ShowTemporaryStatus("Protection activée");
            }
            else
            {
                _filterService.RestoreOriginalRamp();
                ShowTemporaryStatus("Protection désactivée");
            }
            
            OnPropertyChanged(nameof(Settings));
        }
        
        private void ExecuteApplyChanges()
        {
            if (Settings.IsEnabled)
            {
                ApplyFilter();
                ShowTemporaryStatus("Paramètres appliqués");
            }
            else
            {
                Settings.IsEnabled = true;
                ApplyFilter();
                ShowTemporaryStatus("Protection activée");
                OnPropertyChanged(nameof(Settings));
            }
            
            SaveSettings();
        }
        
        private void ExecuteRestoreScreen()
        {
            Settings.IsEnabled = false;
            _filterService.RestoreOriginalRamp();
            ShowTemporaryStatus("Écran restauré");
            OnPropertyChanged(nameof(Settings));
        }
        
        private void ExecuteExit()
        {
            Cleanup();
            Application.Current.Shutdown();
        }
        
        private void ExecuteSelectPresetColor(Color color)
        {
            // Extraire les composantes RGB normalisées
            Settings.RedMultiplier = color.R / 255.0;
            Settings.GreenMultiplier = color.G / 255.0;
            Settings.BlueMultiplier = color.B / 255.0;
            
            // Mettre à jour la couleur d'aperçu
            UpdatePreviewColor();
            
            // Appliquer si activé
            if (Settings.IsEnabled)
            {
                ApplyFilter();
            }
            
            OnPropertyChanged(nameof(Settings));
            ShowTemporaryStatus("Couleur appliquée");
        }
        
        // Méthodes publiques
        public void UpdateFilterMode(int modeIndex)
        {
            if (modeIndex >= 0 && modeIndex < FilterSettings.ModeNames.Length)
            {
                Settings.Mode = (FilterMode)modeIndex;
                
                OnPropertyChanged(nameof(Settings));
                OnPropertyChanged(nameof(IsTemperatureMode));
                OnPropertyChanged(nameof(IsCustomMode));
                
                ShowTemporaryStatus($"Mode: {FilterSettings.ModeNames[modeIndex]}");
            }
        }
        
        public void UpdateAttenuationType(int typeIndex)
        {
            if (typeIndex >= 0 && typeIndex < FilterSettings.AttenuationTypeNames.Length)
            {
                Settings.AttenuationType = (AttenuationType)typeIndex;
                
                OnPropertyChanged(nameof(Settings));
                
                ShowTemporaryStatus($"Type: {FilterSettings.AttenuationTypeNames[typeIndex]}");
            }
        }
        
        public void ApplyFilter()
        {
            _filterService.Apply(Settings);
        }
        
        public void UpdatePreviewColor()
        {
            if (Settings.Mode == FilterMode.Custom)
            {
                byte r = (byte)(Settings.RedMultiplier * 255);
                byte g = (byte)(Settings.GreenMultiplier * 255);
                byte b = (byte)(Settings.BlueMultiplier * 255);
                
                PreviewColor = Color.FromRgb(r, g, b);
            }
        }
        
        public void Cleanup()
        {
            // Restaurer les couleurs originales
            _filterService.RestoreOriginalRamp();
            
            // Arrêter les timers
            _statusTimer?.Stop();
            _scheduleTimer?.Stop();
            
            // Sauvegarder les paramètres
            SaveSettings();
        }
        
        // Méthodes privées
        private void UpdateStatusText()
        {
            StatusText = Settings.IsEnabled
                ? $"Protection active - {FilterSettings.ModeNames[(int)Settings.Mode]}"
                : "Protection inactive";
        }
        
        private void ShowTemporaryStatus(string message)
        {
            StatusText = message;
            _statusTimer.Stop();
            _statusTimer.Start();
        }
        
        private void CheckScheduledActivation()
        {
            if (!Settings.UseSchedule) return;
            
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            bool shouldBeActive = false;
            
            if (Settings.ActivationTime < Settings.DeactivationTime)
            {
                // Activation le même jour
                shouldBeActive = currentTime >= Settings.ActivationTime && currentTime < Settings.DeactivationTime;
            }
            else
            {
                // Activation à cheval sur deux jours (ex: 22h à 7h)
                shouldBeActive = currentTime >= Settings.ActivationTime || currentTime < Settings.DeactivationTime;
            }
            
            if (shouldBeActive != Settings.IsEnabled)
            {
                Settings.IsEnabled = shouldBeActive;
                
                if (shouldBeActive)
                {
                    ApplyFilter();
                    ShowTemporaryStatus("Activation programmée");
                }
                else
                {
                    _filterService.RestoreOriginalRamp();
                    ShowTemporaryStatus("Désactivation programmée");
                }
                
                OnPropertyChanged(nameof(Settings));
            }
        }
        
        private FilterSettings LoadSettings()
        {
            // TODO: Implémenter le chargement depuis les paramètres persistants
            // Retourner des paramètres par défaut pour l'instant
            return new FilterSettings();
        }
        
        private void SaveSettings()
        {
            // TODO: Implémenter la sauvegarde dans les paramètres persistants
        }
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        
        public void Execute(object parameter)
        {
            _execute();
        }
    }
    
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }
        
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}