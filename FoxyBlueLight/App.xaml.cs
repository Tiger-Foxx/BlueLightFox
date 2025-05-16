using System;
using System.Windows;
using FoxyBlueLight.Services;
using FoxyBlueLight.Views;
using SplashScreen = FoxyBlueLight.Views.SplashScreen;

namespace FoxyBlueLight
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Vérifier si l'application s'est fermée de manière inattendue avec le filtre activé
            if (RestoreScreen.GetFilterStateFromRegistry())
            {
                // Restaurer les couleurs normales au démarrage sans afficher de message
                RestoreScreen.RestoreNormalColors(false);
            }
            
            // Gestion des exceptions globales
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Démarrer avec le splash screen
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            
            // Préparer le widget principal
            var mainWindow = new ModernWidget();
            
            // Afficher le widget lorsque le splash screen se ferme
            splashScreen.Closed += (s, args) => mainWindow.Show();
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMessage = e.ExceptionObject.ToString();
                MessageBox.Show($"Une erreur est survenue:\n\n{errorMessage}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                           
                // En cas d'erreur, restaurer les couleurs de l'écran
                RestoreScreen.RestoreNormalColors(false);
            }
            catch
            {
                // Dernier recours si même la gestion d'erreur échoue
            }
        }
    }
}