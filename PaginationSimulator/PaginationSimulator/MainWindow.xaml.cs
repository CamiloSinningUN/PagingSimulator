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
            //int tammarco = 4;
            //int tamproc = 29;
            //int tamso = 16;
            //int tammp = 64;
            //string alg = "lru";

            //pagbajodem simul = new pagbajodem(tammarco, tamproc, tamso, tammp, alg);

            //bool[] marcosinit = genmarcosinit(simul.nummarcos);
            //list<instruc> instruc = geninstruc(simul.tamproc, 10);

            //simul.start(marcosinit);
            //simul.printmarcos();
            //for (int i = 0; i < instruc.count; i++)
            //    simul.exinstruc(instruc[i], i);
            //simul.printmarcos();
            //simul.printmarcoslru();
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
                sim = new PagBajoDem(int.Parse(tamPage.Text), int.Parse(tamPro.Text), int.Parse(tamSO.Text), int.Parse(tamMem.Text), "lru");
            }catch(Exception err)
            {
                
                PagBajoDem.PagBajoDemException ex = (PagBajoDem.PagBajoDemException) err;
                switch (ex.type) {
                    case PagBajoDem.PagBajoDemException.MARCO_EXCEPTION: //hacer algo
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
                    
                    
                return;
            }

            Window1 subWindow = new Window1(sim);
            subWindow.Show();
            this.Hide();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
