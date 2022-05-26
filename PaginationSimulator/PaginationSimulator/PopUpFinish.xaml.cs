using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para PopUpFinish.xaml
    /// </summary>
    public partial class PopUpFinish : Window
    {
        string bitacora;
        public PopUpFinish(string reemps, string fallos, string bitac)
        {
            InitializeComponent();
            reempText.Text = reemps;
            fallText.Text = fallos;
            bitacora = bitac;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void guardar_Click(object sender, RoutedEventArgs e)
        {
            //pedir path para generar txt
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();
            Console.WriteLine(dialog.SelectedPath);
            try
            {
                StreamWriter file = new StreamWriter(dialog.SelectedPath + "\\Bitacora.txt");
                file.Write(bitacora);
                this.Close();
            }
            catch
            {

            }
            
        }
    }
}
