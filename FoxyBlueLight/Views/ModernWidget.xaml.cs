using System;
using System.Windows;
using System.Windows.Input;
using FoxyBlueLight.Services;
using FoxyBlueLight.ViewModels;

namespace FoxyBlueLight.Views
{
    public partial class ModernWidget : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        
        public ModernWidget()
        {
            InitializeComponent();
            
            // Positionner le widget dans le coin supérieur droit
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Top + 20;
        }
        
        // Déplacer la fenêtre avec la souris
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide(); // Cacher au lieu de fermer complètement
        }
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide(); // Cacher la fenêtre
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
                Focus();
            }
        }
        
        private void RestoreScreen_Click(object sender, RoutedEventArgs e)
        {
            RestoreScreen.RestoreNormalColors();
        }
        
        private void ToggleSwitch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ToggleFilter();
        }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Appliquer les changements en temps réel
            if (IsLoaded) // Évite les déclenchements pendant l'initialisation
            {
                ViewModel.ApplyFilter();
            }
        }
        
        private void Mode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsLoaded) // Évite les déclenchements pendant l'initialisation
            {
                ViewModel.UpdateFilterMode();
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
    
            // S'assurer que les couleurs sont restaurées à la fermeture de l'application
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.Cleanup();
            }
        }
    }
}