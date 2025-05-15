using System;
using System.Threading.Tasks;
using System.Windows;
using FoxyBlueLight.Views;
using SplashScreen = FoxyBlueLight.Views.SplashScreen;

namespace FoxyBlueLight
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Gestion des exceptions non gérées
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Lancer le splash screen
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            
            // Créer et préparer la fenêtre principale en arrière-plan
            var mainWindow = new ModernWidget();
            
            // Quand le splash screen se ferme, montrer la fenêtre principale
            splashScreen.Closed += (s, args) => mainWindow.Show();
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorMessage = e.ExceptionObject.ToString();
            MessageBox.Show($"Une erreur inattendue s'est produite :\n\n{errorMessage}", 
                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}