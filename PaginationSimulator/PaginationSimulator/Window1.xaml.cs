using PaginationSimulator.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaginationSimulator
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public PagBajoDem sim;

        public Window1(PagBajoDem sim)
        {
            InitializeComponent();
            this.sim = sim;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            play.Visibility = Visibility.Hidden;
            method.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Visible;
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            play.Visibility = Visibility.Visible;
            method.Visibility = Visibility.Visible;
            pause.Visibility = Visibility.Hidden;
            Reset.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
