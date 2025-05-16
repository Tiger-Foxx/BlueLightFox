using System.Windows;
using FoxyBlueLight.Services;
using FoxyBlueLight.ViewModels;

namespace FoxyBlueLight
{
    public partial class App : Application
    {
        private MainViewModel _mainViewModel;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Capture de l'événement de fermeture de l'application
            Current.Exit += Current_Exit;
            
            // Capture des exceptions non gérées
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }
        
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            // Récupérer le ViewModel principal
            if (Current.MainWindow?.DataContext is MainViewModel viewModel)
            {
                // Nettoyer les ressources et restaurer l'écran
                viewModel.Cleanup();
            }
            else
            {
                // Fallback si le ViewModel n'est pas accessible
                var filterService = new FilterService();
                filterService.RestoreOriginalRamp();
            }
        }
        
        private void Current_DispatcherUnhandledException(object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Journaliser l'exception
            System.Diagnostics.Debug.WriteLine($"Exception non gérée: {e.Exception}");
            
            // Tenter de restaurer l'écran même en cas d'erreur
            try
            {
                var filterService = new FilterService();
                filterService.RestoreOriginalRamp();
            }
            catch
            {
                // Silencieux en cas d'échec
            }
            
            // Marquer l'exception comme gérée pour éviter le crash
            e.Handled = true;
            
            // Afficher un message d'erreur convivial
            MessageBox.Show("Une erreur s'est produite. L'écran a été restauré.\n\nDétails: " + 
                e.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            
            // Fermer proprement l'application
            Current.Shutdown();
        }
    }
}