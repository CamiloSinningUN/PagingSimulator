using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PaginationSimulator.src;

namespace PaginationSimulator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MAX_MB = 1024;
        private const int MAX_KB = 1048576;
        //private const int MAX_BYTE = 1073741824;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += new CancelEventHandler(MainWindow_Closing);
            tamPage.Focus();
        }

        private void Iniciar()
        {
            HideErrors();

            int tamPageVal = CheckInput(tamPage, tamPageCB);
            int tamProVal = CheckInput(tamPro, tamProCB);
            int tamSOVal = CheckInput(tamSO, tamSOCB);
            int tamMemVal = CheckInput(tamMem, tamMemCB);

            if (tamPageVal == -1 || tamProVal == -1 || tamSOVal == -1 || tamMemVal == -1) return;

            try
            {
                PagBajoDem sim = new PagBajoDem(tamPageVal, tamProVal, tamSOVal, tamMemVal);

                Window1 subWindow = new Window1(this, sim);
                subWindow.Show();
                this.Hide();
            }
            catch (PagBajoDem.PagBajoDemException ex)
            {
                Console.WriteLine(ex.Message);
                switch (ex.Type)
                {
                    case PagBajoDem.PagBajoDemException.MARCO_EXCEPTION:
                        Error1.Content = ex.Message;
                        Pic1.Visibility = Visibility.Visible;
                        Error1.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.MARCO_POW_OF_2_EXCEPTION:
                        Error1.Content = ex.Message;
                        Pic1.Visibility = Visibility.Visible;
                        Error1.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.MP_EXCEPTION:
                        Error4.Content = ex.Message;
                        Pic4.Visibility = Visibility.Visible;
                        Error4.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.MP_POW_OF_2_EXCEPTION:
                        Error4.Content = ex.Message;
                        Pic4.Visibility = Visibility.Visible;
                        Error4.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.PROC_EXCEPTION: //hacer algo
                        Error3.Content = ex.Message;
                        Pic3.Visibility = Visibility.Visible;
                        Error3.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.SO_AND_PROC_EXCEPTION: //hacer algo
                        Error4.Content = ex.Message;
                        Pic4.Visibility = Visibility.Visible;
                        Error4.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.SO_EXCEPTION: //hacer algo
                        Error2.Content = ex.Message;
                        Pic2.Visibility = Visibility.Visible;
                        Error2.Visibility = Visibility.Visible;
                        break;
                    case PagBajoDem.PagBajoDemException.NUM_MARCOS_EXCEPTION: //hacer algo
                        Error4.Content = ex.Message;
                        Pic4.Visibility = Visibility.Visible;
                        Error4.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #region TextChanged
        private void Page_TextChanged(object sender, TextChangedEventArgs e)
        {
            tamPage.Text = Regex.Replace(tamPage.Text, "[^0-9]+", "");
        }

        private void SO_TextChanged(object sender, TextChangedEventArgs e)
        {
            tamSO.Text = Regex.Replace(tamSO.Text, "[^0-9]+", "");
        }

        private void Pro_TextChanged(object sender, TextChangedEventArgs e)
        {
            tamPro.Text = Regex.Replace(tamPro.Text, "[^0-9]+", "");
        }

        private void Mem_TextChanged(object sender, TextChangedEventArgs e)
        {
            tamMem.Text = Regex.Replace(tamMem.Text, "[^0-9]+", "");
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Iniciar();
        }

        private void Iniciar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Iniciar();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void HideErrors()
        {
            Pic1.Visibility = Visibility.Hidden;
            Error1.Visibility = Visibility.Hidden;
            Pic2.Visibility = Visibility.Hidden;
            Error2.Visibility = Visibility.Hidden;
            Pic3.Visibility = Visibility.Hidden;
            Error3.Visibility = Visibility.Hidden;
            Pic4.Visibility = Visibility.Hidden;
            Error4.Visibility = Visibility.Hidden;
        }
        
        private int CheckInput(TextBox tb, ComboBox cb)
        {
            try
            {
                int val = checked(int.Parse(tb.Text) * (int)Math.Pow(2, cb.SelectedIndex * 10));
                return val;
            }
            catch (FormatException ex)
            {
                string msg = "Debe ingresar un valor numérico";
                if (tb.Equals(tamPage))
                {
                    Error1.Content = msg;
                    Error1.Visibility = Visibility.Visible;
                    Pic1.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamSO))
                {
                    Error2.Content = msg;
                    Error2.Visibility = Visibility.Visible;
                    Pic2.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamPro))
                {
                    Error3.Content = msg;
                    Error3.Visibility = Visibility.Visible;
                    Pic3.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamMem))
                {
                    Error4.Content = msg;
                    Error4.Visibility = Visibility.Visible;
                    Pic4.Visibility = Visibility.Visible;
                }
            }
            catch(OverflowException ex)
            {
                string msg = "Valor máximo = ";
                switch(cb.SelectedIndex)
                {
                    case 0:
                        msg += $"{int.MaxValue} bytes";
                        break;
                    case 1:
                        msg += $"{MAX_KB} KB";
                        break;
                    case 2:
                        msg += $"{MAX_MB} MB";
                        break;
                }

                if (tb.Equals(tamPage))
                {
                    Error1.Content = msg;
                    Error1.Visibility = Visibility.Visible;
                    Pic1.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamSO))
                {
                    Error2.Content = msg;
                    Error2.Visibility = Visibility.Visible;
                    Pic2.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamPro))
                {
                    Error3.Content = msg;
                    Error3.Visibility = Visibility.Visible;
                    Pic3.Visibility = Visibility.Visible;
                }
                else if (tb.Equals(tamMem))
                {
                    Error4.Content = msg;
                    Error4.Visibility = Visibility.Visible;
                    Pic4.Visibility = Visibility.Visible;
                }
            }
            return -1;
        }

        private void inputGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "0") tb.Text = "";
        }

        private void inputLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "") tb.Text = "0";
        }
    }
}
