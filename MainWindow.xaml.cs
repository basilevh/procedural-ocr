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
        private const int imageWidth = 24;
        private const int imageHeight = 24;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor); // better visualization
            characterSource = new FontCharacterSource(imageWidth, imageHeight);
            ocrModel = new NeuralOCRModel(characterSource);
        }

        private float[,] currentInput;
        private ICharacterSource characterSource;
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
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.bmp;*.png;*.jpg;*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(dialog.FileName));
                currentInput = MyBitmapTools.GetArray(image);
                
                recognize();
            }
        }
        
        private void feedExample()
        {
            var output = characterSource.Generate();
            currentInput = output.Image;
            recognize();
        }

        private void train()
        {
            var result = ocrModel.TrainModel(1024, 4, 1, 0.40f);
            // TODO: plot errors
        }

        private void test()
        {
            // Show accuracy
            var result = ocrModel.TestModel(1024);
            txtResult.Text = "Accuracy: " + (result.TotalAccuracy * 100.0).ToString("0.00") + "%"
                + Environment.NewLine + string.Join(Environment.NewLine, result.Accuracy.Select(
                    (a, i) => i + ": " + (a * 100.0).ToString("0.00") + "%"));

            // Create gallery
            var image = NetworkVisualizer.DrawTestGallery(ocrModel, characterSource, 8, 8);
            imgNetwork.Source = image;
        }

        private void recognize()
        {
            // Assumes currentInput is set correctly
            // Show input
            imgInput.Source = MyBitmapTools.GetImageGray8(currentInput);

            // Feed forward
            var result = ocrModel.ExecuteSingle(currentInput);
            txtResult.Text = "Recog: " + result.MostConfident + string.Join("",
                Enumerable.Range('0', 10).Select(c => Environment.NewLine +
                (char)c + ": " + result.GetProbability((char)c).ToString("0.0000")));
            
            // Visualize full network
            var image = NetworkVisualizer.DrawNetworkSingle((ocrModel as NeuralOCRModel).NeuralNetwork, currentInput);
            imgNetwork.Source = image;
        }
    }
}
