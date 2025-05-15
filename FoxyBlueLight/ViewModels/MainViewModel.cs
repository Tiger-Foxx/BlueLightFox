using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using FoxyBlueLight.Models;
using FoxyBlueLight.Services;

namespace FoxyBlueLight.ViewModels
{
    // Class pour les points du graphique
    public class ChartPoint
    {
        public double Time { get; set; }
        public double Value { get; set; }
    }
    
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FilterService _filterService;
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _updateChartTimer;
        
        private FilterSettings _settings;
        private ObservableCollection<FilterSettings> _profiles;
        private int _selectedProfileIndex;
        private string _statusText;
        private int _selectedModeIndex;
        
        // Liste des modes de filtre pour l'affichage dans le ComboBox
        public List<string> FilterModeNames { get; } = new List<string> {
            "Température (Kelvin)", "Chaud", "Très chaud", "Sépia", "Niveaux de gris", "Rouge nuit", "Personnalisé"
        };
        
        // Graphiques
        public SeriesCollection ChartSeries { get; set; }
        private List<ChartPoint> _chartData;
        
        // Propriétés observables
        public FilterSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<FilterSettings> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                OnPropertyChanged();
            }
        }
        
        public int SelectedProfileIndex
        {
            get => _selectedProfileIndex;
            set
            {
                _selectedProfileIndex = value;
                OnPropertyChanged();
            }
        }
        
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }
        
        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set
            {
                _selectedModeIndex = value;
                OnPropertyChanged();
            }
        }
        
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
            _profiles = new ObservableCollection<FilterSettings>();
            
            // Définir l'indice du mode sélectionné
            _selectedModeIndex = (int)_settings.Mode;
            
            // Initialiser les données du graphique
            InitializeChart();
            
            // Créer les commandes
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            SaveProfileCommand = new RelayCommand(SaveProfile);
            LoadProfileCommand = new RelayCommand(LoadProfile);
            ExitCommand = new RelayCommand(Exit);
            
            // Configurer le timer pour la planification
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            
            // Timer pour mettre à jour le graphique
            _updateChartTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _updateChartTimer.Tick += UpdateChartTimer_Tick;
            _updateChartTimer.Start();
            
            // Mettre à jour l'état initial
            UpdateStatusText();
            
            // Appliquer les paramètres initiaux
            ApplyFilter();
        }
        
        private void InitializeChart()
        {
            // Configuration du graphique
            var mapper = Mappers.Xy<ChartPoint>()
                .X(point => point.Time)
                .Y(point => point.Value);
            Charting.For<ChartPoint>(mapper);
            
            // Données initiales
            _chartData = new List<ChartPoint>();
            for (int i = 0; i < 30; i++)
            {
                _chartData.Add(new ChartPoint
                {
                    Time = i,
                    Value = 0
                });
            }
            
            ChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Activité",
                    Values = new ChartValues<ChartPoint>(_chartData),
                    PointGeometry = null,
                    LineSmoothness = 0.5,
                    Stroke = System.Windows.Media.Brushes.LightBlue,
                    Fill = new System.Windows.Media.SolidColorBrush
                    {
                        Color = System.Windows.Media.Color.FromArgb(128, 173, 216, 230)
                    }
                }
            };
        }
        
        private void UpdateChartTimer_Tick(object sender, EventArgs e)
        {
            if (_chartData.Count > 30)
            {
                _chartData.RemoveAt(0);
            }
            
            // Calculer une valeur pour le graphique (basée sur l'activité du filtre)
            double newValue = Settings.IsEnabled ? 
                50 + (6500 - Settings.ColorTemperature) / 65 : 0;
            
            _chartData.Add(new ChartPoint
            {
                Time = _chartData.Count,
                Value = newValue
            });
            
            // Mettre à jour le graphique
            ChartSeries[0].Values = new ChartValues<ChartPoint>(_chartData);
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
                ProfileName = $"Profil {Profiles.Count + 1}",
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
            // S'assurer que les couleurs de l'écran sont restaurées avant de quitter
            _filterService.RestoreOriginalRamp();
    
            // Arrêter les timers
            _timer.Stop();
            _updateChartTimer.Stop();
    
            // Quitter l'application
            Application.Current.Shutdown();
        }
        
        public void Cleanup()
        {
            _timer.Stop();
            _updateChartTimer.Stop();
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