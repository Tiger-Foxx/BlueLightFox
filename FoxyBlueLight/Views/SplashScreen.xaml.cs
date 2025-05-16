using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FoxyBlueLight.Views
{
    public partial class SplashScreen : Window
    {
        private readonly DispatcherTimer _timer;
        
        public SplashScreen()
        {
            InitializeComponent();
            
            // Timer pour fermer automatiquement après un délai
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                Close();
            };
        }
        
        private void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Démarrer l'animation
                Storyboard sb = FindResource("LoadingAnimation") as Storyboard;
                sb?.Begin();
                
                // Démarrer le timer pour fermeture automatique
                _timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur dans le SplashScreen: {ex.Message}");
            }
        }
    }
}