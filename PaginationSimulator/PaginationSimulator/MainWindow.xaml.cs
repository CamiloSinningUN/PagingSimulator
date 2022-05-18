using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PaginationSimulator.src;

namespace PaginationSimulator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            //int tammarco = 64;
            //int tamproc = 64;
            //int tamso = 64;
            //int tammp = 128;
            //byte alg = PagBajoDem.FIFO;

            //PagBajoDem simul = new PagBajoDem(tammarco, tamproc, tamso, tammp);

            //simul.InitMarcos(genMarcosInit(simul.numMarcos));
            //simul.alg = alg;

            //List<Instruc> instruc = genInstruc(simul.tamProc, 10);
            //simul.printMarcos();
            //for (int i = 0; i < instruc.Count; i++)
            //    simul.ExInstruc(instruc[i], i);

            //simul.printMarcos();
            //simul.printMarcosUsage();
        }

        //static bool[] genMarcosInit(int numMarcos)
        //{
        //    Random rnd = new Random();
        //    bool[] marcos = new bool[numMarcos];
        //    for (int i = 0; i < numMarcos; i++)
        //        marcos[i] = rnd.Next(10) >= 3;
        //    return marcos;
        //}

        //static List<Instruc> genInstruc(int tamProc, int numInst)
        //{
        //    List<Instruc> instruc = new List<Instruc>();
        //    Random rnd = new Random();
        //    for (int i = 0; i < numInst; i++)
        //        instruc.Add(new Instruc(rnd.Next(0, tamProc), rnd.Next(10) < 5));
        //    return instruc;
        //}

        private int convertUnit(ComboBox CB, int tam)
        {
            int conv = tam;
            

            switch (CB.SelectedItem.ToString().Split()[1])
            {
                case "KB":
                    conv *= (int)Math.Pow(2, 10);
                    break;
                case "MB":
                    conv *= (int)Math.Pow(2, 20);
                    break;
            }
            Console.WriteLine(conv);
            return conv;
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
            PagBajoDem sim = null;
            try
            {
                int tamPageVal, tamProVal, tamSOVal, tamMemVal;
                
                checked
                {
                    tamPageVal = convertUnit(tamPageCB, int.Parse(tamPage.Text));
                    tamProVal = convertUnit(tamProCB, int.Parse(tamPro.Text));
                    tamSOVal = convertUnit(tamSOCB, int.Parse(tamSO.Text));
                    tamMemVal = convertUnit(tamMemCB, int.Parse(tamMem.Text));
                }
                sim = new PagBajoDem(tamPageVal, tamProVal, tamSOVal, tamMemVal);

                Window1 subWindow = new Window1(sim);
                subWindow.Show();
                this.Hide();
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);

            }
            catch (OverflowException ex)
            {
                Console.WriteLine(ex.Message);

            }
            catch (PagBajoDem.PagBajoDemException ex)
            {
                Console.WriteLine(ex.Message);
                switch (ex.Type)
                {
                    case PagBajoDem.PagBajoDemException.MARCO_EXCEPTION: 
                        break;
                    case PagBajoDem.PagBajoDemException.MARCO_POW_OF_2_EXCEPTION: //hacer algo
                        break;
                    case PagBajoDem.PagBajoDemException.MP_EXCEPTION: //hacer algo
                        break;
                    case PagBajoDem.PagBajoDemException.MP_POW_OF_2_EXCEPTION: //hacer algo
                        break;
                    case PagBajoDem.PagBajoDemException.PROC_EXCEPTION: //hacer algo
                        break;
                    case PagBajoDem.PagBajoDemException.SO_AND_PROC_EXCEPTION: //hacer algo
                        break;
                    case PagBajoDem.PagBajoDemException.SO_EXCEPTION: //hacer algo
                        break;
                    default: //hacer algo
                        break;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
