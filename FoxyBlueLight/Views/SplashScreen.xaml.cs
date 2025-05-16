using System;
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
            Loaded += SplashScreen_Loaded;
            
            // Timer pour fermer automatiquement
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2.5)
            };
            _timer.Tick += Timer_Tick;
        }
        
        private void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            // Démarrer l'animation de chargement
            Storyboard sb = FindResource("LoadingAnimation") as Storyboard;
            sb?.Begin();
            
            // Démarrer le timer pour fermeture automatique
            _timer.Start();
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            
            // Animation de fondu au départ
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            
            fadeOut.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}