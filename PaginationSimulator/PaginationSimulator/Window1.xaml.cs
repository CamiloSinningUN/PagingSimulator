using PaginationSimulator.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
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
using System.Threading;
using PaginationSimulator.tables;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PaginationSimulator
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public PagBajoDem sim;
        Thread t;
        Instruc[] instruc;
        ManualResetEvent mrse;
        ObservableCollection<ParseInst> instList = new ObservableCollection<ParseInst>();
        ObservableCollection<Mem> memList = new ObservableCollection<Mem>();

        public Window1(PagBajoDem sim)
        {
            InitializeComponent();

            this.sim = sim;
            this.Closing += new CancelEventHandler(Window1_Closing);
            List<tempSim> list = new List<tempSim>();
            list.Add(new tempSim(sim.tamMarco + " Bytes", sim.tamSO + " Bytes", sim.tamProc + " Bytes", sim.tamMP + " Bytes"));
            tamValues.ItemsSource = list;

            mrse = new ManualResetEvent(false);
            InstDG.ItemsSource = instList;

            // Principal memory table
            int i = 0;
            for (; i < sim.numMarcosSO; i++)
                memList.Add(new Mem(i, "SO"));

            for (; i < sim.numMarcos; i++)
                memList.Add(new Mem(i, "Libre"));

            memDG.ItemsSource = memList;

            // Secondary memory table
            // Generate aleatory positions
            List<MemSec> tempSec = new List<MemSec>();
            for (i = 0; i < sim.numPagProc; i++)
                tempSec.Add(new MemSec(i));
            secDG.ItemsSource = tempSec;

            // Page table 
            pageTableDG.ItemsSource= sim.tablaPag;

            t = null;
        }

        private void Run()
        {
            for (int i = 0; i < instruc.Length; i++)
            {
                Console.WriteLine("asdasd");
                mrse.WaitOne();
                sim.ExInstruc(instruc[i], i);
                Thread.Sleep(1000);
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            play.Visibility = Visibility.Hidden;
            method.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Visible;

            //sim.InitMarcos(genMarcosInit(sim.numMarcos));
            sim.InitMarcos(Mem.parse(memList));
            sim.alg = method.Text == "FIFO" ? PagBajoDem.FIFO : PagBajoDem.LRU;
            //instruc = genInstruc(sim.tamProc, 10);
            instruc = ParseInst.parse(instList);

            if (t == null)
            {
                Console.WriteLine("a");
                t = new Thread(new ThreadStart(Run));
                t.Start();
            }
            else
            {
                Console.WriteLine("b");
                mrse.Set();
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            play.Visibility = Visibility.Visible;
            method.Visibility = Visibility.Visible;
            pause.Visibility = Visibility.Hidden;
            Reset.Visibility = Visibility.Hidden;
            resume.Visibility = Visibility.Hidden;

            t = null;
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            resume.Visibility = Visibility.Visible;
            pause.Visibility = Visibility.Hidden;
            mrse.Reset();
        }

        private void resume_Click(object sender, RoutedEventArgs e)
        {
            pause.Visibility = Visibility.Visible;
            resume.Visibility = Visibility.Hidden;
            mrse.Reset();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
            this.Hide();
        }

        void Window1_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool[] genMarcosInit(int numMarcos)
        {
            Random rnd = new Random();
            bool[] marcos = new bool[numMarcos];
            for (int i = 0; i < numMarcos; i++)
                marcos[i] = rnd.Next(10) >= 3;
            return marcos;
        }

        private List<Instruc> genInstruc(int tamProc, int numInst)
        {
            List<Instruc> instruc = new List<Instruc>();
            Random rnd = new Random();
            for (int i = 0; i < numInst; i++)
                instruc.Add(new Instruc(rnd.Next(0, tamProc), rnd.Next(10) < 5));
            return instruc;
        }

        private void addInst_Click(object sender, RoutedEventArgs e)
        {   
            ParseInst tempInst = new ParseInst(0, "L");
            instList.Add(tempInst); 
        }

        private void OnKeyUpDir (object sender, KeyEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            //Console.WriteLine(tb.Text);
            //Console.WriteLine(e.Key);
            try
            {
                while (int.Parse(tb.Text) >= sim.tamProc)
                    tb.Text = tb.Text.Substring(0, tb.Text.Length - 1);
            }
            catch (FormatException ex)
            {
                tb.Text = "";
            }
        }

        private void lect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox temp = (TextBox) sender;
            temp.Text = temp.Text == "L" ? "E": "L";
        }

        private void mem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox temp = (TextBox)sender;
            switch (temp.Text)
            {
                case "SO":
                    break;
                case "Libre": temp.Text = "Ocupado";
                    break;
                case "Ocupado": temp.Text = "Libre";
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            if (tb.Text == "") tb.Text = "0";
        }
    }

    public class ParseInst
    {
        public ParseInst(int dir, string lec)
        {
            this.dir = dir;
            this.lec = lec;
        }

        public static Instruc[] parse(ObservableCollection<ParseInst> p)
        {
            Instruc[] instruc = new Instruc[p.Count];
            for (int i = 0; i < p.Count; i++)
                instruc[i] = new Instruc(p[i].dir, p[i].lec == "L");
            return instruc;
        }

        public int dir { get; set; }
        public string lec { get; set; }
    }

    public class Mem
    {
        public Mem(int marco, string okupa)
        {
            this.marco = marco;
            this.okupa = okupa;
        }

        public static bool[] parse(ObservableCollection<Mem> m)
        {
            bool[] marcos = new bool[m.Count];
            for (int i = 0; i < m.Count; i++)
                marcos[i] = m[i].Equals("Libre");
            return marcos;
        }

        public int marco { get; set; }
        public string okupa { get; set; }
    }
    public class MemSec
    {
        public MemSec(int num)
        {
            this.num = num;
        }

        public int num { get; set; }
    }
}
