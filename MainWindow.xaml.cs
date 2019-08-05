// 08-01-2019, BVH

using Microsoft.Win32;
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

namespace ProceduralOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int imageWidth = 20;
        private const int imageHeight = 20;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor); // better visualization
            characterGenerator = new MyCharacterGenerator(imageWidth, imageHeight);
            ocrModel = new MyOCRModel(imageWidth, imageHeight, characterGenerator);
        }

        private float[,] currentInput;
        private ICharacterGenerator characterGenerator;
        private IOCRModel ocrModel;

        private void btnExample_Click(object sender, RoutedEventArgs e)
        {
            feedExample();
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            train();
            feedExample(); // shows weights
            test(); // shows results
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            test();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.bmp;*.png;*.jpg;*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(dialog.FileName));
                currentInput = MyBitmapTools.GetArray(image);
                imageControl.Source = MyBitmapTools.GetImageGray8(currentInput);
            }
        }

        private void btnRecognize_Click(object sender, RoutedEventArgs e)
        {
            recognize();
        }

        private void feedExample()
        {
            var output = characterGenerator.Generate();
            currentInput = output.Image;
            recognize();

            // Show input
            // var image = MyBitmapTools.GetImageGray8(output.Image);
            // Visualize full network
            var image = NetworkVisualizer.Draw((ocrModel as MyOCRModel).NeuralNetwork, output.Image);
            imageControl.Source = image;
        }

        private void train()
        {
            var result = ocrModel.TrainModel(128, 8, 32);
            txtResult.Text = "Mini-batch errors over time:" + Environment.NewLine +
                string.Join(Environment.NewLine, result.BatchErrors.Select(
                    d => d.ToString("0.000000")));
        }

        private void test()
        {
            var result = ocrModel.TestModel(1024);
            txtResult.Text = "Accuracy:" + Environment.NewLine +
                ((double)result.Correct / result.Tested * 100.0).ToString("0.00") + "%";
        }

        private void recognize()
        {
            // Assumes currentInput is set correctly
            var result = ocrModel.ExecuteSingle(currentInput);
            txtResult.Text = "Recog: " + result.MostConfident + string.Join("",
                Enumerable.Range('0', 10).Select(c => Environment.NewLine +
                (char)c + ": " + result.GetProbability((char)c).ToString("0.0000")));
        }
    }
}
