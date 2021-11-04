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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NNLibrary.Components;
using NNLibrary;
using System.Data;
using SQLConnectionInterface;

namespace NNTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network N;
        Network N2;

        DataTable DataSet;

        List<List<double>> InputsNShit = 
            new List<List<double>>() { 
                new List<double>() { 0.05, 0.10 }
            };

        List<double> Expected = new List<double>() { 0.01, 0.99 };
        int Indexeru = -1;
        private readonly SQLInterfacer Int = new SQLInterfacer("Server=DESKTOP-JGGKP0M;Database=NeuralNetworkTest;Trusted_Connection=True;");

        public MainWindow()
        {
            InitializeComponent();
            N = new Network(3, new int[] { 4, 2, 2 }, new ActivationFunctions[] { ActivationFunctions.Linear, ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid }, 0.5);
            N2 = new Network(3, new int[] { 3, 10, 10 }, new ActivationFunctions[] { ActivationFunctions.Linear, ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid }, 0.5);
            
            List<List<double>>[] weights = new List<List<double>>[2]
            {
                new List<List<double>>() { new List<double>() { 0.15, 0.20 }, new List<double>() { 0.25, 0.30 } },
                new List<List<double>>() { new List<double>() { 0.40, 0.45 }, new List<double>() { 0.50, 0.55 } }
            };

            List<List<double>> biases = new List<List<double>>()
            {
                new List<double>() { 0.35, 0.35 },
                new List<double>() { 0.60, 0.60 },
            };

            //N.SetStartingWeights(weights);
            //N.SetBiases(biases);
            ImportDataSet();
            Plot.SetNetwork(N);
            Plot.Draw();
        }

        private void ImportDataSet()
        {
            DataSet = Int.Read("dbo.GetData");

            InputsNShit = GenerateInputList();
            Expected = GenerateResultList();
        }

        private List<List<double>> GenerateInputList()
        {
            List<List<double>> lists = new List<List<double>>();

            foreach (DataRow row in DataSet.Rows)
                lists.Add(new List<double>() { (double)row["p1"], (double)row["p2"], (double)row["p3"], (double)row["p4"] });

            return lists;
        }

        private List<double> GenerateResultList()
        {
            List<double> results = new List<double>();

            foreach (DataRow row in DataSet.Rows)
                results.Add((double)row["r"]);

            return results;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (++Indexeru == InputsNShit.Count)
                Indexeru = 0;

            N.PassInputs(InputsNShit[Indexeru].ToArray());
            N.FeedForward(Expected.ToArray());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            N.BackPropagate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            N.PassInputs(InputsNShit[0].ToArray());

            for (int i = 0; i < InputsNShit.Count; i++){
                N.PassInputs(InputsNShit[i].ToArray());
                double[] ex = new double[2] { Expected[i], Expected[i] == 1 ? 0 : 1 };
                N.FeedForward(ex);
                N.BackPropagate();
            }

            MessageBox.Show(N.GetError(Expected.ToArray()).ToString("N3"));
        }
    }
}
