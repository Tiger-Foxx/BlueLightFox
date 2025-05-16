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
        // Accès au ViewModel
        private MainViewModel ViewModel => DataContext as MainViewModel;
        
        // Variables pour le redimensionnement
        private bool _isResizing = false;
        private Point _resizeStartPoint;
        private Size _originalSize;
        
        public ModernWidget()
        {
            InitializeComponent();
            
            // Positionner le widget dans le coin supérieur droit au démarrage
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Top + 40;
        }
        
        private void ModernWidget_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialiser l'interface avec les paramètres actuels
        }
        
        // Méthode pour vérifier si le contrôle est chargé
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
            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
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
        
        // Gestionnaires pour le redimensionnement personnalisé
        private void ResizeGrip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isResizing = true;
                _resizeStartPoint = e.GetPosition(this);
                _originalSize = new Size(Width, Height);
                e.Handled = true;
                Mouse.Capture((UIElement)sender);
            }
        }
        
        private void ResizeGrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isResizing)
            {
                Point currentPosition = e.GetPosition(this);
                double diffX = currentPosition.X - _resizeStartPoint.X;
                double diffY = currentPosition.Y - _resizeStartPoint.Y;
                
                Width = Math.Max(MinWidth, _originalSize.Width + diffX);
                Height = Math.Max(MinHeight, _originalSize.Height + diffY);
                
                e.Handled = true;
            }
        }
        
        private void ResizeGrip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isResizing = false;
                Mouse.Capture(null);
                e.Handled = true;
            }
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