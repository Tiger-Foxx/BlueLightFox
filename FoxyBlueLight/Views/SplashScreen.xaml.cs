using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace FoxyBlueLight.Views
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Loaded += SplashScreen_Loaded;
        }

        private void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            // Animation du logo
            var logoAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new ElasticEase { Oscillations = 1, Springiness = 3 }
            };
            LogoScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, logoAnimation);
            LogoScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, logoAnimation);
            
            // Animation du titre
            var titleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                BeginTime = TimeSpan.FromSeconds(0.6)
            };
            AppTitle.BeginAnimation(OpacityProperty, titleAnimation);
            
            // Animation du texte de chargement
            var loadingAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                BeginTime = TimeSpan.FromSeconds(0.8)
            };
            LoadingText.BeginAnimation(OpacityProperty, loadingAnimation);
            
            // Fermer automatiquement après un délai
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2.5)
            };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }
    }
}