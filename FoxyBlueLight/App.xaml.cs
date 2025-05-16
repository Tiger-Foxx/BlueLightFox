using System;
using System.Threading;
using System.Windows;
using FoxyBlueLight.Views;
using SplashScreen = FoxyBlueLight.Views.SplashScreen;

namespace FoxyBlueLight
{
    public partial class App : Application
    {
        private Mutex _mutex = null;
        private const string AppName = "FoxyBlueLightFilter";

        protected override void OnStartup(StartupEventArgs e)
        {
            // Vérifier si l'application est déjà en cours d'exécution
            bool createdNew;
            _mutex = new Mutex(true, AppName, out createdNew);
            
            if (!createdNew)
            {
                MessageBox.Show("FoxyBlueLight est déjà en cours d'exécution.", "Information", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }
            
            base.OnStartup(e);
            
            // Afficher le SplashScreen
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            
            // Fermer le SplashScreen après un délai et afficher la fenêtre principale
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                splashScreen.Close();
                
                // La fenêtre principale sera créée automatiquement via StartupUri
            };
            timer.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
            base.OnExit(e);
        }
    }
}