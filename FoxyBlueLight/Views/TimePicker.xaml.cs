using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FoxyBlueLight.Views
{
    public partial class TimePicker : UserControl
    {
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(TimeSpan), typeof(TimePicker),
                new FrameworkPropertyMetadata(new TimeSpan(20, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));

        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public TimePicker()
        {
            InitializeComponent();
            
            // Initialisation des heures
            var hours = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i.ToString("00"));
            }
            HoursComboBox.ItemsSource = hours;
            
            // Initialisation des minutes
            var minutes = new List<string>();
            for (int i = 0; i < 60; i += 5)
            {
                minutes.Add(i.ToString("00"));
            }
            MinutesComboBox.ItemsSource = minutes;
            
            // Initialisation des valeurs
            UpdateControls();
        }
        
        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimePicker timePicker)
            {
                timePicker.UpdateControls();
            }
        }
        
        private void UpdateControls()
        {
            HoursComboBox.SelectedItem = SelectedTime.Hours.ToString("00");
            
            // Trouver la minute la plus proche par multiple de 5
            int minutes = SelectedTime.Minutes;
            int roundedMinutes = (minutes / 5) * 5;
            MinutesComboBox.SelectedItem = roundedMinutes.ToString("00");
        }
        
        private void HoursComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HoursComboBox.SelectedItem != null && MinutesComboBox.SelectedItem != null)
            {
                int hours = int.Parse(HoursComboBox.SelectedItem.ToString());
                int minutes = int.Parse(MinutesComboBox.SelectedItem.ToString());
                SelectedTime = new TimeSpan(hours, minutes, 0);
            }
        }
        
        private void MinutesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HoursComboBox.SelectedItem != null && MinutesComboBox.SelectedItem != null)
            {
                int hours = int.Parse(HoursComboBox.SelectedItem.ToString());
                int minutes = int.Parse(MinutesComboBox.SelectedItem.ToString());
                SelectedTime = new TimeSpan(hours, minutes, 0);
            }
        }
    }
}