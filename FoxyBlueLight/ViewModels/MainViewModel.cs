using System;
using System.Collections.Generic;
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
        private FilterService _filterService;
        private readonly DispatcherTimer _statusTimer;

        // Propriétés observables
        private FilterSettings _settings;
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

        // Propriétés calculées pour l'interface
        private Color _previewColor;
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

        private string _statusText = "Filtre désactivé";
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

        // Propriétés calculées pour l'interface
        public List<string> FilterModeNames { get; } = new List<string>(FilterSettings.ModeNames);
        
        public bool IsTemperatureMode => Settings.Mode == FilterMode.Temperature;
        public bool IsCustomMode => Settings.Mode == FilterMode.Custom;

        // Commandes
        public ICommand ToggleFilterCommand { get; }
        public ICommand ApplyChangesCommand { get; }
        public ICommand RestoreScreenCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SelectPresetColorCommand { get; }
        
        // Propriétés pour les index des ComboBox
        private int _modeIndex;
        public int ModeIndex
        {
            get => _modeIndex;
            set
            {
                if (_modeIndex != value)
                {
                    _modeIndex = value;
                    Settings.Mode = (FilterMode)value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsTemperatureMode));
                    OnPropertyChanged(nameof(IsCustomMode));
                }
            }
        }

        private int _attenuationTypeIndex;
        public int AttenuationTypeIndex
        {
            get => _attenuationTypeIndex;
            set
            {
                if (_attenuationTypeIndex != value)
                {
                    _attenuationTypeIndex = value;
                    Settings.AttenuationType = (AttenuationType)value;
                    OnPropertyChanged();
                }
            }
        }

// Liste des noms des types d'atténuation
        public List<string> AttenuationTypeNames { get; } = new List<string>(FilterSettings.AttenuationTypeNames);

// Mise à jour du type d'atténuation
        public void UpdateAttenuationType(int typeIndex)
        {
            if (typeIndex >= 0 && typeIndex < FilterSettings.AttenuationTypeNames.Length)
            {
                Settings.AttenuationType = (AttenuationType)typeIndex;
                AttenuationTypeIndex = typeIndex;
        
                // Mettre à jour le statut
                ShowTemporaryStatus($"Type d'atténuation: {FilterSettings.AttenuationTypeNames[typeIndex]}");
            }
        }

        public MainViewModel()
        {
            // Initialisation des services
            _filterService = new FilterService();
    
            // Initialisation des paramètres
            _settings = new FilterSettings();
    
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
            SelectPresetColorCommand = new RelayCommand<Color>(ExecuteSelectPresetColor);
    
            // Timer pour les messages de statut
            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += (s, e) => {
                _statusTimer.Stop();
                UpdateStatusText();
            };
    
            // État initial
            UpdateStatusText();
        }
        
        // Met à jour la couleur d'aperçu basée sur les multiplicateurs actuels
        public void UpdatePreviewColor()
        {
            PreviewColor = Color.FromRgb(
                (byte)(Settings.RedMultiplier * 255),
                (byte)(Settings.GreenMultiplier * 255),
                (byte)(Settings.BlueMultiplier * 255)
            );
        }
        
        // Met à jour les multiplicateurs RGB basés sur une couleur donnée
        public void UpdateFromColor(Color color)
        {
            Settings.RedMultiplier = Math.Max(0.1, color.R / 255.0);
            Settings.GreenMultiplier = Math.Max(0.1, color.G / 255.0);
            Settings.BlueMultiplier = Math.Max(0.1, color.B / 255.0);
            
            // Mettre à jour la couleur d'aperçu
            UpdatePreviewColor();
            
            // Appliquer les changements
            ApplyFilter();
        }
        
        // Met à jour le texte de statut
        private void UpdateStatusText()
        {
            StatusText = Settings.IsEnabled
                ? $"Filtre actif - Mode: {FilterSettings.ModeNames[(int)Settings.Mode]}"
                : "Filtre désactivé";
        }
        
        // Affiche un message temporaire
        private void ShowTemporaryStatus(string message)
        {
            StatusText = message;
            _statusTimer.Stop();
            _statusTimer.Start();
        }
        
        // Commande: Active/désactive le filtre
        private void ExecuteToggleFilter()
        {
            Settings.IsEnabled = !Settings.IsEnabled;
            
            ShowTemporaryStatus(Settings.IsEnabled ? 
                "Filtre activé" : 
                "Filtre désactivé");
                
            ApplyFilter();
        }
        
        // Commande: Applique les changements actuels
        private void ExecuteApplyChanges()
        {
            ApplyFilter();
            ShowTemporaryStatus("Changements appliqués");
        }
        
        // Commande: Restaure l'écran
        private void ExecuteRestoreScreen()
        {
            Settings.IsEnabled = false;
            ApplyFilter();
            RestoreScreen.RestoreNormalColors();
        }
        
        // Commande: Quitte l'application
        private void ExecuteExit()
        {
            Cleanup();
            Application.Current.Shutdown();
        }
        
        // Commande: Sélectionne une couleur prédéfinie
        private void ExecuteSelectPresetColor(Color color)
        {
            // Passer au mode Custom
            Settings.Mode = FilterMode.Custom;
            OnPropertyChanged(nameof(IsCustomMode));
            
            // Mettre à jour les multiplicateurs et appliquer
            UpdateFromColor(color);
            
            ShowTemporaryStatus("Couleur personnalisée appliquée");
        }
        
        // Applique le filtre en fonction des paramètres actuels
        public void ApplyFilter()
        {
            _filterService.Apply(Settings);
            UpdateStatusText();
        }
        
        // Met à jour le mode de filtre
        public void UpdateFilterMode(int modeIndex)
        {
            if (modeIndex >= 0 && modeIndex < FilterSettings.ModeNames.Length)
            {
                Settings.Mode = (FilterMode)modeIndex;
                OnPropertyChanged(nameof(IsTemperatureMode));
                OnPropertyChanged(nameof(IsCustomMode));
                ApplyFilter();
            }
        }
        
        // Restauration et nettoyage
        public void Cleanup()
        {
            _statusTimer.Stop();
            
            // Désactiver le filtre
            Settings.IsEnabled = false;
            ApplyFilter();
            
            // Restauration complète
            _filterService.RestoreOriginalRamp();
        }
        
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    // Classe RelayCommand simple
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

    // Classe RelayCommand avec paramètre
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

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

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}