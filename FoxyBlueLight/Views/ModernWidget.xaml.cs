using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FoxyBlueLight.Services;
using FoxyBlueLight.ViewModels;

namespace FoxyBlueLight.Views
{
    public partial class ModernWidget : Window
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;
        
        public ModernWidget()
        {
            InitializeComponent();
            
            // Positionner le widget dans le coin supérieur droit au démarrage
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Top + 20;
        }
        
        protected override void OnClosed(EventArgs e)
        {
            // S'assurer que les couleurs sont restaurées à la fermeture
            if (ViewModel != null)
            {
                // Désactiver le filtre avant de fermer
                ViewModel.Settings.IsEnabled = false;
                ViewModel.ApplyFilter();
                
                // Appeler la méthode Cleanup qui restaurera les couleurs
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
        
        private void RestoreScreen_Click(object sender, RoutedEventArgs e)
        {
            // Désactiver le filtre
            if (ViewModel != null)
            {
                ViewModel.Settings.IsEnabled = false;
                ViewModel.ApplyFilter();
            }
            
            // Appeler la méthode de restauration
            RestoreScreen.RestoreNormalColors();
        }
        
        private void ToggleSwitch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ToggleFilter();
            }
        }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded && ViewModel != null)
            {
                ViewModel.ApplyFilter();
            }
        }
        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded && ViewModel != null)
            {
                // Mettre à jour la couleur personnalisée
                Color color = Color.FromRgb(
                    (byte)(ViewModel.Settings.RedMultiplier * 255),
                    (byte)(ViewModel.Settings.GreenMultiplier * 255),
                    (byte)(ViewModel.Settings.BlueMultiplier * 255));
        
                ViewModel.CustomColor = color;
        
                // Cela mettra à jour PreviewColor via UpdateCustomColorMultipliers
        
                // Appliquer le filtre
                ViewModel.ApplyFilter();
            }
        }
        
        private void Mode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsLoaded && ViewModel != null)
            {
                ViewModel.UpdateFilterMode();
            }
        }
        
        // private void ToggleSwitch_MouseDown(object sender, MouseButtonEventArgs e)
        // {
        //     if (ViewModel != null)
        //     {
        //         ViewModel.ToggleFilter();
        //
        //         // Animation manuelle du switch
        //         double targetPosition = ViewModel.Settings.IsEnabled ? 22 : 0;
        //         AnimateSwitchThumb(targetPosition);
        //     }
        // }

        private void AnimateSwitchThumb(double targetPosition)
        {
            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                To = targetPosition,
                Duration = TimeSpan.FromSeconds(0.2)
            };
    
            SwitchThumb.BeginAnimation(Canvas.LeftProperty, animation);
        }

// Mise à jour de la position initiale au chargement
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
    
            if (SwitchThumb != null && ViewModel != null)
            {
                Canvas.SetLeft(SwitchThumb, ViewModel.Settings.IsEnabled ? 22 : 0);
            }
        }
        
        
    }
}