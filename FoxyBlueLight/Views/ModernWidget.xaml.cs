using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoxyBlueLight.ViewModels;

namespace FoxyBlueLight.Views
{
    public partial class ModernWidget : Window
    {
        // Ajout de cette propriété pour accéder au ViewModel
        private MainViewModel ViewModel => DataContext as MainViewModel;
        
        public ModernWidget()
        {
            InitializeComponent();
            
            // Positionner le widget dans le coin supérieur droit au démarrage
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Top + 40;
            
            // S'abonner à l'événement Loaded pour initialiser l'UI après le chargement
            Loaded += ModernWidget_Loaded;
        }
        
        private void ModernWidget_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialiser l'état du switch en fonction des paramètres
            if (ViewModel != null)
            {
                UpdateToggleSwitchState(ViewModel.Settings.IsEnabled);
            }
        }
        
        // Méthode pour vérifier si le contrôle est chargé, à utiliser dans les gestionnaires d'événements
        private bool IsControlLoaded => IsLoaded;
        
        protected override void OnClosed(EventArgs e)
        {
            // S'assurer que les couleurs sont restaurées à la fermeture
            if (ViewModel != null)
            {
                ViewModel.Cleanup();
            }
            
            base.OnClosed(e);
        }
        
        // Déplacer la fenêtre en cliquant dessus
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide(); // Cacher au lieu de fermer complètement
        }
        
        private void ShowHideWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            }
        }
        
        // Gestion du toggle switch
        private void ToggleSwitch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel != null)
            {
                // Inverser l'état
                ViewModel.ToggleFilterCommand.Execute(null);
                
                // Mettre à jour l'apparence du toggle switch
                UpdateToggleSwitchState(ViewModel.Settings.IsEnabled);
            }
        }
        
        // Met à jour l'apparence du toggle switch
        private void UpdateToggleSwitchState(bool isOn)
        {
            // Mise à jour de la couleur de fond
            ToggleBackground.Background = new SolidColorBrush(
                isOn ? (Color)ColorConverter.ConvertFromString("#FF6700") : 
                       (Color)ColorConverter.ConvertFromString("#333333"));
            
            // Animation de la position du thumb
            double targetPosition = isOn ? 26 : 2;
            AnimateThumbPosition(targetPosition);
        }
        
        // Anime la position du thumb
        private void AnimateThumbPosition(double targetPosition)
        {
            var animation = new DoubleAnimation
            {
                To = targetPosition,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            SwitchThumb.BeginAnimation(Canvas.LeftProperty, animation);
        }
        
        // Gestion des changements de mode
        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsControlLoaded && ViewModel != null)
            {
                ViewModel.UpdateFilterMode(((ComboBox)sender).SelectedIndex);
            }
        }
        
        // Gestion des changements de type d'atténuation
        private void AttenuationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsControlLoaded && ViewModel != null)
            {
                ViewModel.UpdateAttenuationType(((ComboBox)sender).SelectedIndex);
                
                // Appliquer les changements immédiatement si le filtre est activé
                if (ViewModel.Settings.IsEnabled)
                {
                    ViewModel.ApplyFilter();
                }
            }
        }
        
        // Gestion des sliders RGB
        private void RGBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsControlLoaded && ViewModel != null)
            {
                // Mettre à jour la couleur d'aperçu
                ViewModel.UpdatePreviewColor();
                
                // Appliquer le filtre si déjà activé
                if (ViewModel.Settings.IsEnabled)
                {
                    ViewModel.ApplyFilter();
                }
            }
        }
        
        // Gestion des sliders de paramètres généraux
        private void ParamSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsControlLoaded && ViewModel != null && ViewModel.Settings.IsEnabled)
            {
                ViewModel.ApplyFilter();
            }
        }
    }
}