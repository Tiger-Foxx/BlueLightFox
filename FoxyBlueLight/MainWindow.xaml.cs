// FoxyBlueLight/MainWindow.xaml.cs
using System;
using System.Windows;
using FoxyBlueLight.ViewModels;

namespace FoxyBlueLight
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            // Restaurer les paramètres d'origine lors de la fermeture
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.Cleanup();
            }
        }
        private void RestoreScreen_Click(object sender, RoutedEventArgs e)
        {
            FoxyBlueLight.Services.RestoreScreen.RestoreNormalColors();
        }
    }
}