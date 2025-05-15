using System;
using System.Windows;
using System.Windows.Threading;

namespace FoxyBlueLight.Views
{
    public partial class SplashScreen : Window
    {
        private readonly DispatcherTimer _timer;
        
        public SplashScreen()
        {
            InitializeComponent();
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2.5)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            Close();
        }
    }
}