using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FoxyBlueLight.Views
{
    public partial class ColorSelector : UserControl
    {
        public class ColorItem
        {
            public Color Color { get; set; }
            public string HexColor { get; set; }
            public string Name { get; set; }
            
            public ColorItem(Color color, string name)
            {
                Color = color;
                HexColor = color.ToString();
                Name = name;
            }
            
            public ColorItem(string hexCode, string name)
            {
                Color = (Color)ColorConverter.ConvertFromString(hexCode);
                HexColor = hexCode;
                Name = name;
            }
        }
        
        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(ObservableCollection<ColorItem>), typeof(ColorSelector), 
                new PropertyMetadata(new ObservableCollection<ColorItem>()));
                
        public static readonly DependencyProperty SelectColorCommandProperty =
            DependencyProperty.Register("SelectColorCommand", typeof(ICommand), typeof(ColorSelector), 
                new PropertyMetadata(null));
        
        public ObservableCollection<ColorItem> Colors
        {
            get { return (ObservableCollection<ColorItem>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
        
        public ICommand SelectColorCommand
        {
            get { return (ICommand)GetValue(SelectColorCommandProperty); }
            set { SetValue(SelectColorCommandProperty, value); }
        }
        
        public ColorSelector()
        {
            InitializeComponent();
            ColorCircles.ItemsSource = Colors;
            DataContext = this;
        }
        
        public void AddColors(IEnumerable<ColorItem> colorItems)
        {
            foreach (var item in colorItems)
            {
                Colors.Add(item);
            }
        }
    }
}